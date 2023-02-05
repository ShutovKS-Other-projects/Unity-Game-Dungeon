using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit(); 
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (movementInput.y == 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }    
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.Movement(movementInput);
    }
}
