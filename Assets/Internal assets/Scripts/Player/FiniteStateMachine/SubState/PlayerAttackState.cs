using System;
using Player.FiniteStateMachine.SuperState;
using UnityEngine;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerAttackState : PlayerAbilityState
    {
        public PlayerAttackState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            SwitchCollider(true);
            PlayerStatistic.Stamina -= 10;

            if (UnityEngine.Random.Range(0, 101) < PlayerStatistic.CriticalChance)
                StateController.RegisterDelegateStrengthAttackFloat(AttackCritical);
            else
                StateController.RegisterDelegateStrengthAttackFloat(Attack);
        }

        public override void Exit()
        {
            base.Exit();

            SwitchCollider(false);

            StateController.RegisterDelegateStrengthAttackFloat(AttackZero);
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAbilityDone = true;
        }

        private Delegate.SwitchCollider SwitchCollider => StateController.SwitchCollider;

        private float Attack() => PlayerStatistic.Strength;
        private float AttackCritical() => PlayerStatistic.Strength * (1 + PlayerStatistic.CriticalDamage / 100);
        private static float AttackZero() => 0;
    }
}