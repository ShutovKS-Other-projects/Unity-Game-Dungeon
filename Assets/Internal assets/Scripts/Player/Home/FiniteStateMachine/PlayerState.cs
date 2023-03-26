using UnityEngine;

namespace Player.Home.FiniteStateMachine
{
    public class PlayerState : MonoBehaviour
    {
        protected readonly PlayerStatistic PlayerStatistic;
        protected readonly PlayerStateController StateController;
        protected readonly PlayerStateMachine StateMachine;

        protected bool IsAnimationFinished;
        protected bool IsExitingState;

        protected float StartTime;

        private readonly string _animBoolName;

        public PlayerState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName)
        {
            this.StateController = stateController;
            this.StateMachine = stateMachine;
            this._animBoolName = animBoolName;
            this.PlayerStatistic = playerStatistic;
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

        public virtual void LogicUpdate()
        {
        }

        public virtual void PhysicsUpdate() => DoChecks();

        protected virtual void DoChecks()
        {
        }

        public virtual void TriggerEnter(Collider other)
        {
        }

        public virtual void TriggerExit(Collider other)
        {
        }

        public virtual void AnimationTrigger()
        {
        }

        public virtual void AnimationFinishTrigger() => IsAnimationFinished = true;
    }
}