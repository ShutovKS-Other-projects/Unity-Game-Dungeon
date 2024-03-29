namespace Player.FiniteStateMachine.SubState
{
    public class PlayerDamageState : SuperState.PlayerAbilityState
    {
        public PlayerDamageState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsAnimationFinished)
                return;

            if (PlayerStatistic.CharacteristicHealth.Value > 0)
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