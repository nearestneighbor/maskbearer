using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    Camera mainCamera;
    public float parallaxAmount;
    Vector2 startPos;

    Vector3 position;
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        startPos.x = transform.position.x;
        startPos.y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float distX = (mainCamera.transform.position.x * parallaxAmount);
        float distY = (mainCamera.transform.position.y * parallaxAmount);
        position = transform.position;
        position.x = startPos.x + distX;
        position.y = startPos.y + distY;
        transform.position = position;
        
    }
}
