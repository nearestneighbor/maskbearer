using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    
    [SerializeField]
    float rotationSpeed;
    Vector3 eulerAngles;
    void Start(){
        eulerAngles = transform.eulerAngles;
    }

    void Update(){
        eulerAngles.z += rotationSpeed * Time.deltaTime;
        transform.eulerAngles = eulerAngles;
    }

}
