using ManagerRig;
using UnityEngine;

namespace Player.Game.FiniteStateMachine.SubState
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

            StateController.SwitchCollider?.Invoke(true);

            SetTransformTarget(new Vector3(-0.5f, 0.75f, -0.25f), new Quaternion(-0.5f, 0.75f, -0.25f, 0f));

            if (Random.Range(0, 101) < PlayerStatistic.CharacteristicCriticalChance.Value)
                StateController.RegisterDelegateStrengthAttackFloat(AttackCritical);
            else
                StateController.RegisterDelegateStrengthAttackFloat(Attack);
        }

        public override void Exit()
        {
            base.Exit();

            StateController.SwitchCollider?.Invoke(false);

            SetTransformTargetZero();

            StateController.RegisterDelegateStrengthAttackFloat(AttackZero);
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
            ManagerRigGame.Instance.lHandTargetTransform.localPosition = position;
            ManagerRigGame.Instance.lHandTargetTransform.localRotation = rotation;
        }

        private static void SetTransformTargetZero() =>
            ManagerRigGame.Instance.SetTransformTargetZero(ManagerRigGame.Instance.lHandTargetTransform);
    }
}