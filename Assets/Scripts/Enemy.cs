using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyLaser;
    public ParticleSystem damageEffect;
    public bool isMovingEnemy = false;
    private float speed = 10f;
    private int lives = 10;
    private int randEnemy1Position = 0;
    private Vector3 posToMoveTo;
    private bool isShieldDestroyed;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        isShieldDestroyed = false;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        //Lasers start after a random amount of time so that all enemies aren't firing at the same time
        InvokeRepeating(nameof(ShootLasers),Random.Range(0,6),2);
        if (isMovingEnemy){
            InvokeRepeating(nameof(ChangeEnemyPosition),1,Random.Range(5,11));
        }
        posToMoveTo = new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-21.3f,15.8f));
    }

    // Update is called once per frame    
    void Update()
    {
        if (gameManager.GetIsGameActive()){
            if (isMovingEnemy){
            //     private float xBoundRight = -16.5f;
            //     private float xBoundLeft = 20.5f;
            //     private float zBoundDown = 15.7f;
            //     private float zBoundUp = -21.3f;
                // transform.position = Vector3.MoveTowards(transform.position,gameManager.enemy1Positions[randEnemy1Position],speed*Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position,posToMoveTo,speed*Time.deltaTime);

            }
            else{
                transform.Rotate(0,speed*Time.deltaTime,0);
            }
            if (gameObject.CompareTag("Shield Sphere")){
                // transform.GetChild(0).gameObject.transform.position = transform.position;
                //Replace this with system in which the number of enemies for each wave is generated in gameManager.
                //This is then looked at to determine how many enemies are left.
                //When there is one enemy left, release the shield
                if (!isShieldDestroyed){
                    if (gameManager.GetNumOfEnemies() == 1){
                        Destroy(transform.GetChild(0).gameObject);
                        isShieldDestroyed = true;
                    }
                }
            }
        }
    }

    public void SetIsMovingEnemy(bool isMovingEnemyValue){
        isMovingEnemy = isMovingEnemyValue;
    }

    void ShootLasers(){
        GameObject upLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
        upLaser.GetComponent<EnemyLaser>().selectedDirection = EnemyLaser.Direction.FORWARD;
        GameObject rightLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
        rightLaser.GetComponent<EnemyLaser>().selectedDirection = EnemyLaser.Direction.RIGHT;
        GameObject backLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
        backLaser.GetComponent<EnemyLaser>().selectedDirection = EnemyLaser.Direction.BACK;
        GameObject leftLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
        leftLaser.GetComponent<EnemyLaser>().selectedDirection = EnemyLaser.Direction.LEFT;
    }

    void ChangeEnemyPosition(){
        posToMoveTo = new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-21.3f,15.8f));
        randEnemy1Position = Random.Range(0,9);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Laser")){
            lives--;
            ParticleSystem damageEffectCopy = Instantiate(damageEffect,transform.position,transform.rotation);
            damageEffectCopy.Play();
            Destroy(damageEffectCopy.gameObject,damageEffectCopy.main.duration);
            // StartCoroutine(WaitForDamageEffect(effect));
            // damageEffect.Play();
            Destroy(collision.gameObject);
            if(lives<=0){
                gameManager.SetNumOfEnemies(-1);
                Destroy(gameObject);
            }
        }   
    }
}


//Original camera position: Vector3(2.61133742,34.211998,-2.5123179)
//Original camera rotation: Quaternion(0.000846950687,-0.709633112,0.704570472,0.000848291093)
