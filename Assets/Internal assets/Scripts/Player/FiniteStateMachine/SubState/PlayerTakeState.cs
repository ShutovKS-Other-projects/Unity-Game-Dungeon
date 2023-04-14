using Player.FiniteStateMachine.SuperState;
using Rig;
using UnityEngine;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerTakeState : PlayerAbilityState
    {
        public PlayerTakeState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }
        
        private bool _isTake;

        public override void Enter()
        {
            base.Enter();

            SetTransformTarget(PlayerStatistic.Instance.interactionTransform,
                new Quaternion(63.077f, -72.323f, -33.995f, 0));
        }

        public override void Exit()
        {
            base.Exit();

            _isTake = false;
            SetTransformTargetZero();
        }

        public override void AnimationTrigger()
        {
            base.AnimationTrigger();

            if (_isTake) return;
            RigController.rArm.weight = 0;
            RigController.lArm.weight = 0;
            _isTake = PlayerStatistic.interactionObject.TryTake();
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAbilityDone = true;
        }

        private static void SetTransformTarget(Transform transform, Quaternion rotation)
        {
            RigController.rArmTargetTransform.localPosition = transform.position;
            RigController.rArmTargetTransform.localRotation = rotation;
        }

        private static void SetTransformTargetZero()
        {
            RigController.SetTransformTargetZero(RigController.rArmTargetTransform);
        }
    }
}