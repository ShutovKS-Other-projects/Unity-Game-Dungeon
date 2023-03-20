using Old.Player.FiniteStateMachine.SuperState;
using UnityEngine.SceneManagement;

namespace Old.Player.FiniteStateMachine.SubState
{
    public class PlayerDeathState : PlayerAbilityState
    {
        public PlayerDeathState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            SceneManager.LoadScene("Menu");
        }
    }
}
