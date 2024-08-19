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
    private bool canShoot = true;
    private bool hasTouchedEnemyBox = false;
    private GameManager gameManager;
    private string[] controllers;

    // Start is called before the first frame update
    void Start()
    {
        //cubeRigidbody = GetComponent<Rigidbody>();
        controllers = Input.GetJoystickNames();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetIsGameActive()){
            transform.position = new Vector3(transform.position.x,0.02f,transform.position.z);
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
            float rightStickVertical = Input.GetAxis("RightStickVertical");
            transform.Translate(horizontalMovement * speed * Time.deltaTime * Vector3.left,Space.World);
            transform.Translate(verticalMovement * speed * Time.deltaTime * Vector3.back,Space.World);
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
            else{
                if (!(rightStickHorizontal > -0.9f && rightStickHorizontal < 0.9f)){
                    Debug.Log(rightStickHorizontal);
                    transform.Rotate(0,rightStickHorizontal,0);
                }
            }
        }
        
    }

    public Vector3 GetPlayerPosition(){
        return transform.position;
    }

    void MovementRestrictions(){
        if (transform.position.x < xBoundRight ){
            transform.position = new Vector3(xBoundRight,transform.position.y,transform.position.z);
        } else if(transform.position.x > xBoundLeft){
            transform.position = new Vector3(xBoundLeft,transform.position.y,transform.position.z);
        }
        if (transform.position.z > zBoundDown){
            transform.position = new Vector3(transform.position.x,transform.position.y,zBoundDown);
        } else if(transform.position.z < zBoundUp){
            transform.position = new Vector3(transform.position.x,transform.position.y,zBoundUp);
        }
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
                gameManager.EndGame();
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
                gameManager.EndGame();
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
        }
    }

    // IEnumerator BoxDamageGracePeriod(){
    //     yield return new WaitForSeconds(1);
    //     hasTouchedEnemyBox = false;
    // }
}