// using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Difficulties {EASY,MEDIUM,HARD};
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject enemyBox;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject nextWaveScreen;
    [SerializeField] private GameObject endScreen; 
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI highestWaveEasyText;
    [SerializeField] private TextMeshProUGUI highestWaveMediumText;
    [SerializeField] private TextMeshProUGUI highestWaveHardText;
    [SerializeField] private TextMeshProUGUI passedHighestWaveText;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    private Difficulties selectedDifficulty;
    private Coroutine countDownCoroutine;
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
            DestroyAllEnemies("Enemy Laser");
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
        if (selectedDifficulty == Difficulties.HARD){
            //top left
            Instantiate(enemyBox,new Vector3(11.1999998f,0.0199999996f,-10.3000002f),enemyBox.transform.rotation);
            //top right
            Instantiate(enemyBox,new Vector3(-8.19999981f,0.0199999996f,-10.3000002f),enemyBox.transform.rotation);
            //bottom left
            Instantiate(enemyBox,new Vector3(11.3000002f,0.0199999996f,7.4000001f),enemyBox.transform.rotation);
            //bottom right
            Instantiate(enemyBox,new Vector3(-8.19999981f,0.0199999996f,7.4000001f),enemyBox.transform.rotation);
            //centre
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
        DestroyAllEnemies("Spiral Sphere");
        DestroyAllEnemies("Enemy Laser");
        DestroyAllEnemies("Laser");
        StopAllCoroutines();
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

    public void MovementRestrictions(GameObject objectToDestroy){
        if (objectToDestroy.transform.position.x < -40 || objectToDestroy.transform.position.x > 40 || objectToDestroy.transform.position.z > 40 || objectToDestroy.transform.position.z < -40){
            Destroy(objectToDestroy);
        }
    }

    public void DecreaseNumOfEnemies(){
        numOfEnemies--;
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
                if (wave%5==0){
                    GameObject spiralEnemy = enemies[5];
                    Instantiate(spiralEnemy,new Vector3(1.29999995f,-0.119999997f,-21f),enemies[5].transform.rotation);
                    GameObject cylinder1 = enemies[0];
                    Instantiate(cylinder1,new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-20,15.8f)),cylinder1.transform.rotation);
                    GameObject cylinder2 = enemies[0];
                    Instantiate(cylinder2,new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-20,15.8f)),cylinder2.transform.rotation);
                    GameObject cylinder3 = enemies[0];
                    Instantiate(cylinder3,new Vector3(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-20,15.8f)),cylinder3.transform.rotation);
                    numOfEnemies = 4;
                }
                else{
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
            bool isGoodSpawn = false;
            Vector3 posToSpawnOn;
            do{
                posToSpawnOn = new(Random.Range(-16.5f,20.6f),-0.119999997f,Random.Range(-21.3f,15.8f));
                Vector3 boxSize;
                try{
                    boxSize = enemyToSpawn.GetComponent<BoxCollider>().size;
                } catch (System.Exception){
                    enemyToSpawn.AddComponent<BoxCollider>();
                    boxSize = enemyToSpawn.GetComponent<BoxCollider>().size;
                    enemyToSpawn.GetComponent<BoxCollider>().enabled = false;
                }
                Collider[] hitColliders = Physics.OverlapBox(posToSpawnOn, boxSize / 2, enemyToSpawn.transform.rotation);
                if (hitColliders.Length > 0){
                    if (hitColliders[0].CompareTag("Ground")){
                        isGoodSpawn = true;
                    }
                }
                else{
                    isGoodSpawn = true;
                }
            } while(!isGoodSpawn);
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

    public Difficulties GetDifficulty(){
        return selectedDifficulty;
    }

    public bool GetIsGameActive(){
        return isGameActive;
    }

    public int GetNumOfEnemies(){
        return numOfEnemies;
    }

    public int GetWave(){
        return wave;
    }

    void SetInactiveButtonColour(Button button){
        button.GetComponent<Image>().color = new Color32(255,255,255,255);
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(0,0,0,255);
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
