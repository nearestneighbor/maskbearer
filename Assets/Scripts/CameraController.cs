using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Player player;
    Vector3 cameraPosition;
    float cameraDefaultLerpSpeed = 3;
    Vector3 targetPosition;
    void Start(){
        cameraPosition = transform.position;
    }

    void Update(){
        float hspeed = cameraDefaultLerpSpeed;
        float vspeed = cameraDefaultLerpSpeed;
        targetPosition = player.transform.position;
        targetPosition.y += 1.5f;
        // targetPosition.x += 3 * (player.spriteRenderer.flipX?-1:1);
        
        if (cameraPosition.y > targetPosition.y){
            if (player.velocity.y == player.defaultMaxFall){
                vspeed *= 5;
            }
            else if (player.velocity.y < player.defaultMaxFall){
                vspeed *= 20;
            }
        }

        cameraPosition.x = Mathf.Lerp(cameraPosition.x, targetPosition.x, hspeed * Time.deltaTime);
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, targetPosition.y, vspeed * Time.deltaTime);
        transform.position = cameraPosition;
    }

    public float Approach(float from, float target, float amount)
        {
            if (from > target)
                return Mathf.Max(from - amount, target);
            else
                return Mathf.Min(from + amount, target);
        }
}


