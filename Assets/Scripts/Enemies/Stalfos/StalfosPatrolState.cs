using UnityEngine;
using System;

public class StalfosPatrolState : EnemyState {
    private float direction = 1f;
    public StalfosPatrolState(EnemyController enemy) : base(enemy) {}

    public override void Enter() {
        StalfosEnemy stalfos = enemy as StalfosEnemy;
        direction = stalfos.currentDirection;
        enemy.animator?.SetBool("isMoving", true);
        enemy.animator?.SetFloat("direction", direction);
    }

    public override void Execute() {
        enemy.rb.linearVelocity = new Vector2(direction * enemy.moveSpeed, enemy.rb.linearVelocity.y);
        StalfosEnemy stalfos = enemy as StalfosEnemy;
        float posX = enemy.transform.position.x;
        float playerDistance = Vector2.Distance(enemy.transform.position, stalfos.player.position);

        bool isBeyondLeft = posX <= stalfos.leftBoundaryX;
        bool isBeyondRight = posX >= stalfos.rightBoundaryX;

        if (isBeyondLeft) {
            direction = Math.Abs(direction);
        }
        else if (isBeyondRight) {
            direction = -Math.Abs(direction);
        }

        bool noGroundAhead = !Physics2D.Raycast(enemy.groundCheck.position, Vector2.down, 1f, enemy.groundLayer);
        if (noGroundAhead) {
            direction *= -1f;
        }

        enemy.animator?.SetFloat("direction", direction);
        stalfos.currentDirection = direction;

        if (playerDistance <= stalfos.chaseRange) {
            enemy.ChangeState(new StalfosChaseState(enemy));
        }
    }

    public override void Exit() {
        enemy.animator?.SetBool("isMoving", false);
    }
}
