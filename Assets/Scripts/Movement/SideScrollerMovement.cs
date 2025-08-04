using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SideScrollerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpDelay = 0.15f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float airControlSpeedMultiplier = 1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerControls playerControls;
    private BoxCollider2D boxCollider;
    private PlayerCombatController combat;

    private Vector2 moveInput;
    private bool grounded;
    private bool isJumping;
    private bool isFalling;
    private bool isCrouching;
    private Coroutine jumpCoroutine;
    private int currentDirection = 1;
    private float jumpDirectionXMomentum = 0f;

    public bool isAttacking { get; private set; }
    public bool isCrouched => isCrouching;
    public bool isGrounded => grounded;
    public Rigidbody2D RB => rb;
    public Vector2 MoveInput => moveInput;

    private static readonly int animIsWalking = Animator.StringToHash("isWalking");
    private static readonly int animIsJumping = Animator.StringToHash("isJumping");
    private static readonly int animIsFalling = Animator.StringToHash("isFalling");
    private static readonly int animIsCrouching = Animator.StringToHash("isCrouching");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        combat = GetComponent<PlayerCombatController>();
        playerControls = new PlayerControls();

        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        playerControls.Player.Jump.started += OnJumpStarted;
        playerControls.Player.Crouch.started += OnCrouchStarted;
        playerControls.Player.Crouch.canceled += OnCrouchCanceled;
    }

    private void OnEnable() => playerControls.Enable();
    private void OnDisable() => playerControls.Disable();

    private void Update()
    {
        CheckGroundStatus();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJumpPhysics();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (!isJumping && !isCrouching)
        {
            UpdateDirection(moveInput.x);
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        if (!isJumping)
        {
            animator.SetBool(animIsWalking, false);
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (animator.GetBool("attackHold")) return;
        if (grounded && !isCrouching && jumpCoroutine == null)
        {
            isJumping = true;
            isFalling = false;
            jumpDirectionXMomentum = rb.linearVelocity.x;
            jumpCoroutine = StartCoroutine(JumpWithDelay());
        }
    }

    private IEnumerator JumpWithDelay()
    {
        isCrouching = true;
        UpdateAnimations();

        yield return new WaitForSeconds(jumpDelay);

        isCrouching = false;
        isJumping = true;
        UpdateAnimations();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCoroutine = null;
    }

    private void OnCrouchStarted(InputAction.CallbackContext context)
    {
        if (grounded)
        {
            isCrouching = true;
        }
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        if (isCrouching)
        {
            isCrouching = false;
        }
    }

    public void SetAttacking(bool state)
    {
        isAttacking = state;
    }

    private void HandleMovement()
    {
        bool windUpHold = animator.GetBool("attackHold");

        if (isAttacking && windUpHold)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (moveInput.x != 0) UpdateDirection(moveInput.x);
            return;
        }

        // Thrust or any other attack lock: full lock
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Crouching: freeze horizontal movement but allow turning
        if (isCrouching && grounded)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (moveInput.x != 0) UpdateDirection(moveInput.x);
            return;
        }

        // Jumping and Falling
        if (isJumping)
        {
            rb.linearVelocity = new Vector2(jumpDirectionXMomentum, rb.linearVelocity.y);
        }
        else if (isFalling)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed * airControlSpeedMultiplier, rb.linearVelocity.y);
            if (moveInput.x != 0) UpdateDirection(moveInput.x);
        }
        else // Normal grounded movement
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            if (moveInput.x != 0) UpdateDirection(moveInput.x);
        }
    }

    private void HandleJumpPhysics()
    {
        if (rb.linearVelocity.y < 0 && !grounded)
        {
            isJumping = false;
            isFalling = true;
        }
        else if (grounded && (isJumping || isFalling))
        {
            isJumping = false;
            isFalling = false;
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
            }
        }
    }

    private void CheckGroundStatus()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (grounded && (isJumping || isFalling))
        {
            isJumping = false;
            isFalling = false;
        }
    }

    private void UpdateDirection(float xInput)
    {
        if (xInput > 0)
        {
            currentDirection = 1;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (xInput < 0)
        {
            currentDirection = -1;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void UpdateAnimations()
    {
        animator.SetBool(animIsCrouching, isCrouching);

        bool isMovingHorizontally = Mathf.Abs(moveInput.x) > 0.01f;
        animator.SetBool(animIsWalking, isMovingHorizontally && grounded && !isJumping && !isFalling);

        if (!grounded)
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                animator.SetBool(animIsJumping, true);
                animator.SetBool(animIsFalling, false);
            }
            else if (rb.linearVelocity.y < -0.1f)
            {
                animator.SetBool(animIsJumping, false);
                animator.SetBool(animIsFalling, true);
            }
        }
        else
        {
            animator.SetBool(animIsJumping, false);
            animator.SetBool(animIsFalling, false);
        }

        UpdateColliderForState();
    }

    public void UpdateColliderForState()
    {
        if (isAttacking)
        {
            bool windUpHold = animator.GetBool("attackHold");

            if (windUpHold)
            {
                boxCollider.offset = new Vector2(0f, 0f);
                boxCollider.size = new Vector2(1.5f, 2f);
            }
            return;
        }

        if (isCrouching)
        {
            boxCollider.offset = new Vector2(0f, -0.138f);
            boxCollider.size = new Vector2(1f, 1.724f);
        }
        else if (!grounded)
        {
            boxCollider.offset = new Vector2(0f, 0f);
            boxCollider.size = new Vector2(1f, 2f);
        }
        else
        {
            boxCollider.offset = new Vector2(0f, 0f);
            boxCollider.size = new Vector2(1f, 1.93f);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
