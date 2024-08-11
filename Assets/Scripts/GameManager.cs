using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float xBound = 40;
    public float zBound = 40;
    public GameObject[] enemies;
    public GameObject enemyBox;
    public GameObject startScreen;
    public GameObject nextWaveScreen;
    public GameObject endScreen; 
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI highestWaveEasyText;
    public TextMeshProUGUI highestWaveMediumText;
    public TextMeshProUGUI highestWaveHardText;
    public TextMeshProUGUI passedHighestWaveText;
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    private Coroutine countDownCoroutine;
    private enum Difficulties {EASY,MEDIUM,HARD};
    private Difficulties selectedDifficulty;
    private bool isGameActive;
    private bool isBetweenWaves;
    private bool hasPassedHighestWave;
    private int wave;
    private int numOfEnemies;

    // Start is called before the first frame update
    void Start()
    {
        isGameActive = false;
        isBetweenWaves = false;
        hasPassedHighestWave = false;
        selectedDifficulty = Difficulties.MEDIUM;
        numOfEnemies = 3;
        wave = 1;
        if (!PlayerPrefs.HasKey("highestWaveEasy")){
            PlayerPrefs.SetInt("highestWaveEasy",1);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("highestWaveMedium")){
            PlayerPrefs.SetInt("highestWaveMedium",1);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("highestWaveHard")){
            PlayerPrefs.SetInt("highestWaveHard",1);
            PlayerPrefs.Save();
        }
        highestWaveEasyText.text = $"Easy: {PlayerPrefs.GetInt("highestWaveEasy")}";
        highestWaveMediumText.text = $"Medium: {PlayerPrefs.GetInt("highestWaveMedium")}";
        highestWaveHardText.text = $"Hard: {PlayerPrefs.GetInt("highestWaveHard")}";
    }

    // Update is called once per frame
    void Update()
    {
        if(numOfEnemies<=0 && !isBetweenWaves && isGameActive){
            //needs to be a variable because it needs to be reset
            StopCoroutine(countDownCoroutine);
            nextWaveScreen.SetActive(true);
            isBetweenWaves = true;
            wave++;
            StartCoroutine(WaitForNextWave());
        }
    }

    public void SelectDifficulty(){
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        selectedButton.GetComponent<Image>().color = new Color32(127,122,103,255);
        selectedButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(255,255,255,255);
        if (selectedButton.CompareTag("Easy")){
            selectedDifficulty = Difficulties.EASY;
            SetInactiveButtonColour(mediumButton);
            SetInactiveButtonColour(hardButton);
        }
        else if (selectedButton.CompareTag("Medium")){
            selectedDifficulty = Difficulties.MEDIUM;
            SetInactiveButtonColour(easyButton);
            SetInactiveButtonColour(hardButton);
        }
        else{
            selectedDifficulty = Difficulties.HARD;
            SetInactiveButtonColour(easyButton);
            SetInactiveButtonColour(mediumButton);
        }
    }

    public void StartGame(){
// //centre bottom left, 3
//         new Vector3(10.0900002f,-0.119999997f,-0.100000001f),
//         //centre bottom right, 4
//         new Vector3(-10.0100002f,-0.119999997f,-0.0700000003f),
//         //centre top right, 5
//         new Vector3(-10.0100002f,-0.119999997f,-10.2f),
//         //centre top left, 6
//         new Vector3(10.0900002f,-0.119999997f,-10.5200005f),
        if (selectedDifficulty == Difficulties.HARD){
            // //top left
            // new Vector3(11.1999998,0.0199999996,-10.3000002);
            // //top right
            // Vector3(-8.19999981,0.0199999996,-10.3000002);
            // //bottom right
            // Vector3(-8.19999981,0.0199999996,7.4000001);
            // //bottom left
            // Vector3(11.3000002,0.0199999996,7.4000001);
            // //centre
            // Vector3(1.39999998,0.0199999996,-1)
            Instantiate(enemyBox,new Vector3(11.1999998f,0.0199999996f,-10.3000002f),enemyBox.transform.rotation);
            Instantiate(enemyBox,new Vector3(-8.19999981f,0.0199999996f,-10.3000002f),enemyBox.transform.rotation);
            Instantiate(enemyBox,new Vector3(-8.19999981f,0.0199999996f,7.4000001f),enemyBox.transform.rotation);
            Instantiate(enemyBox,new Vector3(11.3000002f,0.0199999996f,7.4000001f),enemyBox.transform.rotation);
            Instantiate(enemyBox,new Vector3(1.39999998f,0.0199999996f,-1f),enemyBox.transform.rotation);
        }
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
        StopAllCoroutines();
        // StopCoroutine(countDownCoroutine);
        CancelInvoke();
        nextWaveScreen.SetActive(false);
        endScreen.SetActive(true);
        if (hasPassedHighestWave){
            passedHighestWaveText.text = $"Highest Wave Passed! Highest Wave ({selectedDifficulty.ToString().ToLower()}): {wave}";
        }
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

    void SetInactiveButtonColour(Button button){
        button.GetComponent<Image>().color = new Color32(255,255,255,255);
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(0,0,0,255);
    }

    void SpawnWave(){
        switch(selectedDifficulty){
            case Difficulties.EASY:
                if (wave<4){ //wave 1,2,3
                    SpawnEnemies(0,2,3);
                    numOfEnemies = 3;
                }
                else if(wave<7){ //wave 7,8,9
                    SpawnEnemies(2,5,3);
                    numOfEnemies = 3;
                }
                else if(wave>=7){ //wave 13,14,15
                    SpawnEnemies(0,5,3);
                    numOfEnemies = 3;
                }
                CheckHighestWave("highestWaveEasy",highestWaveEasyText,"Easy");
                break;
            case Difficulties.MEDIUM or Difficulties.HARD:
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
                switch(selectedDifficulty){
                    case Difficulties.MEDIUM:
                        CheckHighestWave("highestWaveMedium",highestWaveMediumText,"Medium");
                        break;
                    case Difficulties.HARD:
                        CheckHighestWave("highestWaveHard",highestWaveHardText,"Hard");
                        break;
                }
                break;
        }
        waveText.text = $"Wave: {wave}";
        countDownCoroutine = StartCoroutine(CountDown());
    }

    void CheckHighestWave(string waveToExamine, TextMeshProUGUI textToChange, string textForWave){
        if (wave > PlayerPrefs.GetInt(waveToExamine)){
            PlayerPrefs.SetInt(waveToExamine,wave);
            PlayerPrefs.Save();
            textToChange.text = $"{textForWave}: {wave}";
            hasPassedHighestWave = true;
        }
    }

    void SpawnEnemies(int lowestEnemyRange, int highestEnemyRange, int numOfEnemiesToSpawn){
        int numOfShieldSpheres = 0;
        for(int i=0;i<numOfEnemiesToSpawn;i++){
            GameObject enemyToSpawn = enemies[Random.Range(lowestEnemyRange,highestEnemyRange)];
            if (enemyToSpawn.CompareTag("Shield Sphere")){
                numOfShieldSpheres++;
                if (numOfShieldSpheres > 1){
                    enemyToSpawn = enemies[2];
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

    IEnumerator CountDown(){
        int timeLeft = 30;
        timerText.text = $"Time: {timeLeft}";
        for (int i=0;i<30;i++){
            yield return new WaitForSeconds(1);
            timeLeft--;
            timerText.text = $"Time: {timeLeft}";
        }
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        EndGame();
    }
}
