using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyLaser;
    [SerializeField] private ParticleSystem damageEffect;
    [SerializeField] private bool isMovingEnemy;
    private PlayerController playerController;
    private readonly float speed = 10f;
    private int lives = 10;
    private Vector3 posToMoveTo;
    private bool isShieldDestroyed;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        isShieldDestroyed = false;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        //Lasers start after a random amount of time so that all enemies aren't firing at the same time
        if (gameObject.CompareTag("Spiral Sphere")){
            InvokeRepeating(nameof(ShootLasers),1,0.1f);
        }
        else{
            if (gameManager.GetWave()%5 != 0 || gameManager.GetDifficulty() == GameManager.Difficulties.EASY){

                InvokeRepeating(nameof(ShootLasers),Random.Range(0,6),2f);
            }
        }
        if (isMovingEnemy){
            InvokeRepeating(nameof(ChangeEnemyPosition),1,Random.Range(5,11));
            posToMoveTo = new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-21.3f,15.8f));
        }
    }

    // Update is called once per frame    
    void Update()
    {
        if (gameManager.GetIsGameActive()){
            if (isMovingEnemy){
                transform.position = Vector3.MoveTowards(transform.position,posToMoveTo,speed*Time.deltaTime);
            }
            else if (gameObject.CompareTag("Homing Cone")){
                transform.LookAt(playerController.GetPlayerPosition());
                transform.position = Vector3.MoveTowards(transform.position,playerController.GetPlayerPosition(),2*Time.deltaTime);
            }
            else if (gameObject.CompareTag("Spiral Sphere")){
                    transform.Rotate(0,20*Time.deltaTime,0);
            }
            else{
                transform.Rotate(0,speed*Time.deltaTime,0);
            }
            if (gameObject.transform.childCount == 1){
                if (!isShieldDestroyed){
                    if (gameManager.GetNumOfEnemies() == 1){
                        Destroy(transform.GetChild(0).gameObject);
                        isShieldDestroyed = true;
                    }
                }
            }
        }
    }

    void ShootLasers(){
        GameObject upLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
        upLaser.GetComponent<EnemyLaser>().SetSelectedDirection(EnemyLaser.Direction.FORWARD);
        if (!gameObject.CompareTag("Homing Cone")){
            GameObject rightLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
            rightLaser.GetComponent<EnemyLaser>().SetSelectedDirection(EnemyLaser.Direction.RIGHT);
            GameObject backLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
            backLaser.GetComponent<EnemyLaser>().SetSelectedDirection(EnemyLaser.Direction.BACK);
            GameObject leftLaser = Instantiate(enemyLaser, transform.position, transform.rotation);
            leftLaser.GetComponent<EnemyLaser>().SetSelectedDirection(EnemyLaser.Direction.LEFT);
        }
    }

    void ChangeEnemyPosition(){
        posToMoveTo = new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-21.3f,15.8f));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Laser")){
            lives--;
            ParticleSystem damageEffectCopy = Instantiate(damageEffect,transform.position,transform.rotation);
            damageEffectCopy.Play();
            Destroy(damageEffectCopy.gameObject,damageEffectCopy.main.duration);
            Destroy(collision.gameObject);
            if(lives<=0){
                gameManager.DecreaseNumOfEnemies();
                Destroy(gameObject);
            }
        }   
    }
}
