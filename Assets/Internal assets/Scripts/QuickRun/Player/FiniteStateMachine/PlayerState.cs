using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine;
using UnityEngine;
using UnityEngine.Serialization;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine
{
    public class PlayerState : MonoBehaviour
    {
        public PlayerStatistic playerStatistic;
        
        protected PlayerStateController StateController;
        protected PlayerStateMachine StateMachine;
        
        protected bool IsAnimationFinished;
        protected bool IsExitingState;

        protected float StartTime;

        private string _animBoolName;

        public PlayerState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName)
        {
            this.StateController = stateController;
            this.StateMachine = stateMachine;
            this.playerStatistic = playerStatistic;
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

        protected virtual void DoChecks() { }
    
        public virtual void TriggerEnter(Collider other) { }
    
        public virtual void TriggerExit(Collider other) { }

        public virtual void AnimationTrigger() { }

        public virtual void AnimationFinishTrigger() => IsAnimationFinished = true;
    }
}
