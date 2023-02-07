using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    protected PlayerStateController playerStateController;
    protected PlayerStateMachine stateMachine;
    public PlayerData playerData;

    protected bool isAnimationFinished;
    protected bool isExitingState;

    protected float startTime;
    
    private string animBoolName;

    protected Transform playerTransform => playerStateController.transform;

    public PlayerState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
    {
        this.playerStateController = playerStateController;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        DoChecks();
        playerStateController.Animator.SetBool(animBoolName, true);
        startTime = Time.time;
        isAnimationFinished = false;
        isExitingState = false;
    }

    public virtual void Exit()
    {
        playerStateController.Animator.SetBool(animBoolName, false);
        isExitingState = true;
    }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() => DoChecks();

    public virtual void DoChecks() { }

    public virtual void AnimationTrigger() { }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
}