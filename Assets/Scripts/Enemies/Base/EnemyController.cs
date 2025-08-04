using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    protected EnemyState currentState;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Settings")]
    public float moveSpeed = 1f;
    public float detectionRange = 5f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
}
