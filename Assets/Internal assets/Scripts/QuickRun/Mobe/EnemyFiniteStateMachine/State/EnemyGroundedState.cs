using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundedState : EnemyState
{
    private bool isPlayer;
    private float playerDistance;

    public EnemyGroundedState(EnemyStateController enemyStateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemyStateController, stateMachine, enemyData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayer = enemyStateController.CheckIfPlayer();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        isPlayer = enemyStateController.CheckIfPlayer();
        playerDistance = enemyStateController.CheckPlayerDistance();

        if (isPlayer)
        {
            stateMachine.ChangeState(enemyStateController.MoveState);
        }
    }
}
