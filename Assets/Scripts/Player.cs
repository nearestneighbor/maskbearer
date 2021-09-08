using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    const float accelerationTimeAirborne = 0.2f;

    const float accelerationTimeGrounded = 0.1f;

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
    private PlayerInput _input;

    [SerializeField]
    new BoxCollider2D collider;

    Transform spriteGroup;

    Vector2 forceInputs;
    Timer forceInputsTimer = new Timer();

    private SpriteAnimator _anim;

    private void Awake()
    {
        _anim ??= transform.Find("Sprite").GetComponent<SpriteAnimator>();
        _input ??= GetComponent<PlayerInput>();

        _anim.AnimationFinished += OnAnimationFinish;

        InputSetup();
    }

    void Start()
    {
        controller = GetComponent<Controller>();
    }

    public Vector2 input;
    void Update() {
        //input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (forceInputsTimer) input = forceInputs;

        if (controller.collisions.below) {
            velocity.y = 0;
            varJumpTimer.Zero();
        }

        if (controller.collisions.below) {
            coyoteTimer.Start();
        }

        if (jumpGraceTimer && coyoteTimer) {
            jumpGraceTimer.Zero();
            velocity.y = jumpVelocity;
            varJumpTimer.Start();
            if (Mathf.Abs(input.x) > 0) {
                velocity.x += jumpHBoost * Mathf.Sign(input.x);
            }
        }

        // TODO: Use grace timers.
        if (jumpGraceTimer && (controller.collisions.right || controller.collisions.left)) {
            jumpGraceTimer.Zero();

            velocity.y = jumpVelocity;

            varJumpTimer.Start();
            float boostDir = controller.collisions.right ? -1 : 1;
            velocity.x = wallJumpHBoost * boostDir;
            forceInputs.x = boostDir;
            forceInputs.y = 0;
            forceInputsTimer.time = .25f;
            forceInputsTimer.Start();
        }

        if (varJumpTimer) {
            if (_jumping) {
                velocity.y = jumpVelocity;
                varJumpTimer.Update();
            }
            else {
                varJumpTimer.Zero();
            }
        }


        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position, collider.bounds.size, 0, Vector2.zero, 0, enemyMask);

        if (hit) {
            Vector2 recoil;
            if (transform.position.x < hit.transform.position.x) {
                recoil.x = -recoilSpeed;
            } else {
                recoil.x = recoilSpeed;
            }
            recoil.y = recoilSpeed;
            velocity += recoil;
        }

        forceInputsTimer.Update();
        jumpGraceTimer.Update();
        coyoteTimer.Update();

        float targetVelocityX = input.x * moveSpeed;
        float accelTime = controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);
        float maxFall = input.y == -1 ? extraMaxFall : defaultMaxFall;
        float downAccel = input.y == -1 ? extraFallAccelFactor * gravity : gravity;
        velocity.y = Mathf.Max(velocity.y + downAccel * Time.deltaTime, maxFall);
        controller.Move(velocity * Time.deltaTime);


        if (!controller.collisions.below) {
            spriteRenderer.flipX = velocity.x < 0;
            if (velocity.y > 0) {
                if (_anim.CurrentAnimation.animationName != "JumpMid") {
                    _anim.Play("JumpUp");
                }
            }
            else if (Mathf.Abs(velocity.y) < 0.1f) {
                _anim.Play("JumpMid");
            }
            else {
                _anim.Play("JumpDown");
            }
        }
        else {
            if (_anim.CurrentAnimation.animationName == "JumpDownLast"
                || _anim.CurrentAnimation.animationName == "JumpDown"
                || _anim.CurrentAnimation.animationName == "JumpMid") {
                _anim.Play("Idle");
            }
            if (Mathf.Abs(velocity.x) > 0.1f) {
                if (_anim.CurrentAnimation.animationName == "Idle" || _anim.CurrentAnimation.animationName == "RunStop") {
                    spriteRenderer.flipX = velocity.x < 0;
                    _anim.Play("RunStart");
                }
                else {
                    if (velocity.x < 0 && input.x > 0) _anim.Play("Turn"); ;
                    if (velocity.x > 0 && input.x < 0) _anim.Play("Turn"); ;
                }
            }
            else if (input.x == 0) {
                if (_anim.CurrentAnimation.animationName == "Run") {
                    _anim.Play("RunStop");
                }
                if (_anim.CurrentAnimation.animationName == "RunStart") {
                    _anim.Play("Idle");
                }
            }
        }
    }

    private void OnEnable()
    {
        _input.actions.ToList().ForEach(action => { Debug.Log(action.name); action.Enable(); });

        _attack.performed += OnAttack;
        _direction.performed += OnDirection;
        _jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _input.actions.ToList().ForEach(action => action.Disable());

        _attack.performed -= OnAttack;
        _direction.performed -= OnDirection;
        _jump.performed -= OnJump;
    }

    private void OnDestroy()
    {
        _anim.AnimationFinished -= OnAnimationFinish;
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        Debug.Log("Attack");
    }

    private void OnDirection(InputAction.CallbackContext ctx)
    {
        input = ctx.ReadValue<Vector2>();
    }

    private bool _jumping;
    private void OnJump(InputAction.CallbackContext ctx)
    {
        _jumping = ctx.ReadValueAsButton();
        if (ctx.ReadValueAsButton())
            jumpGraceTimer.Start();
    }

    enum Dir {
        Left, Right,
    }

    public SpriteRenderer spriteRenderer;

    private InputAction _attack;
    private InputAction _direction;
    private InputAction _jump;
    private void InputSetup() {
        _attack = _input.actions["Attack"];
        _direction = _input.actions["Direction"];
        _jump = _input.actions["Jump"];
    }

    private void OnAnimationFinish(string animName)
    {
        switch (animName)
        {
            case "RunStart":
                _anim.Play("Run");
                break;
            case "RunStop":
                _anim.Play("Idle");
                break;
            case "Turn":
                FlipInNextFrame(velocity.x < 0);
                _anim.Play("Run");
                break;
            case "JumpUp":
                _anim.Play("JumpMid");
                break;
            case "JumpDown":
                _anim.Play("JumpDownLast");
                break;
        }
    }

    bool? nextFlip = null;
    void FlipInNextFrame(bool flip) {
        nextFlip = flip;
    }
}
