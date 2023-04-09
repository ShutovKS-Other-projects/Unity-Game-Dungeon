using Manager;
using Weapon;

namespace Player.FiniteStateMachine.SubState
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

            WeaponController.OnSwitchColliderWeapon(true);
        }

        public override void Exit()
        {
            base.Exit();

            WeaponController.OnSwitchColliderWeapon(false);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!ManagerInput.Instance.GetPlayerAttackSuperInput())
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