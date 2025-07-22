using UnityEngine;

public class BotEnemy : EnemyController {
    [Header("Patrol Boundaries")]
    public float leftBoundaryX;
    public float rightBoundaryX;
    private void Start()
    {
        ChangeState(new BotPatrolState(this));
    }
}
