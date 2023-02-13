using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilityState : EnemyState
{
    protected bool isAbilityDone;

    public EnemyAbilityState(EnemyStateController enemyStateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemyStateController, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (isAbilityDone)
        {
            stateMachine.ChangeState(enemyStateController.IdleState);
        }
    }
}
