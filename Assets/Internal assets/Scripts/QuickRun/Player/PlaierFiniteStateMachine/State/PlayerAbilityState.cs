using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    protected bool isAbilityDone;
    protected bool isGrounded;

    public PlayerAbilityState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = playerStateController.CheckIfGrounded();
    }

    public override void Enter()
    {
        base.Enter();

        isAbilityDone = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(isAbilityDone)
        {
            if(isGrounded && playerStateController.RB.velocity.y < 0.01f)
            {
                stateMachine.ChangeState(playerStateController.IdleState);
            }
            else
            {
                stateMachine.ChangeState(playerStateController.InAirState);
            }
        }
    }
}
