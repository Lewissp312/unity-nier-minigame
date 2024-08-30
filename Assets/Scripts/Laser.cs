using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private ParticleSystem destroyEffect;
    private Vector3 fireDirection;
    private GameManager gameManager;
    private readonly float speed = 35;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Unity methods

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if(!gameManager.GetIsUsingController()){
            fireDirection = (gameManager.GetMouseOnBoardPosition(out bool isOverPlayer) - transform.position).normalized * speed;
            fireDirection.y = 0.3f;
        }
        else{
            fireDirection = speed * Time.deltaTime * -transform.forward;
        }
    }

    void Update()
    {
        if(gameManager.GetIsUsingController()){
            transform.Translate(speed * Time.deltaTime * -transform.forward);
        }
        else{
            transform.position += fireDirection * Time.deltaTime;
        }
        gameManager.MovementRestrictions(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Enemy Box")){
            gameManager.PlayHitEffect(destroyEffect,transform.position);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Shield")){
            gameManager.PlayHitEffect(destroyEffect,transform.position);
            Destroy(gameObject);
        }
    }
}
