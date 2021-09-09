using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteOrderManager : MonoBehaviour
{
    
    public float interval = 1f;
    float timer = 0;

    void Update(){
        timer += Time.deltaTime;
        if (timer > interval){
            timer -= interval;
        }

        var sprites = FindObjectsOfType<SpriteRenderer>();
        foreach (var sprite in sprites){
            sprite.sortingOrder = -Mathf.FloorToInt(sprite.transform.position.z * 100);
        }
    }
}
