using System;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine
{
    public class EnemyState : MonoBehaviour
    {
        protected EnemyStateController StateController;
        protected EnemyStateMachine StateMachine;
        protected EnemyState enemyState;
        [NonSerialized] public EnemyData enemyData;

        protected bool IsAnimationFinished;
        protected bool IsExitingState;

        protected float StartTime;

        private string _animBoolName;

        public EnemyState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName)
        {
            this.StateController = stateController;
            this.StateMachine = stateMachine;
            this.enemyData = enemyData;
            this._animBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            DoChecks();
            StateController.Animator.SetBool(_animBoolName, true);
            StartTime = Time.time;
            IsAnimationFinished = false;
            IsExitingState = false;
        }

        public virtual void Exit()
        {
            StateController.Animator.SetBool(_animBoolName, false);
            IsExitingState = true;
        }

        public virtual void LogicUpdate() { }

        public virtual void PhysicsUpdate() => DoChecks();

        public virtual void DoChecks() { }

        public virtual void TriggerEnter(Collider other) { }
    
        public virtual void TriggerExit(Collider other) { }

        public virtual void AnimationTrigger() { }

        public virtual void AnimationFinishTrigger() => IsAnimationFinished = true;
    }
}
