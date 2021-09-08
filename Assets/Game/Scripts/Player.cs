using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller))]
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

    [SerializeField]
    new BoxCollider2D collider;

    Vector2 forceInputs;
    Timer forceInputsTimer = new Timer();

    private SpriteAnimator _anim;
    private PlayerInputActions _actions;

    private Vector2 movementInput;
    private float boostDir = 1;

    SpriteFlash spriteFlash;

    private void Awake()
    {
        _anim ??= transform.Find("Held/Sprite").GetComponent<SpriteAnimator>();

        _anim.AnimationFinished += OnAnimationFinish;

        _actions ??= new PlayerInputActions();

        spriteFlash = GetComponent<SpriteFlash>();

        controller = GetComponent<Controller>();
    }

    void Update() {
        movementInput = _actions.Player.Direction.ReadValue<Vector2>();
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

        if (_justJumped) {
            _justJumped = false;
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
        _actions.ToList().ForEach(action => action.Enable());

        _actions.Player.Attack.performed += OnAttack;
        _actions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _actions.ToList().ForEach(action => action.Disable());

        _actions.Player.Attack.performed -= OnAttack;
        _actions.Player.Jump.performed -= OnJump;
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        Debug.Log("Attack");
    }

    private bool _jumping;
    private bool _justJumped;
    private void OnJump(InputAction.CallbackContext ctx)
    {
        _jumping = _justJumped = ctx.ReadValueAsButton();
        if (ctx.ReadValueAsButton())
            jumpGraceTimer.Start();
    }

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

    [SerializeField]
    Transform heldTransform;
    private void FlipScale() {
        var localScale = heldTransform.transform.localScale;
        localScale.x *= -1;
        heldTransform.localScale = localScale;
    }
}
