using Manager;
using UnityEngine;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerAttackState : SuperState.PlayerAbilityState
    {
        public PlayerAttackState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            ManagerWeapon.Instance.OnSwitchColliderWeapon(true);
            SetTransformTarget(new Vector3(-0.5f, 0.75f, -0.25f), new Quaternion(-0.5f, 0.75f, -0.25f, 0f));

            // if (Random.Range(0, 101) < PlayerStatistic.CharacteristicCriticalChance.Value)
                // StateController.RegisterDelegateStrengthAttackFloat(AttackCritical);
            // else
                // StateController.RegisterDelegateStrengthAttackFloat(Attack);
        }

        public override void Exit()
        {
            base.Exit();


            SetTransformTargetZero();
            ManagerWeapon.Instance.OnSwitchColliderWeapon(false);
            // StateController.RegisterDelegateStrengthAttackFloat(AttackZero);
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAbilityDone = true;
        }


        private float Attack() => PlayerStatistic.CharacteristicStrength.Value;

        private float AttackCritical() => PlayerStatistic.CharacteristicStrength.Value *
                                          (1 + PlayerStatistic.CharacteristicCriticalAttack.Value / 100);

        private static float AttackZero() => 0;

        private static void SetTransformTarget(Vector3 position, Quaternion rotation)
        {
            ManagerRig.Instance.lHandTargetTransform.localPosition = position;
            ManagerRig.Instance.lHandTargetTransform.localRotation = rotation;
        }

        private static void SetTransformTargetZero()
        {
            ManagerRig.SetTransformTargetZero(ManagerRig.Instance.lHandTargetTransform);
        }
    }
}