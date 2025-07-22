using UnityEngine;

public abstract class EnemyState {
    protected EnemyController enemy;

    public EnemyState(EnemyController enemy) {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}

