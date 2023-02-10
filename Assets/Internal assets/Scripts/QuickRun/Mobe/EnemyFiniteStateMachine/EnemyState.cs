using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    protected EnemyStateController enemyStateController;
    protected EnemyStateMachine stateMachine;
    protected EnemyState enemyState;
    public EnemyData enemyData;

    protected bool isAnimationFinished;
    protected bool isExitingState;

    protected float startTime;

    private string animBoolName;

    public EnemyState(EnemyStateController enemyStateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName)
    {
        this.enemyStateController = enemyStateController;
        this.stateMachine = stateMachine;
        this.enemyData = enemyData;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        DoChecks();
        enemyStateController.Animator.SetBool(animBoolName, true);
        startTime = Time.time;
        isAnimationFinished = false;
        isExitingState = false;
    }

    public virtual void Exit()
    {
        enemyStateController.Animator.SetBool(animBoolName, false);
        isExitingState = true;
    }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() => DoChecks();

    public virtual void DoChecks() { }

    public virtual void AnimationTrigger() { }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
}
