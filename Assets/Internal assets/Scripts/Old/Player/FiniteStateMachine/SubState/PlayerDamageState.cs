using Old.Player.FiniteStateMachine.SuperState;

namespace Old.Player.FiniteStateMachine.SubState
{
    public class PlayerDamageState : PlayerAbilityState
    {
        public PlayerDamageState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsAnimationFinished)
                return;

            if (PlayerStatistic.Health > 0)
                StateMachine.ChangeState(StateController.IdleState);
            else
                StateMachine.ChangeState(StateController.DeathState);
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAbilityDone = true;
        }
    }
}
