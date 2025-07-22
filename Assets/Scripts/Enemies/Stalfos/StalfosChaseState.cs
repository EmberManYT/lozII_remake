using UnityEngine;
using System;

public class StalfosChaseState : EnemyState {
    public StalfosChaseState(EnemyController enemy) : base(enemy) {}

    public override void Enter() {
        enemy.animator?.SetBool("isMoving", true);
    }

    public override void Execute() {
        StalfosEnemy stalfos = enemy as StalfosEnemy;
        Vector2 dirToPlayer = stalfos.player.position - enemy.transform.position;

        float direction = Mathf.Sign(dirToPlayer.x);
        enemy.rb.linearVelocity = new Vector2(direction * stalfos.chaseSpeed, enemy.rb.linearVelocity.y);
        enemy.animator?.SetFloat("direction", direction);
        stalfos.currentDirection = direction;

        float distance = Vector2.Distance(enemy.transform.position, stalfos.player.position);

        if (distance > stalfos.chaseRange + 1f) {
            enemy.ChangeState(new StalfosPatrolState(enemy));
        }
    }
    
    public override void Exit()
    {
        enemy.animator?.SetBool("isMoving", false);
    }
}
