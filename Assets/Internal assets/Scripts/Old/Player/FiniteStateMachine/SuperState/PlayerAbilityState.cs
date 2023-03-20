using UnityEngine;

namespace Old.Player.FiniteStateMachine.SuperState
{
    public class PlayerAbilityState : PlayerState
    {

        public PlayerAbilityState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        protected Vector2 MovementInput;

        protected bool IsAbilityDone;
        protected bool IsGrounded;

        protected override void DoChecks()
        {
            base.DoChecks();

            IsGrounded = StateController.CheckIfGrounded();
        }

        public override void Enter()
        {
            base.Enter();

            IsAbilityDone = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsAbilityDone)
                return;

            if (IsGrounded && StateController.Rb.velocity.y < 0.01f)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
            else
            {
                StateMachine.ChangeState(StateController.InAirState);
            }
        }
    }
}