using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public ParticleSystem destroyEffect;
    private Vector3 fireDirection;
    private float speed = 25;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        fireDirection = (gameManager.GetMouseOnBoardPosition() - transform.position).normalized * speed;
        fireDirection.y = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += fireDirection * Time.deltaTime;
        gameManager.MovementRestrictions(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy Box") || other.gameObject.CompareTag("Shield")){
            ParticleSystem destroyEffectCopy = Instantiate(destroyEffect,transform.position,transform.rotation);
            destroyEffectCopy.Play();
            Destroy(destroyEffectCopy.gameObject,destroyEffectCopy.main.duration);
            Destroy(gameObject);
        }
    }
}
