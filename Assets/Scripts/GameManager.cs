using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float xBound = 40;
    public float zBound = 40;
    public GameObject[] enemies;
    public GameObject startScreen;
    public GameObject nextWaveScreen;
    public GameObject endScreen; 
    public TextMeshProUGUI waveText;
    // private Vector3 posToSpawnOn;
    private bool isGameActive;
    private bool isBetweenWaves;
    private int wave;
    private int numOfEnemies;

    // Start is called before the first frame update
    void Start()
    {
        isGameActive = false;
        isBetweenWaves = false;
        numOfEnemies = 3;
        wave = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(numOfEnemies<=0 && !isBetweenWaves && isGameActive){
            nextWaveScreen.SetActive(true);
            isBetweenWaves = true;
            wave++;
            StartCoroutine(WaitForNextWave());
        }
        
    }

    public void StartGame(){
        isGameActive = true;
        startScreen.SetActive(false);
        nextWaveScreen.SetActive(true);
        waveText.text = $"Wave: {wave}";
        StartCoroutine(WaitForNextWave());
    }

    public void EndGame(){
        isGameActive = false;
        DestroyAllEnemies("Cylinder");
        DestroyAllEnemies("Shield Cylinder");
        DestroyAllEnemies("Sphere");
        DestroyAllEnemies("Shield Sphere");
        DestroyAllEnemies("Homing Cone");
        DestroyAllEnemies("Enemy Laser");
        DestroyAllEnemies("Laser");
        endScreen.SetActive(true);
        CancelInvoke();
    }

    public void ResetGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void SetNumOfEnemies(int numOfEnemiesValue){
        numOfEnemies+=numOfEnemiesValue;
    }

    public int GetNumOfEnemies(){
        return numOfEnemies;
    }

    void SpawnWave(){
        if (wave<4){ //wave 1,2,3
            SpawnEnemies(0,2,3);
            numOfEnemies = 3;
        }
        else if(wave<7){ //wave 4,5,6
            SpawnEnemies(0,2,6);
            numOfEnemies = 6;
        }
        else if(wave<10){ //wave 7,8,9
            SpawnEnemies(2,5,3);
            numOfEnemies = 3;
        }
        else if(wave<13){ //wave 10,11,12
            SpawnEnemies(2,5,6);
            numOfEnemies = 6;
        }
        else if(wave<16){ //wave 13,14,15
            SpawnEnemies(0,5,3);
            numOfEnemies = 3;
        }
        else if(wave>=16){ //waves beyond
            SpawnEnemies(0,5,6);
            numOfEnemies = 6;
        }
        waveText.text = $"Wave: {wave}";
    }

    void SpawnEnemies(int lowestEnemyRange, int highestEnemyRange, int numOfEnemiesToSpawn){
        int numOfShieldSpheres = 0;
        for(int i=0;i<numOfEnemiesToSpawn;i++){
            GameObject enemyToSpawn = enemies[Random.Range(lowestEnemyRange,highestEnemyRange)];
            if (enemyToSpawn.CompareTag("Shield Sphere")){
                numOfShieldSpheres++;
                if (numOfShieldSpheres > 1){
                    enemyToSpawn = enemies[2];
                    Debug.Log("Converted");
                }
            }
            if (enemyToSpawn.CompareTag("Sphere") || enemyToSpawn.CompareTag("Shield Sphere")){
                enemyToSpawn.GetComponent<Enemy>().SetIsMovingEnemy(true);
            }
            Vector3 posToSpawnOn = new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-21.3f,15.8f));
            Instantiate(enemyToSpawn,posToSpawnOn,enemyToSpawn.transform.rotation);
        } 
    }

    void DestroyAllEnemies(string enemyType){
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyType);
        if (enemies.Length>0){
            foreach(GameObject enemy in enemies){
                Destroy(enemy);
            }
        }
    }

    IEnumerator WaitForNextWave(){
        yield return new WaitForSeconds(5);
        nextWaveScreen.SetActive(false);
        SpawnWave();
        isBetweenWaves = false;
    }
}
