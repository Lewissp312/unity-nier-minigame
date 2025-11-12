using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyLaser;
    [SerializeField] private ParticleSystem damageEffect;
    [SerializeField] private bool isMovingEnemy;
    private GameManager gameManager;
    private PlayerController playerController;
    private Rigidbody rb;
    private Vector3 posToMoveTo;
    private int lives;
    private bool canHomeIn = true;
    private readonly float speed = 10f;


////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Unity methods

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        lives = 10;
        if(isMovingEnemy){
            //The time between position changes is at random intervals so enemies are not all changing them
            //at the same time
            InvokeRepeating(nameof(ChangeEnemyPosition),1,Random.Range(5,11));
            posToMoveTo = new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-21.3f,15.8f));
        }
        if(CompareTag("Spiral Sphere")){
            //Spiral sphere enemies need to fire lasers at a much faster rate than normal
            InvokeRepeating(nameof(ShootLasers),1,0.1f);
        }
        else{
            //The cylinders that spawn during a spiral wave don't shoot, so the invoke call isn't made in this case
            if(!gameManager.GetIsSpiralWave()){
                //Lasers also start at different times
                InvokeRepeating(nameof(ShootLasers),Random.Range(0,3.5f),2f);
            }
        }
    }

    void Update()
    {
        if(gameManager.GetIsGameActive()){
            //Prevents stationary enemies sliding around
            rb.linearVelocity = Vector3.zero;
            if(isMovingEnemy){
                transform.position = Vector3.MoveTowards(transform.position,posToMoveTo,speed * Time.deltaTime);
            }
            else if(CompareTag("Homing Cone")){
                transform.LookAt(playerController.GetPlayerPosition());
                if(canHomeIn){
                    transform.position = Vector3.MoveTowards(transform.position,playerController.GetPlayerPosition(),2 * Time.deltaTime);
                }
            }
            else if(CompareTag("Spiral Sphere")){
                transform.Rotate(0,20 * Time.deltaTime,0);
            }
            else{
                transform.Rotate(0,speed * Time.deltaTime,0);
            }
            //If it's a shielded enemy
            if(transform.childCount == 1 && gameManager.GetNumOfEnemies() == 1){
                //Destroys the shield
                Destroy(transform.GetChild(0).gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Laser")){
            //Prevents the player from damaging shield enemies if they manage to clip through the shield
            if (transform.childCount != 1){
                lives--;
                gameManager.PlayHitEffect(damageEffect,transform.position);
                Destroy(other.gameObject);
                if(lives <= 0){
                    gameManager.DecreaseNumOfEnemies();
                    Destroy(gameObject);
                }
            }
            else{
                gameManager.PlayHitEffect(damageEffect,transform.position);
                Destroy(other.gameObject);
            }
        }
        if(other.gameObject.CompareTag("Player") && CompareTag("Homing Cone")){
            //Prevents homing cones from clipping into the player, stopping them when they get too close
            canHomeIn = false;
        }   
    }

    void OnCollisionExit(Collision other){
        if(other.gameObject.CompareTag("Player") && CompareTag("Homing Cone")){
            canHomeIn = true;
        }    
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Enemy methods

    void ShootLasers(){
        GameObject upLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
        upLaser.GetComponent<EnemyLaser>().SetSelectedDirection(EnemyLaser.Direction.FORWARD);
        upLaser.GetComponent<EnemyLaser>().SetEnemyTag(gameObject.tag);
        if(!CompareTag("Homing Cone")){
            //Homing cones only fire lasers in front of them, so these ones are not needed for them
            GameObject rightLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
            rightLaser.GetComponent<EnemyLaser>().SetSelectedDirection(EnemyLaser.Direction.RIGHT);
            rightLaser.GetComponent<EnemyLaser>().SetEnemyTag(gameObject.tag);

            GameObject backLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
            backLaser.GetComponent<EnemyLaser>().SetSelectedDirection(EnemyLaser.Direction.BACK);
            backLaser.GetComponent<EnemyLaser>().SetEnemyTag(gameObject.tag);

            GameObject leftLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
            leftLaser.GetComponent<EnemyLaser>().SetSelectedDirection(EnemyLaser.Direction.LEFT);
            leftLaser.GetComponent<EnemyLaser>().SetEnemyTag(gameObject.tag);
        }
    }

    void ChangeEnemyPosition(){
        posToMoveTo = new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-21.3f,15.8f));
    }
}
