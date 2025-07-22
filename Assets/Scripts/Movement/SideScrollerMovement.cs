using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SideScrollerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpDelay = 0.1f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float airControlSpeedMultiplier = 1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Crouch Settings")]
    // [SerializeField] private Collider2D playerCollider;
    // [SerializeField] private SpriteRenderer spriteRenderer;
    // [SerializeField] private float crouchRatio = 0.5f;
    // private float originalColliderHeight;
    // private Vector2 originalColliderOffset;
    // private float originalSpriteHeight; 

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerControls playerControls; 

    // Movement State Vars
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isJumping;
    private bool isFalling;
    private bool isCrouching;
    private Coroutine jumpCoroutine;
    private int currentDirection = 1;
    private float jumpDirectionXMomentum = 0f;

    // Animations
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsFalling = Animator.StringToHash("isFalling");
    private static readonly int IsCrouching = Animator.StringToHash("isCrouching");
    private static readonly int Direction = Animator.StringToHash("direction"); // 1 for right, -1 for left

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerControls = new PlayerControls();

        // if (spriteRenderer == null)
        // {
        //     spriteRenderer = GetComponent<SpriteRenderer>();
        // }

        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        playerControls.Player.Jump.started += OnJumpStarted;
        playerControls.Player.Crouch.started += OnCrouchStarted;
        playerControls.Player.Crouch.canceled += OnCrouchCanceled;

        // WILL FIX CROUCH LATER
        // if (playerCollider != null)
        // {
        //     if (playerCollider is CapsuleCollider2D capsule)
        //     {
        //         originalColliderHeight = capsule.size.y;
        //         originalColliderOffset = capsule.offset;
        //     }
        //     else if (playerCollider is BoxCollider2D box)
        //     {
        //         originalColliderHeight = box.size.y;
        //         originalColliderOffset = box.offset;
        //     }
        //     else
        //     {
        //         Debug.LogError("Unsupported Collider2D type! Only CapsuleCollider2D and BoxCollider2D are supported for crouching.");
        //     }
        // }
        // else
        // {
        //     Debug.LogError("Player Collider not assigned in SideScrollerMovement script!");
        // }

        // if (spriteRenderer != null)
        // {
        //     originalSpriteHeight = spriteRenderer.bounds.size.y;
        // }
        // else
        // {
        //     Debug.LogWarning("Sprite Renderer not assigned or found. Crouching visual adjustment might be inaccurate.");
        //     originalSpriteHeight = originalColliderHeight;
        // }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

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
            animator.SetBool(IsWalking, false);
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (isGrounded && !isCrouching && jumpCoroutine == null)
        {
            isJumping = true;
            isFalling = false;
            jumpDirectionXMomentum = rb.linearVelocity.x; 
            jumpCoroutine = StartCoroutine(JumpWithDelay());
        }
    }

    private IEnumerator JumpWithDelay()
    {
        yield return new WaitForSeconds(jumpDelay); 
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); 
        jumpCoroutine = null; 
    }

    private void OnCrouchStarted(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            isCrouching = true;
            // SetCrouchCollider(true);
        }
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        if (isCrouching)
        {
            isCrouching = false;
            // SetCrouchCollider(false);
        }
    }

    private void HandleMovement()
    {
        float currentMoveSpeed = moveSpeed;

        if (isCrouching)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // stop movement when crouched
            return;
        }

        if (isJumping)
        {
            rb.linearVelocity = new Vector2(jumpDirectionXMomentum, rb.linearVelocity.y); 
            return; 
        }
        else if (isFalling)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed * airControlSpeedMultiplier, rb.linearVelocity.y); 
            if (moveInput.x != 0)
            {
                UpdateDirection(moveInput.x);
            }
        }
        else 
        {
            rb.linearVelocity = new Vector2(moveInput.x * currentMoveSpeed, rb.linearVelocity.y); 
        }
    }

    private void HandleJumpPhysics()
    {
        if (rb.linearVelocity.y < 0 && !isGrounded) 
        {
            isJumping = false;
            isFalling = true;
        }
        else if (isGrounded && (isJumping || isFalling)) 
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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && (isJumping || isFalling))
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
        }
        else if (xInput < 0)
        {
            currentDirection = -1;
        }
        animator.SetFloat(Direction, currentDirection);
    }

    private void UpdateAnimations()
    {
        animator.SetBool(IsCrouching, isCrouching);

        if (isCrouching)
        {
            animator.SetBool(IsWalking, false);
            animator.SetBool(IsJumping, false);
            animator.SetBool(IsFalling, false);
            return;
        }

        if (isJumping)
        {
            animator.SetBool(IsJumping, true);
            animator.SetBool(IsFalling, false);
            animator.SetBool(IsWalking, false);
        }
        else if (isFalling)
        {
            animator.SetBool(IsJumping, false);
            animator.SetBool(IsFalling, true);
            animator.SetBool(IsWalking, false);
        }
        else if (isGrounded)
        {
            animator.SetBool(IsJumping, false);
            animator.SetBool(IsFalling, false);
            if (Mathf.Abs(moveInput.x) > 0.01f) 
            {
                animator.SetBool(IsWalking, true);
            }
            else
            {
                animator.SetBool(IsWalking, false);
            }
        }
        else 
        {
            animator.SetBool(IsWalking, false);
            if(rb.linearVelocity.y > 0) 
            {
                animator.SetBool(IsJumping, true);
                animator.SetBool(IsFalling, false);
            }
            else if (Mathf.Abs(rb.linearVelocity.y) < 0.1f) 
            {
                animator.SetBool(IsJumping, true);
                animator.SetBool(IsFalling, false);
            }
        }
    }

    // WILL FIX
    // private void SetCrouchCollider(bool crouch)
    // {
    //     if (playerCollider == null) return;

    //     float heightReduction = originalSpriteHeight * (1f - crouchRatio); 
    //     float targetCrouchColliderHeight = originalColliderHeight * crouchRatio;
    //     float deltaHeight = originalColliderHeight - targetCrouchColliderHeight; 
    //     float newOffsetY = originalColliderOffset.y + (deltaHeight / 2f);

    //     if (crouch)
    //     {
    //         if (playerCollider is CapsuleCollider2D capsule)
    //         {
    //             capsule.size = new Vector2(capsule.size.x, targetCrouchColliderHeight);
    //             capsule.offset = new Vector2(originalColliderOffset.x, newOffsetY);
    //         }
    //         else if (playerCollider is BoxCollider2D box)
    //         {
    //             box.size = new Vector2(box.size.x, targetCrouchColliderHeight);
    //             box.offset = new Vector2(originalColliderOffset.x, newOffsetY);
    //         }

    //         transform.position -= new Vector3(0, heightReduction, 0);

    //     }
    //     else // Standing up
    //     {
    //         transform.position += new Vector3(0, heightReduction, 0);

    //         if (playerCollider is CapsuleCollider2D capsule)
    //         {
    //             capsule.size = new Vector2(capsule.size.x, originalColliderHeight);
    //             capsule.offset = originalColliderOffset;
    //         }
    //         else if (playerCollider is BoxCollider2D box)
    //         {
    //             box.size = new Vector2(box.size.x, originalColliderHeight);
    //             box.offset = originalColliderOffset;
    //         }
    //     }
    // }
    
    private void OnDrawGizmos()     // debugging
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}