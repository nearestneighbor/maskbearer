using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Destructible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision){
        Debug.Log("Trigger Enter");
    }
    
}
