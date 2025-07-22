using UnityEngine;

public class StalfosEnemy : EnemyController {
    [Header("Patrol Boundaries")]
    public float leftBoundaryX;
    public float rightBoundaryX;

    [Header("Player Tracking")]
    public Transform player;
    public float chaseRange = 4f;
    
    [Header("Speed Settings")]
    public float chaseSpeed = 2f;

    [HideInInspector]
    public float currentDirection = 1f;


    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        ChangeState(new StalfosPatrolState(this));
    }
}
