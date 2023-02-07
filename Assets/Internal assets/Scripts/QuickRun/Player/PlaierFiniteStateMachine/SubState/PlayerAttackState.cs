using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    public PlayerAttackState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerData.stamina -= 10;
        //OnEnable trigger weapon collider
    }

    public override void Exit()
    {
        base.Exit();

        //OnDisable trigger weapon collider
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }
}
