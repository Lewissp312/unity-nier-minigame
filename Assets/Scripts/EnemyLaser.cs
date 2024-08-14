using Unity.VisualScripting;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public enum Direction{FORWARD,RIGHT,BACK,LEFT};
    [SerializeField] private Material orange;
    [SerializeField] private ParticleSystem destroyEffectPurple;
    [SerializeField] private ParticleSystem destroyEffectOrange;
    private Direction selectedDirection;
    private readonly float speed = 10f;
    private GameManager gameManager;
    private ParticleSystem destroyEffectCopy;

    private bool isOrange;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        int randNum = Random.Range(1,21);
        //if the number is modularly divided by 3, make it purple for the 5th wave, otherwise it's orange. Just the inverse really
        if (randNum%3==0){
            //if the wave is not a special wave or the game mode is easy
            if (gameManager.GetWave()%5 != 0 || gameManager.GetDifficulty() == GameManager.Difficulties.EASY){
                GetComponent<MeshRenderer>().material = orange;
                isOrange = true;
            }
        }
        else{
            if (gameManager.GetWave()%5 == 0 && gameManager.GetDifficulty() != GameManager.Difficulties.EASY){
                GetComponent<MeshRenderer> ().material = orange;
                isOrange = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(selectedDirection){
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Laser") && isOrange){
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy Box")){
            if (isOrange){
                destroyEffectCopy = Instantiate(destroyEffectOrange,transform.position,transform.rotation);
            }
            else{
                destroyEffectCopy = Instantiate(destroyEffectPurple,transform.position,transform.rotation);
            }
            destroyEffectCopy.Play();
            Destroy(destroyEffectCopy.gameObject,destroyEffectCopy.main.duration);
            Destroy(gameObject);
        }   
    }

    public void SetSelectedDirection(Direction direction){
        selectedDirection = direction;
    }


}
