using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockState : PlayerAbilityState
{
    public PlayerBlockState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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

        if (!player.InputManager.GetPlayerBlockInput())
        {
            isAbilityDone = true;
        }
        else if (player.InputManager.GetPlayerAttackInput())
        {
            stateMachine.ChangeState(player.AttackState);
        }
    }
}
