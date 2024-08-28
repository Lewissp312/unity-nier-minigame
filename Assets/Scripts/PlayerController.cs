using System.Collections;
// using System.Numerics;
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
    private bool isDamageable = true;
    private bool hasTouchedEnemyBox = false;
    private GameManager gameManager;
    private string[] controllers;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate(){
        if (gameManager.GetIsGameActive()){
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            Vector3 movement = new(-horizontalMovement, 0.0f, -verticalMovement);
            rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movement);
            if (gameManager.GetIsUsingController()){
                float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
                float rightStickVertical = Input.GetAxis("RightStickVertical");
                Vector3 direction = new(-rightStickHorizontal, 0.0f, rightStickVertical);
                if (direction.magnitude > 0.1f) // Prevents jitter when the stick is near the center
                {
                    // direction.Normalize();
                    Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                    rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 20 * Time.fixedDeltaTime));
                    // rb.MoveRotation(targetRotation);
                }
            }
            rb.velocity = Vector3.zero;
        }
    }

    //old: 1000, 1000, snap disabled
    //new: 3,3,snap enabled

    //Vector3 direction = new(-rightStickVertical, 0.0f, rightStickHorizontal); - bottom right and top left work
    //Vector3 direction = new(-rightStickHorizontal, 0.0f, -rightStickHorizontal); - top right and bottom left work (sort of, without registering of others)

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetIsGameActive()){
            
            MovementRestrictions();
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.GetKey(KeyCode.JoystickButton5)) && canShoot){
                gameManager.GetMouseOnBoardPosition(out bool isOverPlayer);
                if (gameManager.GetIsUsingController() || !isOverPlayer){
                    laserPosition = (transform.forward * 2) + transform.position;
                    laserRotation = transform.rotation * Quaternion.Euler(90,0,0);
                    Instantiate(laser,laserPosition,laserRotation);
                    StartCoroutine(WaitToShoot());
                }
            }
            if (!gameManager.GetIsUsingController()){
                transform.LookAt(gameManager.GetMouseOnBoardPosition(out bool isOverPlayer));
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
        if (other.gameObject.CompareTag("Enemy Laser") && isDamageable){
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
                StartCoroutine(WaitForDamage());
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

    IEnumerator WaitToShoot(){
        canShoot = false;
        yield return new WaitForSeconds(0.1f);
        canShoot = true;
    }

    IEnumerator WaitForDamage(){
        isDamageable = false;
        yield return new WaitForSeconds(0.5f);
        isDamageable = true;
    }
}