using UnityEngine;

public class BotAttackState : EnemyState
{
    private BotEnemy bot;
    private float attackCooldown = 1f;
    private float lastAttackTime;
    private float attackRange = 1.2f;
    private float chaseRange = 3f;

    private GameObject attackHitbox;

    public BotAttackState(BotEnemy bot) : base(bot)
    {
        this.bot = bot;
    }

    public override void Enter()
    {
        bot.animator?.SetBool("isMoving", true);

        attackHitbox = bot.transform.Find("BotHitbox")?.gameObject;
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    public override void Execute()
    {
        float distance = Vector2.Distance(bot.transform.position, bot.Player.position);

        if (distance > chaseRange)
        {
            float directionToPlayer = Mathf.Sign(bot.Player.position.x - bot.transform.position.x);
            float posX = bot.transform.position.x;
            bool isBeyondLeft = posX <= bot.leftBoundaryX;
            bool isBeyondRight = posX >= bot.rightBoundaryX;

            float patrolDirection = directionToPlayer;
            if (isBeyondLeft) patrolDirection = 1f;
            else if (isBeyondRight) patrolDirection = -1f;

            bot.ChangeState(new BotPatrolState(bot, patrolDirection));
            return;
        }

        // Move toward player
        float horizontalDir = Mathf.Sign(bot.Player.position.x - bot.transform.position.x);
        bot.SetVelocity(new Vector2(horizontalDir * bot.moveSpeed, bot.rb.linearVelocity.y));
        bot.FaceDirection(horizontalDir);

        // Attack if in range and cooldown passed
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            if (attackHitbox != null)
            {
                attackHitbox.SetActive(true);
                bot.StartCoroutine(DisableHitboxAfterDelay(0.2f));
            }

            lastAttackTime = Time.time;
        }
    }

    public override void Exit()
    {
        bot.animator?.SetBool("isMoving", false);
        bot.SetVelocity(Vector2.zero);

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    private System.Collections.IEnumerator DisableHitboxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }
}
