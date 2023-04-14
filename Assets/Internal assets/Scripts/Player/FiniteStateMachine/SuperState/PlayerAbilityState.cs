using UnityEngine;

namespace Player.FiniteStateMachine.SuperState
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

        public override void Enter()
        {
            base.Enter();

            IsAbilityDone = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (IsAbilityDone) StateMachine.ChangeState(StateController.IdleState);
        }
    }
}