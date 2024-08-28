using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private ParticleSystem destroyEffect;
    private Vector3 fireDirection;
    private readonly float speed = 35;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (!gameManager.GetIsUsingController()){
            fireDirection = (gameManager.GetMouseOnBoardPosition(out bool isOverPlayer) - transform.position).normalized * speed;
            fireDirection.y = 0.3f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetIsUsingController()){
            transform.Translate(speed * Time.deltaTime * -transform.forward);
        }
        else{
            transform.position += fireDirection * Time.deltaTime;
        }
        gameManager.MovementRestrictions(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy Box")){
            PlayEffectAndDestroy();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shield")){
            PlayEffectAndDestroy();
        }
    }

    void PlayEffectAndDestroy(){
        ParticleSystem destroyEffectCopy = Instantiate(destroyEffect,transform.position,transform.rotation);
        destroyEffectCopy.Play();
        Destroy(destroyEffectCopy.gameObject,destroyEffectCopy.main.duration);
        Destroy(gameObject);
    }

}

//Vector3(2.15816331,2.15816331,2.15816331)
