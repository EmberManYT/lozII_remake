using UnityEngine;

public class StalfosPatrolState : EnemyState
{
    private float direction = 1f;

    public StalfosPatrolState(EnemyController enemy) : base(enemy) {}

    public override void Enter()
    {
        StalfosEnemy stalfos = enemy as StalfosEnemy;
        direction = stalfos.currentDirection;

        enemy.animator?.SetBool("isMoving", true);
        enemy.FlipSprite(direction);
    }

    public override void Execute()
    {
        if (!(enemy is StalfosEnemy stalfos) || !stalfos.player) return;

        enemy.rb.linearVelocity = new Vector2(direction * enemy.moveSpeed, enemy.rb.linearVelocity.y);

        float posX = enemy.transform.position.x;
        float playerDistance = Vector2.Distance(enemy.transform.position, stalfos.player.position);

        enemy.FlipSprite(direction);

        bool isBeyondLeft = posX <= stalfos.leftBoundaryX;
        bool isBeyondRight = posX >= stalfos.rightBoundaryX;

        if (isBeyondLeft) direction = 1f;
        else if (isBeyondRight) direction = -1f;

        //bool noGroundAhead = !Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, 1f, enemy.groundLayer);
        //if (noGroundAhead) direction *= -1f;

        stalfos.currentDirection = direction;

        if (playerDistance <= stalfos.chaseRange)
        {
            enemy.ChangeState(new StalfosChaseState(enemy));
        }
    }

    public override void Exit()
    {
        enemy.animator?.SetBool("isMoving", false);
    }
}
