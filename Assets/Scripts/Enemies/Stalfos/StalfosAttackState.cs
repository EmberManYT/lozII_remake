using UnityEngine;

public class StalfosAttackState : EnemyState
{
    private StalfosEnemy stalfos;
    private float attackRange = 1.5f;
    private float attackCooldown = 1.25f;
    private float attackTimer;

    public StalfosAttackState(EnemyController enemy) : base(enemy)
    {
        stalfos = enemy as StalfosEnemy;
    }

    public override void Enter()
    {
        if (stalfos == null || stalfos.player == null)
        {
            enemy.ChangeState(new StalfosPatrolState(enemy));
            return;
        }

        enemy.rb.linearVelocity = Vector2.zero;
        enemy.animator?.SetTrigger("attack");
        attackTimer = Time.time;
    }

    public override void Execute()
    {
        if (stalfos == null || stalfos.player == null)
        {
            enemy.ChangeState(new StalfosPatrolState(enemy));
            return;
        }

        float distance = Vector2.Distance(enemy.transform.position, stalfos.player.position);

        if (distance > attackRange && Time.time > attackTimer + 0.25f)
        {
            enemy.ChangeState(new StalfosChaseState(enemy));
        }

        if (Time.time > attackTimer + attackCooldown)
        {
            enemy.ChangeState(new StalfosChaseState(enemy));
        }
    }

    public override void Exit()
    {
    }
}