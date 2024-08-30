using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public enum Direction{FORWARD,RIGHT,BACK,LEFT};
    [SerializeField] private Material orange;
    [SerializeField] private ParticleSystem destroyEffectPurple;
    [SerializeField] private ParticleSystem destroyEffectOrange;
    private Direction direction;
    private GameManager gameManager;
    private string enemyTag;
    private bool isOrange;
    private readonly float speed = 10f;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Unity methods

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        int randNum = Random.Range(1,21);
        //This selection process decides if a laser is a destroyable one (orange) or not (purple)
        if(randNum % 3 == 0){
            if(!gameManager.GetIsSpiralWave()){
                GetComponent<MeshRenderer>().material = orange;
                isOrange = true;
            }
        }
        else{
            if(gameManager.GetIsSpiralWave()){
                //The odds are reversed for spiral waves so that it's easier to get a destroyable laser
                //This creates more clusters of lasers that the player can destroy and pass through
                GetComponent<MeshRenderer>().material = orange;
                isOrange = true;
            }
        }
    }

    void Update()
    {
        switch(direction){
            case Direction.FORWARD:
                transform.Translate(speed * Time.deltaTime * Vector3.forward);
                break;
            case Direction.RIGHT:
                transform.Translate(speed * Time.deltaTime * Vector3.right);
                break;
            case Direction.BACK:
                transform.Translate(speed * Time.deltaTime * Vector3.back);
                break;
            case Direction.LEFT:
                transform.Translate(speed * Time.deltaTime * Vector3.left);
                break;
        }
        gameManager.MovementRestrictions(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Laser") && isOrange){
            gameManager.PlayHitEffect(destroyEffectOrange,transform.position);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        if(other.gameObject.CompareTag("Enemy Box")){
            if(isOrange){
                gameManager.PlayHitEffect(destroyEffectOrange,transform.position);
            }
            else{
                gameManager.PlayHitEffect(destroyEffectPurple,transform.position);
            }
            Destroy(gameObject);
        }   
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Get methods

    public string GetEnemyTag(){
        return enemyTag;
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Set methods

    public void SetSelectedDirection(Direction direction){
        this.direction = direction;
    }

    public void SetEnemyTag(string enemyTag){
        this.enemyTag = enemyTag;
    }
}
