using Unity.VisualScripting;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    private float speed = 10f;
    private GameManager gameManager;
    public enum Direction{FORWARD,RIGHT,BACK,LEFT};
    public Direction selectedDirection;
    public Material orange;
    private bool isOrange;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        int randNum = Random.Range(1,21);
        if (randNum%5==0){
            GetComponent<MeshRenderer> ().material = orange;
            isOrange = true;
        }
        else{
            isOrange = false;
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
    }


}
