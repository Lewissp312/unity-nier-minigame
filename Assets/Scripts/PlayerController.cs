using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int lives = 3;
    private float speed = 15;
    private float xBoundRight = -16.5f;
    private float xBoundLeft = 20.5f;
    private float zBoundDown = 15.7f;
    private float zBoundUp = -21.3f;
    private Vector3 laserPosition;
    private Vector3 stickDirection;
    private Quaternion laserRotation;
    private bool canShoot = true;
    private GameManager gameManager;
    private string[] controllers;
    public GameObject laser;
    public ParticleSystem damageEffect;
    public LayerMask layersToHit;
    // public GameObject camera;

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
                // laserRotation = new Quaternion(laser.transform.rotation.x,transform.rotation.y,laser.transform.rotation.z,laser.transform.rotation.w);
                // Instantiate(laser,laserPosition,laserRotation);
                laserRotation = new Quaternion(transform.rotation.x,transform.rotation.y,transform.rotation.z + 5,transform.rotation.w);
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

    public float GetXBoundRight(){
        return this.xBoundRight;
    }

    public float GetXBoundLeft(){
        return this.xBoundLeft;
    }

    public float GetZBoundDown(){
        return this.zBoundDown;
    }

    public float GetZBoundUp(){
        return this.zBoundUp;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy Laser")){
            ParticleSystem damageEffectCopy = Instantiate(damageEffect,transform.position,transform.rotation);
            damageEffectCopy.Play();
            Destroy(damageEffectCopy,damageEffectCopy.main.duration);
            lives--;
            Destroy(other.gameObject);
        }
        if (lives<=0){
            gameManager.EndGame();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other){
        if (other.gameObject.CompareTag("Enemy Box")){
            lives--;
            if (lives<=0){
                gameManager.EndGame();
                Destroy(gameObject);
            }
        }
    }
}