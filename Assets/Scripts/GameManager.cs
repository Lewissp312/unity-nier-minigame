using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float xBound = 40;
    public float zBound = 40;
    public GameObject[] enemies;
    public GameObject startScreen;
    public GameObject endScreen; 
    private Vector3 posToSpawnOn;
    private bool isGameActive;
    private int numOfEnemies;

    // Start is called before the first frame update
    void Start()
    {
        isGameActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(){
        isGameActive = true;
        startScreen.SetActive(false);
        InvokeRepeating(nameof(SpawnEnemies),1,10);
    }

    public void EndGame(){
        isGameActive = false;
        DestroyAllEnemies("Enemy 1");
        DestroyAllEnemies("Enemy 2");
        DestroyAllEnemies("Enemy 3");
        DestroyAllEnemies("Enemy 4");
        endScreen.SetActive(true);
        CancelInvoke();
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

    public bool GetIsGameActive(){
        return isGameActive;
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

    void DestroyAllEnemies(System.String enemyType){
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyType);
        if (enemies.Length>0){
            foreach(GameObject enemy in enemies){
                Destroy(enemy);
            }
        }
    }
}
