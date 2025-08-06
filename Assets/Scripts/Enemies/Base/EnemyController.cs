using UnityEngine;
using System.Collections;

public abstract class EnemyController : MonoBehaviour
{
    protected EnemyState currentState;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 1f;
    public float detectionRange = 5f;

    [Header("Facing Settings")]
    [Tooltip("Initial facing direction. -1 = left, 1 = right")]
    [SerializeField] private int initialDirection = -1; // Set in inspector
    private int currentFacingDirection;

    [Header("Damage Effects")]
    public float knockbackForce = 3f;
    public float flashDuration = 0.15f;
    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentFacingDirection = Mathf.Clamp(initialDirection, -1, 1);
        FlipSprite(currentFacingDirection);
    }

    protected virtual void Update()
    {
        currentState?.Execute();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void FlipSprite(float moveDirection)
    {
        int dir = Mathf.Clamp(Mathf.RoundToInt(moveDirection), -1, 1);

        if (dir != currentFacingDirection && dir != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
            currentFacingDirection = dir;
        }
    }
    
    public void TakeDamage(int amount, Vector2 sourcePosition, bool applyKnockback)
    {
        EnemyHealth health = GetComponent<EnemyHealth>();
        if (health == null || health.IsDead) return;

        health.TakeDamage(amount);

        if (applyKnockback)
        {
            Vector2 knockDir = (Vector2)(transform.position - (Vector3)sourcePosition).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(knockDir.x, 0.3f) * knockbackForce, ForceMode2D.Impulse);
        }

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
        }
    }
}
