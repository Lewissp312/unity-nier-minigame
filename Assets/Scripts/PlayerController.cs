using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject laser;
    [SerializeField] private ParticleSystem damageEffect;
    [SerializeField] private LayerMask layersToHit;
    private int lives = 3;
    private readonly float speed = 15;
    private readonly float xBoundRight = -16.5f;
    private readonly float xBoundLeft = 20.5f;
    private readonly float zBoundDown = 15.7f;
    private readonly float zBoundUp = -21.3f;
    private Vector3 laserPosition;
    private Vector3 stickDirection;
    private Quaternion laserRotation;
    private Rigidbody rb;
    private bool canShoot = true;
    private bool hasTouchedEnemyBox = false;
    private GameManager gameManager;
    private string[] controllers;

    // Start is called before the first frame update
    void Start()
    {
        controllers = Input.GetJoystickNames();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate(){
        if (gameManager.GetIsGameActive()){
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
            float rightStickVertical = Input.GetAxis("RightStickVertical");
            Vector3 movement = new(-horizontalMovement, 0.0f, -verticalMovement);
            rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movement);
            rb.velocity = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetIsGameActive()){
            MovementRestrictions();
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.GetKey(KeyCode.JoystickButton5)) && canShoot){
                laserPosition = (transform.forward * 2) + transform.position;
                laserRotation = transform.rotation * Quaternion.Euler(90,0,0);
                Instantiate(laser,laserPosition,laserRotation);
                StartCoroutine(WaitToShoot());
            }
            if (controllers.Length == 0){
                transform.LookAt(gameManager.GetMouseOnBoardPosition());
            }
        }
        
    }

    public Vector3 GetPlayerPosition(){
        return transform.position;
    }

    void MovementRestrictions(){
        Vector3 newPosition = rb.position;
        if (newPosition.x < xBoundRight ){
            newPosition.x = xBoundRight;
        } else if(newPosition.x > xBoundLeft){
            newPosition.x = xBoundLeft;
        }
        if (newPosition.z > zBoundDown ){
            newPosition.z = zBoundDown;
        } else if(newPosition.z < zBoundUp){
            newPosition.z = zBoundUp;
        }
        rb.MovePosition(newPosition);
    }

    IEnumerator WaitToShoot(){
        canShoot = false;
        yield return new WaitForSeconds(0.1f);
        canShoot = true;
    }

    void OnCollisionEnter(Collision other){
        if (other.gameObject.CompareTag("Enemy Box") && !hasTouchedEnemyBox){
            canShoot = false;
            hasTouchedEnemyBox = true;
            ParticleSystem damageEffectCopy = Instantiate(damageEffect,transform.position,transform.rotation);
            damageEffectCopy.Play();
            Destroy(damageEffectCopy.gameObject,damageEffectCopy.main.duration);
            lives--;
            if (lives<=0){
                gameManager.EndGame("Enemy Box");
                Destroy(gameObject);
            }
            else{
                Destroy(transform.GetChild(0).gameObject);
            }
        }
        if (other.gameObject.CompareTag("Enemy Laser")){
            ParticleSystem damageEffectCopy = Instantiate(damageEffect,transform.position,transform.rotation);
            damageEffectCopy.Play();
            Destroy(damageEffectCopy.gameObject,damageEffectCopy.main.duration);
            lives--;
            if (lives<=0){
                gameManager.EndGame(other.gameObject.GetComponent<EnemyLaser>().GetEnemyTag());
                Destroy(gameObject);
            }
            else{
                Destroy(transform.GetChild(0).gameObject);
                Destroy(other.gameObject);
            }
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy Box")){
            hasTouchedEnemyBox = false;
            canShoot = true;
            rb.velocity = Vector3.zero;
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy Box")){
            hasTouchedEnemyBox = true;
            canShoot = false;
        }
    }
}