namespace Player.Game.FiniteStateMachine.SubState
{
    public class PlayerBlockState : SuperState.PlayerAbilityState
    {
        public PlayerBlockState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            //OnEnable trigger weapon collider
        }

        public override void Exit()
        {
            base.Exit();

            //OnDisable trigger weapon collider
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!StateController.InputManagerGame.GetPlayerBlockInput())
            {
                IsAbilityDone = true;
            }
            else if (StateController.InputManagerGame.GetPlayerAttackInput())
            {
                StateMachine.ChangeState(StateController.AttackState);
            }
        }
    }
}
