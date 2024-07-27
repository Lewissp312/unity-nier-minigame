using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyLaser;
    private float speed = 10f;
    private int lives = 10;
    private int randEnemy1Position = 0;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        InvokeRepeating(nameof(ShootLasers),Random.Range(0,6),2);
        if (gameObject.CompareTag("Enemy 1")){
            InvokeRepeating(nameof(ChangeEnemyPosition),1,Random.Range(5,11));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.CompareTag("Enemy 1")){
            transform.position = Vector3.MoveTowards(transform.position,gameManager.enemy1Positions[randEnemy1Position],speed*Time.deltaTime);
        }
        else{
            transform.Rotate(0,speed*Time.deltaTime,0);
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
