using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float xBound = 40;
    public float zBound = 40;
    public GameObject[] enemies;
    public Vector3[] enemy1Positions = {
        //centre, 0
        new Vector3(0,-0.119999997f,0),
        //centre left, 1
        new Vector3(10.0900002f,-0.119999997f,0),
        //centre right, 2
        new Vector3(-10.0900002f,-0.119999997f,0),
        //centre bottom left, 3
        new Vector3(10.0900002f,-0.119999997f,-0.100000001f),
        //centre bottom right, 4
        new Vector3(-10.0100002f,-0.119999997f,-0.0700000003f),
        //centre top right, 5
        new Vector3(-10.0100002f,-0.119999997f,-10.2f),
        //centre top left, 6
        new Vector3(10.0900002f,-0.119999997f,-10.5200005f),
        //left, 7
        new Vector3(15.04f,-0.119999997f,0),
        //right, 8
        new Vector3(-9.80000019f,-0.119999997f,0),
        //top left, 9
        new Vector3(15.04f,-0.119999997f,-15.6000004f),
        //top right, 10
        new Vector3(-9.80000019f,-0.119999997f,-16.2099991f),
        //bottom right, 11
        new Vector3(-9.80000019f,-0.119999997f,12.1899996f),
        //bottom left, 12
        new Vector3(15.04f,-0.119999997f,12.2799997f)};
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
        int enemyToSpawn = Random.Range(0,3);
        int position = Random.Range(0,13);
        Instantiate(enemies[enemyToSpawn],enemy1Positions[position],enemies[enemyToSpawn].transform.rotation);
    }
}
