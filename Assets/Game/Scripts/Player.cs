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
    Timer varJumpTimer = new Timer(.3f);

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
    private PlayerInputActions _actions;

    public Vector2 controllerInput;

    private Vector2 movementInput;
    private float boostDir = 1;

    SpriteFlash spriteFlash;

    [SerializeField]
    private Transform weaponTransform;

    private void Awake()
    {
        _anim ??= heldTransform.Find("Sprite").GetComponent<SpriteAnimator>();
        _input ??= GetComponent<PlayerInput>();

        _anim.AnimationFinished += OnAnimationFinish;

        spriteFlash = GetComponentInChildren<SpriteFlash>();

        controller = GetComponent<Controller>();

        _actions = new PlayerInputActions();
    }

    void Update()
    {
        wallJumpGraceTimer.Update();
        forceInputsTimer.Update();
        jumpGraceTimer.Update();
        coyoteTimer.Update();
        attackCooldownTimer.Update();
        attackTimer.Update();

        var control = _actions.Player.Direction.ReadValue<Vector2>();
        controllerInput.x = Mathf.Abs(control.x) > 0.5f ? Mathf.Sign(control.x) : 0;
        controllerInput.y = Mathf.Abs(control.y) > 0.5f ? Mathf.Sign(control.y) : 0;
        movementInput = controllerInput;
        if (forceInputsTimer) movementInput = forceInputs;

        if (controller.collisions.below || controller.collisions.above)
        {
            velocity.y = 0;
            varJumpTimer.Zero();
        }

        if (controller.collisions.below)
        {
            coyoteTimer.Start();
        }

        if (jumpGraceTimer && coyoteTimer)
        {
            jumpGraceTimer.Zero();
            velocity.y = jumpVelocity;
            varJumpTimer.Start();
            if (Mathf.Abs(movementInput.x) > 0)
            {
                velocity.x += jumpHBoost * Mathf.Sign(movementInput.x);
            }
        }

        if ((controller.collisions.right || controller.collisions.left) && !controller.collisions.below)
        {
            wallJumpGraceTimer.Start();
            boostDir = controller.collisions.right ? -1 : 1;
        }

        if (_justJumped)
        {
            _justJumped = false;
            jumpGraceTimer.Start();
        }

        if (jumpGraceTimer && wallJumpGraceTimer)
        {
            jumpGraceTimer.Zero();

            velocity.y = jumpVelocity;

            varJumpTimer.Start();

            velocity.x = wallJumpHBoost * boostDir;
            forceInputs.x = boostDir;
            forceInputs.y = 0;
            forceInputsTimer.time = .25f;
            forceInputsTimer.Start();
        }


        if (varJumpTimer)
        {
            if (_jumping)
            {
                velocity.y = jumpVelocity;
                varJumpTimer.Update();
            }
            else
            {
                varJumpTimer.Zero();
            }
        }

        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position, collider.bounds.size, 0, Vector2.zero, 0, enemyMask);

        if (hit)
        {
            Vector2 recoil;
            if (transform.position.x < hit.transform.position.x)
            {
                recoil.x = -recoilSpeed;
            }
            else
            {
                recoil.x = recoilSpeed;
            }
            recoil.y = recoilSpeed;
            // velocity = recoil;
        }

        velocity.x = movementInput.x * moveSpeed;
        //float accelTime = controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne;
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelTime);
        bool clinging = (controller.collisions.right && movementInput.x == 1) || (controller.collisions.left && movementInput.x == -1);

        float maxFall = clinging ? slowMaxFall : defaultMaxFall;
        velocity.y = Mathf.Max(velocity.y + gravity * Time.deltaTime, maxFall);
        controller.Move(velocity * Time.deltaTime);

        if (_attackPressed)
        {
            _attackPressed = false;
            if (!attackCooldownTimer)
            {
                attackCooldownTimer.time = attackCooldown;
                attackCooldownTimer.Start();

                attackTimer.time = attackTime;
                attackTimer.Start();

                spriteFlash.Flash(0.25f, 0.05f);
                Vector3 weaponTransformLocalPostion = Vector3.zero;
                if (Mathf.Abs(controllerInput.y) > float.Epsilon)
                {
                    weaponTransformLocalPostion.y = 1.5f * controllerInput.y;
                }
                else
                {
                    weaponTransformLocalPostion.x = 1.5f;
                }
                weaponTransform.localPosition = weaponTransformLocalPostion;
                weaponTransform.gameObject.SetActive(true);
            }

        }

        if (!attackTimer)
        {
            weaponTransform.gameObject.SetActive(false);
        }

        if (!controller.collisions.below)
        {
            if (heldTransform.localScale.x < 0 && movementInput.x > 0 ||
            heldTransform.localScale.x > 0 && movementInput.x < 0)
                FlipScale();
            if (velocity.y >= 8)
            {
                _anim.Play("Jump");
            }
            else if (velocity.y < 8 && velocity.y >= 0)
            {
                _anim.Play("JumpMid");
            }
            else if (velocity.y < 0 && _anim.CurrentAnimation.animationName != "JumpMid")
            {
                _anim.Play("Fall");
            }
        }
        else
        {
            if (heldTransform.localScale.x < 0 && movementInput.x > 0 ||
                    heldTransform.localScale.x > 0 && movementInput.x < 0)
            {
                FlipScale();
                _anim.Stop();
                _anim.Play("Turn");
            }
            else if (Mathf.Abs(velocity.x) > float.Epsilon)
            {
                if (_anim.CurrentAnimation.animationName != "Turn")
                {
                    _anim.Play("Run");
                }
            }
            else if (movementInput.x == 0 && _anim.CurrentAnimation.animationName == "Run")
                _anim.Play("RunStop");
            else if (_anim.CurrentAnimation.animationName != "RunStop" &&
                     _anim.CurrentAnimation.animationName != "Turn")
                _anim.Play("Idle");
        }

        if (attackCooldownTimer)
        {
            if (_anim.CurrentAnimation.animationName != "Slash")
            {
                _anim.Play("Slash");
            }
        }
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

    private void OnDestroy()
    {
        _anim.AnimationFinished -= OnAnimationFinish;
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

    [SerializeField]
    float attackCooldown;
    Timer attackCooldownTimer = new Timer();

    [SerializeField]
    float attackTime;
    Timer attackTimer = new Timer();
    bool _attackPressed = false;
    private void OnAttack(InputAction.CallbackContext ctx)
    {
        _attackPressed = true;
    }

    private bool _jumping;
    private bool _justJumped;
    private void OnJump(InputAction.CallbackContext ctx)
    {
        _jumping = _justJumped = ctx.ReadValueAsButton();

    }

    [SerializeField]
    Transform heldTransform;
    private void FlipScale()
    {
        var heldScale = heldTransform.localScale;
        heldScale.x *= -1;
        heldTransform.localScale = heldScale;

    }
}
