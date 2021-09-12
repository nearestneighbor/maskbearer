using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//i think it would be useful for dialogs, signs, beches etc
//dont work on this moment
abstract public class InteractableObject : MonoBehaviour
{
    Animator anim;
    bool isPlayerTouching;
    Collider2D player;
    PlayerInputActions inputs;
    public abstract string Name_t { get; }
    private void Awake()
    {
        inputs = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>()._actions;
        inputs.Player.Direction.started += bruh => TryInteract(bruh.ReadValue<Vector2>());
        Transform.FindObjectOfType<Text>().GetComponent<Text>().text = Name_t;
        anim = Transform.FindObjectOfType<Canvas>().GetComponent<Animator>();
        anim.SetBool("isActive", false);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == GameObject.FindGameObjectWithTag("PlayerCollider")) 
        {
            player = collision;
            isPlayerTouching = true;
            anim.SetBool("isActive", true);
            OnEnter();
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == GameObject.FindGameObjectWithTag("PlayerCollider"))
        {
            isPlayerTouching = false;
            anim.SetBool("isActive", false);
            OnExit();
        }
    }
    void TryInteract(Vector2 inpoot)
    {
        if(isPlayerTouching==true)
        {
            if (inpoot.y > 0.8 && Mathf.Abs(inpoot.x) < 0.3)
            {
                OnActivate();
            }
        }
    }
    abstract public void OnEnter();
    abstract public void OnExit();
    abstract public void OnActivate();
}
