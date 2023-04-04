using Manager;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerDeathState : SuperState.PlayerAbilityState
    {
        public PlayerDeathState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ManagerScene.Instance.SwitchScene(SceneType.Home);
        }
    }
}