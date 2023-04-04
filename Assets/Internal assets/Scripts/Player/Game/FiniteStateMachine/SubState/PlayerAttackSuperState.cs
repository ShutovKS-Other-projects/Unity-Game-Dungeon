using Manager;
using Weapon;

namespace Player.Game.FiniteStateMachine.SubState
{
    public class PlayerAttackSuperState : SuperState.PlayerAbilityState
    {
        public PlayerAttackSuperState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            ManagerWeapon.Instance.OnSwitchTriggerColliderWeapon(true);
        }

        public override void Exit()
        {
            base.Exit();

            ManagerWeapon.Instance.OnSwitchTriggerColliderWeapon(false);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!ManagerInput.Instance.GetPlayerBlockInput())
            {
                IsAbilityDone = true;
            }
            else if (ManagerInput.Instance.GetPlayerAttackInput())
            {
                StateMachine.ChangeState(StateController.AttackState);
            }
        }
    }
}