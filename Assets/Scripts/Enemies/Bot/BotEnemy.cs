using UnityEngine;

public class BotEnemy : EnemyController
{
    [Header("Patrol Boundaries")]
    public float leftBoundaryX;
    public float rightBoundaryX;

    public Transform Player { get; private set; }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        ChangeState(new BotPatrolState(this));
    }

    public void SetVelocity(Vector2 velocity)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = velocity;
    }

    public void FaceDirection(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        transform.localScale = scale;
    }
}
