﻿using Manager;
using Rig;
using UnityEngine;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerAttackMagicState : SuperState.PlayerAbilityState
    {
        public PlayerAttackMagicState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        private bool _isMagicAttack;

        public override void Enter()
        {
            base.Enter();

            _isMagicAttack = false;
            SetTransformTarget(new Vector3(0.023f, 1.242f, 0.677f), new Quaternion(119.012f, -237.792f, -234.372f, 0f));
        }

        public override void Exit()
        {
            base.Exit();

            SetTransformTargetZero();
            // StateController.RegisterDelegateStrengthAttackFloat(AttackZero);
        }

        public override void AnimationTrigger()
        {
            base.AnimationTrigger();

            // SetTransformTarget(new Vector3(0.115f, 1.098f, 0.517f), new Quaternion(-44f, -85.01f, -78.862f, 0f));
            if (!_isMagicAttack)
            {
                // StateController.MagicAttackDelegate!.Invoke();
                // if (Random.Range(0, 101) < PlayerStatistic.characteristicCriticalChance.Value)
                // StateController.RegisterDelegateStrengthAttackFloat(CriticalMagicAttack);
                // else
                // StateController.RegisterDelegateStrengthAttackFloat(MagicAttack);
                _isMagicAttack = true;
            }
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();
            IsAbilityDone = true;
        }

        private float CriticalMagicAttack() =>
            PlayerStatistic.CharacteristicStrengthMagic.Value *
            (1 + PlayerStatistic.CharacteristicCriticalAttack.Value / 100);

        private float MagicAttack() => PlayerStatistic.CharacteristicStrengthMagic.Value;
        private static float AttackZero() => 0;

        private static void SetTransformTarget(Vector3 position, Quaternion rotation)
        {
            RigController.rArmTargetTransform.localPosition = position;
            RigController.rArmTargetTransform.localRotation = rotation;
        }

        private static void SetTransformTargetZero()
        {
            RigController.SetTransformTargetZero(RigController.rArmTargetTransform);
        }
    }
}