using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float xBound = 40;
    public float zBound = 40;
    public GameObject[] enemies;
    
    private Vector3 posToSpawnOn;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemies),1,10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 GetMouseOnBoardPosition(){
        Ray ray;
        ray =  Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out RaycastHit hitData)){
            return new Vector3(hitData.point.x,hitData.point.y + 0.5f, hitData.point.z);
        } 
        else{
            return Vector3.forward;
        }
    }

    public void MovementRestrictions(GameObject objectToDestroy){
        if (objectToDestroy.transform.position.x < -40 || objectToDestroy.transform.position.x > 40 || objectToDestroy.transform.position.z > 40 || objectToDestroy.transform.position.z < -40){
            Destroy(objectToDestroy);
        }
    }

    void SpawnEnemies(){
        GameObject enemyToSpawn = enemies[Random.Range(0,4)];
        if (enemyToSpawn.CompareTag("Enemy 1") || enemyToSpawn.CompareTag("Enemy 4")){
            enemyToSpawn.GetComponent<Enemy>().isMovingEnemy = true;
        }
        else{
            enemyToSpawn.GetComponent<Enemy>().isMovingEnemy = false;
        }
        posToSpawnOn = new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-21.3f,15.8f));
        Instantiate(enemyToSpawn,posToSpawnOn,enemyToSpawn.transform.rotation);
    }
}
