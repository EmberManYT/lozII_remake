using UnityEngine;

public class StalfosEnemy : EnemyController
{
    [Header("Patrol Boundaries")]
    public float leftBoundaryX;
    public float rightBoundaryX;

    [Header("Player Tracking")]
    public Transform player;
    public float chaseRange = 4f;

    [Header("Speed Settings")]
    public float chaseSpeed = 2f;

    [Header("Combat Hitboxes")]
    public GameObject shieldHitbox;
    public GameObject swordHitbox;
    private BoxCollider2D hurtbox;

    [HideInInspector]
    public float currentDirection = 1f;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        hurtbox = GetComponent<BoxCollider2D>();
        ChangeState(new StalfosPatrolState(this));

        SetAttackPose(false); // Default state
    }

    public void SetAttackPose(bool attacking)
    {
        if (attacking)
        {
            EnableSwordHitbox();
            DisableShield();
            SetAttackHurtbox();
        }
        else
        {
            DisableSwordHitbox();
            EnableShield();
            SetIdleHurtbox();
        }
    }

    public void StartAttackPose() => SetAttackPose(true);
    public void EndAttackPose() => SetAttackPose(false);

    public void EnableSwordHitbox() => swordHitbox?.SetActive(true);
    public void DisableSwordHitbox() => swordHitbox?.SetActive(false);

    public void EnableShield() => shieldHitbox?.SetActive(true);
    public void DisableShield() => shieldHitbox?.SetActive(false);

    public void SetIdleHurtbox()
    {
        if (hurtbox != null)
        {
            hurtbox.offset = new Vector2(0f, 0f);
            hurtbox.size = new Vector2(0.9f, 2f);
        }
    }

    public void SetAttackHurtbox()
    {
        if (hurtbox != null)
        {
            hurtbox.offset = new Vector2(0.3f, 0f);
            hurtbox.size = new Vector2(1.35f, 2f);
        }
    }
}

