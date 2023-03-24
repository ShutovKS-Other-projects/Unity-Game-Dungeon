using Random = UnityEngine.Random;

namespace Player.Game.FiniteStateMachine.SubState
{
    public class PlayerMagicAttackState : SuperState.PlayerAbilityState
    {
        public PlayerMagicAttackState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }


        public override void Enter()
        {
            base.Enter();

            // StateController.MagicAttackDelegate!.Invoke();
            // if (Random.Range(0, 101) < PlayerStatistic.CriticalChance)
                // StateController.RegisterDelegateStrengthAttackFloat(CriticalMagicAttack);
            // else
                // StateController.RegisterDelegateStrengthAttackFloat(MagicAttack);
        }

        public override void Exit()
        {
            base.Exit();

            StateController.RegisterDelegateStrengthAttackFloat(AttackZero);
        }


        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();
            IsAbilityDone = true;
        }

        private float CriticalMagicAttack() =>
            PlayerStatistic.MagicAttackDamage * (1 + PlayerStatistic.CriticalDamage / 100);

        private float MagicAttack() => PlayerStatistic.MagicAttackDamage;
        private static float AttackZero() => 0;
    }
}