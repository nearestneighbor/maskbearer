using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Reactible : MonoBehaviour
{

    Timer timer = new Timer();

    Vector3 eulerAngles;
    

    [SerializeField]
    float waveAmplitude;


    [SerializeField]
    float waveTime;

    float direction = 0;

    void Start(){
        eulerAngles = transform.eulerAngles;
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if (!timer){
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player")){
                direction = Mathf.Sign(collision.gameObject.transform.position.x - transform.position.x);
                timer.time = waveTime;
                timer.Start();
            }
        }
    }

    void Update(){
        if (timer){
           eulerAngles.z = -direction * waveAmplitude * Mathf.Sin(Mathf.PI * Easing.Elastic.Out(timer.progress));
           transform.eulerAngles = eulerAngles;
        }
        timer.Update();
    }
    
}
