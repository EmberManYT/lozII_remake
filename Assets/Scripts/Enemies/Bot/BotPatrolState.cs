using UnityEngine;
using System;

public class BotPatrolState : EnemyState {
    private float direction = 1f;

    public BotPatrolState(EnemyController enemy) : base(enemy) {}

    public override void Enter() {
        enemy.animator?.SetBool("isMoving", true);
    }

    public override void Execute() {
        enemy.rb.linearVelocity = new Vector2(direction * enemy.moveSpeed, enemy.rb.linearVelocity.y);
        float posX = enemy.transform.position.x;
        BotEnemy Bot = enemy as BotEnemy;

        bool isBeyondLeft = posX <= Bot.leftBoundaryX;
        bool isBeyondRight = posX >= Bot.rightBoundaryX;
        bool noGroundAhead = !Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, 1f, enemy.groundLayer);
        
        if (isBeyondLeft)
        {
            direction = Math.Abs(direction);
            Vector3 localScale = enemy.transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * direction;
            enemy.transform.localScale = localScale;
        }
        else if (isBeyondRight)
        {
            direction = -Math.Abs(direction);
            Vector3 localScale = enemy.transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * direction;
            enemy.transform.localScale = localScale;
        }
        if (noGroundAhead)
        {
            direction *= -1f;
            Vector3 localScale = enemy.transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * direction;
            enemy.transform.localScale = localScale;
        }
    }

    public override void Exit() {
        enemy.animator?.SetBool("isMoving", false);
    }
}
