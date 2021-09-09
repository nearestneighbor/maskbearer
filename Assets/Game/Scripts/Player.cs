using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    const float accelerationTimeAirborne = 0.1f;
       
    const float accelerationTimeGrounded = 0.05f;
    const float gravity = -50f;

    public LayerMask enemyMask;

    const float jumpVelocity = 10f;


    const float moveSpeed = 6f;
    const float jumpHBoost = 0.5f;
    const float wallJumpHBoost = 5f;

    const float defaultMaxFall = -15f;
    const float slowMaxFall = -5f;

    const float extraFallAccelFactor = 1.5f;

    const float recoilSpeed = 2.5f;

    float velocityXSmoothing;

    Timer jumpGraceTimer = new Timer(.05f);
    Timer wallJumpGraceTimer = new Timer(.05f);
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
    }

    void Start()
    {
        _anim ??= heldTransform.Find("Sprite").GetComponent<SpriteAnimator>();
        _input ??= GetComponent<PlayerInput>();

        _anim.AnimationFinished += OnAnimationFinish;

        spriteFlash = GetComponent<SpriteFlash>();
        
        controller = GetComponent<Controller>();
    }

    public Vector2 input;
    void Update() {
        //input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (forceInputsTimer) input = forceInputs;
        movementInput = controllerInput;
        if (forceInputsTimer) movementInput = forceInputs;

        if (controller.collisions.below || controller.collisions.above) {
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
            if (Mathf.Abs(movementInput.x) > 0) {
                velocity.x += jumpHBoost * Mathf.Sign(movementInput.x);
            }
        }

        if (controller.collisions.right || controller.collisions.left){
            wallJumpGraceTimer.Start();
            boostDir = controller.collisions.right ? -1 : 1;
        }

        if (jumpKeyPressed) {
            jumpGraceTimer.Start();
        }

        if (jumpGraceTimer && wallJumpGraceTimer) {
            jumpGraceTimer.Zero();

            velocity.y = jumpVelocity;

            varJumpTimer.Start();
            
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
            // velocity = recoil;
        }

        wallJumpGraceTimer.Update();
        forceInputsTimer.Update();
        jumpGraceTimer.Update();
        coyoteTimer.Update();

        velocity.x = movementInput.x * moveSpeed;
        //float accelTime = controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);
        bool clinging = (controller.collisions.right && movementInput.x == 1) || (controller.collisions.left && movementInput.x == -1);

        float maxFall = clinging ? slowMaxFall : defaultMaxFall;
        float downAccel = movementInput.y == -1 ? extraFallAccelFactor * gravity : gravity;
        velocity.y = Mathf.Max(velocity.y + downAccel * Time.deltaTime, maxFall);
        controller.Move(velocity * Time.deltaTime);


        if (!controller.collisions.below) {
            if (heldTransform.localScale.x < 0 && movementInput.x > 0 ||
            heldTransform.localScale.x > 0 && movementInput.x < 0)
                FlipScale();
            if (velocity.y >= 8) {
                _anim.Play("Jump");
            }
            else if (velocity.y < 8 && velocity.y >= 0){
                _anim.Play("JumpMid");
            }
            else if (velocity.y < 0 && _anim.CurrentAnimation.animationName != "JumpMid") {
                _anim.Play("Fall");
            }
        }
        else {
            if (heldTransform.localScale.x < 0 && movementInput.x > 0 ||
                    heldTransform.localScale.x > 0 && movementInput.x < 0){
                FlipScale();
                _anim.Stop();
                _anim.Play("Turn");
            }
            else if (Mathf.Abs(velocity.x) > 0.1f){
                if (_anim.CurrentAnimation.animationName != "Turn"){
                    _anim.Play("Run");
                }
            }
            else if (movementInput.x == 0 && _anim.CurrentAnimation.animationName == "Run")
                _anim.Play("RunStop");
            else if (_anim.CurrentAnimation.animationName != "RunStop" &&
                     _anim.CurrentAnimation.animationName != "Turn")
                _anim.Play("Idle");
        }
    }

    private void OnDestroy()
    {
        _anim.AnimationFinished -= OnAnimationFinish;
    }

    private void OnEnable()
    {
        _input.actions.ToList().ForEach(action => action.Enable());

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

    private void OnAnimationFinish(string animName)
    {
        switch (animName)
        {
            case "JumpMid":
                _anim.Play("Fall");
                break;
            case "RunStop":
                _anim.Play("Idle");
                break;
            case "Turn":
                _anim.Play(Mathf.Abs(velocity.x) > 0.1f ? "Run" : "RunStop");
                break;
        }
    }

    private void FlipScale() {
        var localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
