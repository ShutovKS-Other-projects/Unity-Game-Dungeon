using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Magic;
using Magic.SubMagic;
using Magic.SuperMagic;
using Magic.Type;
using Player.FiniteStateMachine.SuperState;
using Skill;
using Skill.Enum;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerMagicAttackState : PlayerAbilityState
    {
        public PlayerMagicAttackState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }


        public override void Enter()
        {
            base.Enter();

            StateController.MagicAttackDelegate!.Invoke();

            if (Random.Range(0, 101) < PlayerStatistic.CriticalChance)
                StateController.RegisterDelegateStrengthAttackFloat(CriticalMagicAttack);
            else
                StateController.RegisterDelegateStrengthAttackFloat(MagicAttack);
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