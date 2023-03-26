using Input;

namespace Player.Game.FiniteStateMachine.SubState
{
    public class PlayerBlockState : SuperState.PlayerAbilityState
    {
        public PlayerBlockState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
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

            if (!InputManagerGame.Instance.GetPlayerBlockInput())
            {
                IsAbilityDone = true;
            }
            else if (InputManagerGame.Instance.GetPlayerAttackInput())
            {
                StateMachine.ChangeState(StateController.AttackState);
            }
        }
    }
}