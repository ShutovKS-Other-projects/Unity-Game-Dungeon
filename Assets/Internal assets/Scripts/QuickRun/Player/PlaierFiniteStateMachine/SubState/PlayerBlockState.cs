using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockState : PlayerAbilityState
{
    public PlayerBlockState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //OnEnable trigger weapon collider
    }

    public override void Exit()
    {
        base.Exit();

        //OnDisable trigger weapon collider
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!playerStateController.InputManager.GetPlayerBlockInput())
        {
            isAbilityDone = true;
        }
        else if (playerStateController.InputManager.GetPlayerAttackInput())
        {
            stateMachine.ChangeState(playerStateController.AttackState);
        }
    }
}
