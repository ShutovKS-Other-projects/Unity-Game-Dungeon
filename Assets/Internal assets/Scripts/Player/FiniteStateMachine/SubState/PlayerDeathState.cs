using Manager;
using Scene;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerDeathState : PlayerState
    {
        public PlayerDeathState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            SceneController.SwitchScene(SceneType.Home);
        }
    }
}