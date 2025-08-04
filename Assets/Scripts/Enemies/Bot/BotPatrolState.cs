using UnityEngine;
using System;

public class BotPatrolState : EnemyState
{
    public float direction = 1f;
    public BotPatrolState(EnemyController enemy, float startDirection = 1f) : base(enemy)
    {
        direction = Mathf.Sign(startDirection);
    }
    public override void Enter()
    {
        enemy.animator?.SetBool("isMoving", true);
    }

    public override void Execute()
    {
        BotEnemy bot = enemy as BotEnemy;
        Transform player = bot.Player;
        float posX = enemy.transform.position.x;

        if (Vector2.Distance(player.position, enemy.transform.position) < 1f)
        {
            enemy.ChangeState(new BotAttackState(bot));
            return;
        }

        enemy.rb.linearVelocity = new Vector2(direction * enemy.moveSpeed, enemy.rb.linearVelocity.y);

        bool isBeyondLeft = posX <= bot.leftBoundaryX;
        bool isBeyondRight = posX >= bot.rightBoundaryX;
        bool noGroundAhead = !Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, 1f, enemy.groundLayer);

        if (isBeyondLeft)
        {
            direction = Math.Abs(direction);
            Flip();
        }
        else if (isBeyondRight)
        {
            direction = -Math.Abs(direction);
            Flip();
        }

        if (noGroundAhead)
        {
            direction *= -1f;
            Flip();
        }
    }

    public override void Exit()
    {
        enemy.animator?.SetBool("isMoving", false);
    }

    private void Flip()
    {
        Vector3 localScale = enemy.transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * direction;
        enemy.transform.localScale = localScale;
    }
}
