using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public class Player : MonoBehaviour
{
    const float accelerationTimeAirborne = 0.1f;
       
    const float accelerationTimeGrounded = 0.05f;
    
    const float gravity = -50f;

    public LayerMask enemyMask;

    const float jumpVelocity = 9f;


    const float moveSpeed = 6f;
    const float jumpHBoost = 0.5f;
    const float wallJumpHBoost = 5f;

    public float defaultMaxFall = -15f;
    public float extraMaxFall = -30f;

    const float extraFallAccelFactor = 1.5f;

    const float recoilSpeed = 2.5f;

    float velocityXSmoothing;

    Timer jumpGraceTimer = new Timer(.05f);
    Timer varJumpTimer = new Timer(.2f);

    Timer coyoteTimer = new Timer(.05f);

    public Vector2 velocity;
    
    Controller controller;

    [SerializeField]
    new BoxCollider2D collider;

    Transform spriteGroup;

    Vector2 forceInputs;
    Timer forceInputsTimer = new Timer();
    
    void Start()
    {
        controller = GetComponent<Controller>();
        AnimationSetup();
    }
    
    public Vector2 input;
    void Update(){
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Debug.Log($"Input: {input.x}, {input.y}");
        if (forceInputsTimer) input = forceInputs;

        if (controller.collisions.below || controller.collisions.above) {
            velocity.y = 0;
            varJumpTimer.Zero();
        }
        
        if (Input.GetKeyDown(KeyCode.C)){
            jumpGraceTimer.Start();
        }

        if (controller.collisions.below){
            coyoteTimer.Start();
        }

        if (jumpGraceTimer && coyoteTimer){
            jumpGraceTimer.Zero();
            velocity.y = jumpVelocity;
            varJumpTimer.Start();
            if (Mathf.Abs(input.x) > 0){
                velocity.x += jumpHBoost * Mathf.Sign(input.x);
            }
        }

        // TODO: Use grace timers.
        if (jumpGraceTimer && (controller.collisions.right || controller.collisions.left)){
            jumpGraceTimer.Zero();
            
            velocity.y = jumpVelocity;
            
            varJumpTimer.Start();
            float boostDir = controller.collisions.right?-1:1;
            velocity.x = wallJumpHBoost * boostDir;
            forceInputs.x = boostDir;
            forceInputs.y = 0;
            forceInputsTimer.time = .25f;
            forceInputsTimer.Start();
        }

        if (varJumpTimer){
            if (Input.GetKey(KeyCode.C)){
                velocity.y = jumpVelocity;
                varJumpTimer.Update();
            }
            else{
                varJumpTimer.Zero();
            }
        }

        
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position, collider.bounds.size,  0,  Vector2.zero, 0, enemyMask);

        if (hit){
            Vector2 recoil;
            if (transform.position.x < hit.transform.position.x){
                recoil.x = - recoilSpeed;
            } else{
                recoil.x = recoilSpeed;
            }
            recoil.y = recoilSpeed;
            velocity += recoil;
        }

        forceInputsTimer.Update();
        jumpGraceTimer.Update();
        coyoteTimer.Update();

        float targetVelocityX = input.x * moveSpeed;
        float accelTime = controller.collisions.below?accelerationTimeGrounded:accelerationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);
        float maxFall = input.y == -1? extraMaxFall:defaultMaxFall;
        float downAccel = input.y == -1? extraFallAccelFactor * gravity: gravity;
        velocity.y = Mathf.Max(velocity.y + downAccel * Time.deltaTime, maxFall);
        controller.Move(velocity * Time.deltaTime);

        
        if (!controller.collisions.below){
            spriteRenderer.flipX = velocity.x < 0;
            if (velocity.y > 0){
                if (currentState != Anim.JumpMid){
                    Play(Anim.JumpUp);
                }
            }
            else if (Mathf.Abs(velocity.y) < 0.1f){
                Play(Anim.JumpMid);
            }
            else{
                Play(Anim.JumpDown);
            }
        }
        else{
            if (currentState == Anim.JumpDownLast
                || currentState == Anim.JumpDown){
                    Play(Anim.JumpMid);
            }
            if (Mathf.Abs(velocity.x) > 0.1f){
                if (currentState == Anim.Idle || currentState == Anim.RunStop){
                    spriteRenderer.flipX = velocity.x < 0;
                    Play(Anim.RunStart);
                }
                else{
                    if (velocity.x < 0 && input.x > 0) Play(Anim.Turn);
                    if (velocity.x > 0 && input.x < 0) Play(Anim.Turn);
                }
            } 
            else if (input.x == 0){
                if (currentState == Anim.Run){
                    Play(Anim.RunStop);
                }
                if (currentState == Anim.RunStart){
                    Play(Anim.Idle);
                }
            }
        }

        AnimationUpdate();
    }

    enum Dir{
        Left, Right,
    }

    enum Anim {
        Run,
        RunStart,
        RunStop,
        Idle,
        IdleUp,
        IdleDown,
        Turn,
        JumpUp,
        JumpMid,
        JumpDown,
        JumpDownLast,
    }

    public List<AnimationData> animationData;
    private Dictionary<string, AnimationData> animationDataTable;
    
    private Anim currentState;
    private AnimationData currentAnim;
    private int frame = 0;
    private float subFrame = 0;
    private int frameCount = -1;
    public SpriteRenderer spriteRenderer;
    

    void AnimationSetup(){
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animationDataTable = new Dictionary<string, AnimationData>();

        foreach(var data in animationData){
            print($"Adding animation: {data.name}");
            animationDataTable[data.name] = data;
        }
        subFrame = 0;
    }

    float default_seconds_per_frame = 1f/12;
    void AnimationUpdate(){
        float seconds_per_frame = default_seconds_per_frame;
        if (currentState == Anim.Turn){
            seconds_per_frame = 1f/18;
        }
        subFrame += Time.deltaTime;
        if (subFrame > seconds_per_frame){
            spriteRenderer.sprite = currentAnim.sprites[frame];
            if (nextFlip.HasValue) 
            {
                spriteRenderer.flipX = nextFlip.Value;
                nextFlip = null;
            }
            frame = frame + 1;
            if (frame == frameCount){
                OnLastFrame();
                frame = 0;
            }
            subFrame -= seconds_per_frame;
        }
    }

    void OnLastFrame(){
        if (currentState == Anim.RunStart) Play(Anim.Run);
        if (currentState == Anim.RunStop) Play(Anim.Idle);
        if (currentState == Anim.Turn){
            FlipInNextFrame(velocity.x < 0);
            Play(Anim.Run);
        }
        if (currentState == Anim.JumpUp) Play(Anim.JumpMid);
        if (currentState == Anim.JumpDown) Play(Anim.JumpDownLast);
        if (currentState == Anim.JumpMid && controller.collisions.below) Play(Anim.Idle);
    }


    bool? nextFlip = null;
    void FlipInNextFrame(bool flip){
        nextFlip = flip;
    }

    void Play(Anim state){
        if (currentState != state){
            
            currentState = state;

            var name = Enum.GetName(typeof(Anim), state);
            print($"Switing to {name}");
            var newAnim = animationDataTable[name];
        
            currentAnim = newAnim;
            frame = 0;
            frameCount = currentAnim.sprites.Count;
        }
    }
    
}
