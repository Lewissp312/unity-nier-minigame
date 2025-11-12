using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject laser;
    [SerializeField] private ParticleSystem damageEffect;
    [SerializeField] private LayerMask layersToHit;
    private GameManager gameManager;
    private Vector3 laserPosition;
    private Quaternion laserRotation;
    private Rigidbody rb;
    private int lives;
    private bool canShoot;
    private bool isDamageable;
    private bool hasTouchedEnemyBox;
    private readonly float speed = 15;
    private readonly float xBoundRight = -16.5f;
    private readonly float xBoundLeft = 20.5f;
    private readonly float zBoundDown = 15.7f;
    private readonly float zBoundUp = -21.3f;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Unity methods

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        lives = 3;
        canShoot = true;
        isDamageable = true;
    }

    void FixedUpdate(){
        if(gameManager.GetIsGameActive()){
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            Vector3 movement = new(-horizontalMovement, 0.0f, -verticalMovement);
            rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movement);

            if(gameManager.GetIsUsingController()){
                float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
                float rightStickVertical = Input.GetAxis("RightStickVertical");
                Vector3 direction = new(-rightStickHorizontal, 0.0f, rightStickVertical);
                //Prevents the reading of very small movements of the right stick
                if(direction.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                    //Quaternion.Slerp makes it so that the player turns to face the new rotation,
                    //rather than facing it straight away
                    rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 20 * Time.fixedDeltaTime));
                }
            }
            //Prevents any of the enemies or lasers having an impact on the player's movement
            rb.linearVelocity = Vector3.zero;
            MovementRestrictions();
        }
    }

    void Update()
    {
        if(gameManager.GetIsGameActive()){
            if((Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || 
            Input.GetKey(KeyCode.JoystickButton5)) && canShoot){
                gameManager.GetMouseOnBoardPosition(out bool isOverPlayer);
                //Lasers will not be fired if the mouse is hovering over or near the player,
                //as this can lead to some erroneous paths for the lasers
                if(gameManager.GetIsUsingController() || !isOverPlayer){
                    laserPosition = (transform.forward * 2) + transform.position;
                    laserRotation = transform.rotation * Quaternion.Euler(90,0,0);
                    Instantiate(laser,laserPosition,laserRotation);
                    StartCoroutine(WaitToShoot());
                }
            }
            if(!gameManager.GetIsUsingController()){
                transform.LookAt(gameManager.GetMouseOnBoardPosition(out bool isOverPlayer));
            }
        }
        
    }

     void OnCollisionEnter(Collision other){
        //The hasTouchedEnemyBox bool prevents any erroneous behaviour from the player staying within the box,
        //such as being damaged repeatedly
        if(other.gameObject.CompareTag("Enemy Box") && !hasTouchedEnemyBox){
            canShoot = false;
            hasTouchedEnemyBox = true;
            gameManager.PlayHitEffect(damageEffect,transform.position);
            lives--;
            if(lives<=0){
                gameManager.EndGame("Enemy Box");
                Destroy(gameObject);
            }
            else{
                //This destroys one of the two cubes behind the player
                Destroy(transform.GetChild(0).gameObject);
            }
        }
        //isDamageable ensures that the player is not immediately killed by a cluster of lasers 
        if(other.gameObject.CompareTag("Enemy Laser") && isDamageable){
            gameManager.PlayHitEffect(damageEffect,transform.position);
            lives--;
            if(lives <= 0){
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
        if(other.gameObject.CompareTag("Enemy Box")){
            hasTouchedEnemyBox = false;
            canShoot = true;
        }
    }

    void OnCollisionStay(Collision other)
    {
        if(other.gameObject.CompareTag("Enemy Box")){
            hasTouchedEnemyBox = true;
            canShoot = false;
        }
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//PlayerController methods

    public Vector3 GetPlayerPosition(){
        return transform.position;
    }

    void MovementRestrictions(){
        Vector3 newPosition = rb.position;
        if(newPosition.x < xBoundRight ){
            newPosition.x = xBoundRight;
        } 
        else if(newPosition.x > xBoundLeft){
            newPosition.x = xBoundLeft;
        }

        if(newPosition.z > zBoundDown ){
            newPosition.z = zBoundDown;
        } 
        else if(newPosition.z < zBoundUp){
            newPosition.z = zBoundUp;
        }

        rb.MovePosition(newPosition);
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//IEnumerators

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