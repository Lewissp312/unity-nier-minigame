using System.Collections;
using TMPro;
using UnityEngine;
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
    [SerializeField] private TextMeshProUGUI causeOfFailureText;
    [SerializeField] private TextMeshProUGUI passedHighestWaveText;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    private PlayerController playerController;
    private Difficulties selectedDifficulty;
    private Coroutine countDownCoroutine;
    private Vector3 lastGoodMousePos;
    private bool isGameActive;
    private bool isBetweenWaves;
    private bool hasPassedHighestWave;
    private bool isSpiralWave;
    private int wave;
    private int numOfEnemies;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        selectedDifficulty = Difficulties.MEDIUM;
        wave = 1;
        isBetweenWaves = true;
        //Initialising high scores if the player doesn't have any
        if(!PlayerPrefs.HasKey("highestWaveEasy")){
            PlayerPrefs.SetInt("highestWaveEasy",1);
            PlayerPrefs.Save();
        }
        if(!PlayerPrefs.HasKey("highestWaveMedium")){
            PlayerPrefs.SetInt("highestWaveMedium",1);
            PlayerPrefs.Save();
        }
        if(!PlayerPrefs.HasKey("highestWaveHard")){
            PlayerPrefs.SetInt("highestWaveHard",1);
            PlayerPrefs.Save();
        }
        highestWaveEasyText.text = $"Easy: {PlayerPrefs.GetInt("highestWaveEasy")}";
        highestWaveMediumText.text = $"Medium: {PlayerPrefs.GetInt("highestWaveMedium")}";
        highestWaveHardText.text = $"Hard: {PlayerPrefs.GetInt("highestWaveHard")}";
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Unity methods

    void Update()
    {
        if(numOfEnemies <= 0 && !isBetweenWaves && isGameActive){
            DestroyAllClones("Enemy Laser");
            //The countdown coroutine needs to be a variable because it needs to be reset
            StopCoroutine(countDownCoroutine);
            nextWaveScreen.SetActive(true);
            isBetweenWaves = true;
            wave++;
            StartCoroutine(WaitForNextWave());
        }
        else if(startScreen.activeSelf){
            if(GetIsUsingController()){
                //Makes the controller button prompts appear
                startScreen.transform.GetChild(0).gameObject.SetActive(true);
                startScreen.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = 
                "Use the left stick to move. Move the right stick to change your firing direction. Hold R1/RB to fire.";
                //Square / X
                if(Input.GetKeyDown(KeyCode.JoystickButton0)){
                    SelectDifficulty(0);
                }
                //Triangle / Y
                else if(Input.GetKeyDown(KeyCode.JoystickButton3)){
                    SelectDifficulty(1);
                }
                //Circle / B
                else if(Input.GetKeyDown(KeyCode.JoystickButton2)){
                    SelectDifficulty(2);
                }
                //X / A
                else if(Input.GetKeyDown(KeyCode.JoystickButton1)){
                    StartGame();
                }
            }
            else{
                //Removes the controller button prompts
                startScreen.transform.GetChild(0).gameObject.SetActive(false);
                startScreen.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = 
                "Use WASD to move. Move the mouse to change your firing direction. Hold right or left click to fire.";
            }
        }
        else if(endScreen.activeSelf){
            if(GetIsUsingController()){
                //Makes the controller button prompts appear
                endScreen.transform.GetChild(0).gameObject.SetActive(true);
                // X / A
                if(Input.GetKeyDown(KeyCode.JoystickButton1)){
                    ResetGame();
                }
            }
            else{
                //Removes the controller button prompts
                endScreen.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//GameManager methods

    public void SelectDifficulty(int selectedOption){
        switch(selectedOption){
            case 0:
                SetActiveButtonColour(easyButton.gameObject);
                selectedDifficulty = Difficulties.EASY;
                SetInactiveButtonColour(mediumButton.gameObject);
                SetInactiveButtonColour(hardButton.gameObject);
                break;
            case 1:
                SetActiveButtonColour(mediumButton.gameObject);
                selectedDifficulty = Difficulties.MEDIUM;
                SetInactiveButtonColour(easyButton.gameObject);
                SetInactiveButtonColour(hardButton.gameObject);
                break;
            case 2:
                SetActiveButtonColour(hardButton.gameObject);
                selectedDifficulty = Difficulties.HARD;
                SetInactiveButtonColour(easyButton.gameObject);
                SetInactiveButtonColour(mediumButton.gameObject);
                break;
        }
    }

    public void StartGame(){
        if(selectedDifficulty == Difficulties.HARD){
            //Top left
            Instantiate(enemyBox,new Vector3(11.1999998f,0.0199999996f,-10.3000002f),enemyBox.transform.rotation);
            //Top right
            Instantiate(enemyBox,new Vector3(-8.19999981f,0.0199999996f,-10.3000002f),enemyBox.transform.rotation);
            //Bottom left
            Instantiate(enemyBox,new Vector3(11.3000002f,0.0199999996f,7.4000001f),enemyBox.transform.rotation);
            //Bottom right
            Instantiate(enemyBox,new Vector3(-8.19999981f,0.0199999996f,7.4000001f),enemyBox.transform.rotation);
            //Centre
            Instantiate(enemyBox,new Vector3(1.39999998f,0.0199999996f,-1f),enemyBox.transform.rotation);
        }
        isGameActive = true;
        startScreen.SetActive(false);
        nextWaveScreen.SetActive(true);
        waveText.text = $"Wave: {wave}";
        StartCoroutine(WaitForNextWave());
    }

    public void EndGame(string causeOfFailure){
        isGameActive = false;
        DestroyAllClones("Cylinder");
        DestroyAllClones("Shield Cylinder");
        DestroyAllClones("Sphere");
        DestroyAllClones("Shield Sphere");
        DestroyAllClones("Homing Cone");
        DestroyAllClones("Spiral Sphere");
        DestroyAllClones("Enemy Laser");
        DestroyAllClones("Enemy Box");
        DestroyAllClones("Laser");
        StopAllCoroutines();
        CancelInvoke();
        nextWaveScreen.SetActive(false);
        endScreen.SetActive(true);
        causeOfFailureText.text = $"Cause of Failure: {causeOfFailure}";
        if(hasPassedHighestWave){
            passedHighestWaveText.text = $"Highest Wave Passed! Highest Wave ({selectedDifficulty.ToString().ToLower()}): {wave}";
        }
    }

    public void ResetGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MovementRestrictions(GameObject objectToDestroy){
        if(objectToDestroy.transform.position.x < -60 || objectToDestroy.transform.position.x > 60 || objectToDestroy.transform.position.z > 60 || objectToDestroy.transform.position.z < -60){
            Destroy(objectToDestroy);
        }
    }

    public void DecreaseNumOfEnemies(){
        numOfEnemies--;
    }

    public void PlayHitEffect(ParticleSystem damageEffect, Vector3 position){
        ParticleSystem damageEffectCopy = Instantiate(damageEffect,position,transform.rotation);
        damageEffectCopy.Play();
        Destroy(damageEffectCopy.gameObject,damageEffectCopy.main.duration);
    }


    void SpawnWave(){
        switch(selectedDifficulty){
            case Difficulties.EASY:
                switch(wave){
                    case < 4: //wave 1,2,3
                        SpawnEnemies(0,2,3);
                        numOfEnemies = 3;
                        break;
                    case < 7: //wave 7,8,9
                        SpawnEnemies(2,5,3);
                        numOfEnemies = 3;
                        break;
                    case >= 7: //wave 13,14,15
                        SpawnEnemies(0,5,3);
                        numOfEnemies = 3;
                        break;
                }
                CheckHighestWave("highestWaveEasy",highestWaveEasyText,"Easy");
                break;
            case Difficulties.MEDIUM or Difficulties.HARD:
                if(wave % 5 == 0){
                    isSpiralWave = true;
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
                    isSpiralWave = false;
                    switch (wave){
                        case < 4: //wave 1,2,3
                            SpawnEnemies(0,2,3);
                            numOfEnemies = 3;
                            break;
                        case < 7: //wave 4,5,6
                            SpawnEnemies(0,2,6);
                            numOfEnemies = 6;
                            break;
                        case < 10: //wave 7,8,9
                            SpawnEnemies(2,5,3);
                            numOfEnemies = 3;
                            break;
                        case < 13: //wave 10,11,12
                            SpawnEnemies(2,5,6);
                            numOfEnemies = 6;
                            break;
                        case < 16: //wave 13,14,15
                            SpawnEnemies(0,5,3);
                            numOfEnemies = 3;
                            break;
                        case >= 16: //waves beyond
                            SpawnEnemies(0,5,6);
                            numOfEnemies = 6;
                            break;
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
        if(wave > PlayerPrefs.GetInt(waveToExamine)){
            PlayerPrefs.SetInt(waveToExamine,wave);
            PlayerPrefs.Save();
            textToChange.text = $"{textForWave}: {wave}";
            hasPassedHighestWave = true;
        }
    }

    void SpawnEnemies(int lowestEnemyRange, int highestEnemyRange, int numOfEnemiesToSpawn){
        int numOfShieldSpheres = 0;
        for(int i = 0;i < numOfEnemiesToSpawn;i++){
            int randEnemyIndex = Random.Range(lowestEnemyRange,highestEnemyRange);
            GameObject enemyToSpawn = enemies[randEnemyIndex];
            if(enemyToSpawn.CompareTag("Shield Sphere")){
                numOfShieldSpheres++;
                if(numOfShieldSpheres > 1){
                    //Shield spheres will release their shields when all other enemies have been
                    //defeated, so there should only be one in any wave
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
                if(hitColliders.Length > 0){
                    if(hitColliders[0].CompareTag("Ground")){
                        //The enemy will almost always collide with the ground, so when this is the only collider,
                        //the spawn position is fine
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
    void DestroyAllClones(string enemyType){
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyType);
        if(enemies.Length > 0){
            foreach(GameObject enemy in enemies){
                Destroy(enemy);
            }
        }
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Get methods

    public Vector3 GetMouseOnBoardPosition(out bool isOverPlayer){
        Ray ray;
        ray =  Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 playerPos = playerController.GetPlayerPosition();
        //If the ray hits a collider
        if(Physics.Raycast(ray,out RaycastHit hitData)){
            bool isFarEnough = Vector3.Distance(hitData.point, playerPos) > 4f;
            if (!hitData.collider.gameObject.CompareTag("Player") && isFarEnough){
                lastGoodMousePos = new(hitData.point.x,hitData.point.y + 0.5f, hitData.point.z);
                isOverPlayer = false;
                return lastGoodMousePos;
            }
            else{
                isOverPlayer = true;
                return lastGoodMousePos;
            }
        } 
        else{
            isOverPlayer = true;
            return lastGoodMousePos;
        }
    }

    public bool GetIsGameActive(){
        return isGameActive;
    }

    public bool GetIsSpiralWave(){
        return isSpiralWave;
    }

    public bool GetIsUsingController(){
        return Input.GetJoystickNames().Length > 0;
    }

    public int GetNumOfEnemies(){
        return numOfEnemies;
    }


////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Set methods

    void SetActiveButtonColour(GameObject button){
        //Button background set to dark cream-ish colour
        button.GetComponent<Image>().color = new Color32(127,122,103,255);
        //Text set to white
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(255,255,255,255);
    }

    void SetInactiveButtonColour(GameObject button){
        //Button background set to white
        button.GetComponent<Image>().color = new Color32(255,255,255,255);
        //Text set to black
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(0,0,0,255);
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//IEnumerators


    IEnumerator WaitForNextWave(){
        yield return new WaitForSeconds(5);
        nextWaveScreen.SetActive(false);
        SpawnWave();
        isBetweenWaves = false;
    }

    IEnumerator CountDown(){
        int timeLeft = 30;
        timerText.text = $"Time: {timeLeft}";
        for(int i = 0;i < 30;i++){
            yield return new WaitForSeconds(1);
            timeLeft--;
            timerText.text = $"Time: {timeLeft}";
        }
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        EndGame("Out of Time");
    }
}
