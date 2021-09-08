using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    Direction startDir = Direction.Left;

    const float gravity = -900f;
    const float moveSpeed = 30f;
    
    Controller controller;

    [SerializeField]
    Vector2 velocity;
    void Start(){
        controller = GetComponent<Controller>();
        if(startDir == Direction.Left)
            velocity.x = -moveSpeed;
        if(startDir == Direction.Right)
            velocity.x = moveSpeed;
    }

    void Update(){
        if (!controller.collisions.below){
            velocity.y += gravity * Time.deltaTime;
        }
        else{
            velocity.y = 0;
        }

        if (controller.collisions.leftEdge || controller.collisions.left){
            velocity.x = moveSpeed;
        }

        if (controller.collisions.rightEdge || controller.collisions.right){
            velocity.x = - moveSpeed;
        }


        controller.Move(velocity * Time.deltaTime);
    }
}

public enum Direction {
        None,
        Left,
        Right,
    };
