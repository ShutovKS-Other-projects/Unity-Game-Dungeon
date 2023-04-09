using Manager;
using Rig;
using UnityEngine;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerInteractState : SuperState.PlayerAbilityState
    {
        public PlayerInteractState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        private bool _isInteracting;

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityZero();
            SetTransformTarget(PlayerStatistic.Instance.interactionTransform,
                new Quaternion(63.077f, -72.323f, -33.995f, 0));
        }

        public override void Exit()
        {
            base.Exit();

            PlayerStatistic.interactionObject.ResetData();
            _isInteracting = false;
            SetTransformTargetZero();
        }

        public override void AnimationTrigger()
        {
            base.AnimationTrigger();

            if (_isInteracting) return;
            RigController.rHand.weight = 0;
            RigController.lHand.weight = 0;
            PlayerStatistic.interactionObject.Interact();
            _isInteracting = true;
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAbilityDone = true;
        }


        private static void SetTransformTarget(Transform transform, Quaternion rotation)
        {
            RigController.rHandTargetTransform.localPosition = transform.position;
            RigController.rHandTargetTransform.localRotation = rotation;
        }

        private static void SetTransformTargetZero()
        {
            RigController.SetTransformTargetZero(RigController.rHandTargetTransform);
        }
    }
}