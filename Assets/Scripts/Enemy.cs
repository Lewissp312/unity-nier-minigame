using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyLaser;
    public bool isMovingEnemy;
    private float speed = 10f;
    private int lives = 10;
    private int randEnemy1Position = 0;
    private bool isShieldDestroyed;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        isShieldDestroyed = false;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        InvokeRepeating(nameof(ShootLasers),Random.Range(0,6),2);
        if (gameObject.CompareTag("Enemy 1")){
            InvokeRepeating(nameof(ChangeEnemyPosition),1,Random.Range(5,11));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingEnemy){
            transform.position = Vector3.MoveTowards(transform.position,gameManager.enemy1Positions[randEnemy1Position],speed*Time.deltaTime);
        }
        else{
            transform.Rotate(0,speed*Time.deltaTime,0);
        }
        if (gameObject.CompareTag("Enemy 4")){
            // transform.GetChild(0).gameObject.transform.position = transform.position;
            //Replace this with system in which the number of enemies for each wave is generated in gameManager.
            //This is then looked at to determine how many enemies are left.
            //When there is one enemy left, release the shield
            if (!isShieldDestroyed){
                if (GameObject.FindGameObjectsWithTag("Enemy 1").Length==0 && 
                GameObject.FindGameObjectsWithTag("Enemy 2").Length==0 && 
                GameObject.FindGameObjectsWithTag("Enemy 3").Length==0){
                    Destroy(transform.GetChild(0).gameObject);
                    isShieldDestroyed = true;
                }
            }
        }
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
        randEnemy1Position = Random.Range(0,9);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Laser")){
            lives--;
            Destroy(collision.gameObject);
            if(lives<=0){
                Destroy(gameObject);
            }
        }   
    }
}


//Original camera position: Vector3(2.61133742,34.211998,-2.5123179)
//Original camera rotation: Quaternion(0.000846950687,-0.709633112,0.704570472,0.000848291093)
