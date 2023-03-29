using UnityEngine;

namespace Player.Home.FiniteStateMachine.SubState
{
    public class PlayerInteractState : SuperState.PlayerAbilityState
    {
        public PlayerInteractState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public delegate Transform InteractTransform();

        public static InteractTransform OnInteractTransform;

        private Transform _interactTransform;
        private Transform _handIKTarget;
        private bool _isInteracting;

        public override void Enter()
        {
            base.Enter();

            _interactTransform = OnInteractTransform?.Invoke();
            Debug.Log(_interactTransform);
            _isInteracting = false;
            if (_handIKTarget == null)
                _handIKTarget = StateController.transform.Find("Rig Interaction").transform.Find("R_Hand (1)").transform.Find("R_Hand_Target");
            _handIKTarget.position = _interactTransform.position;
            StateController.SetVelocityZero();
        }

        public override void Exit()
        {
            base.Exit();

            PlayerStatistic.interactionData.ResetData();
        }

        public override void AnimationTrigger()
        {
            base.AnimationTrigger();

            if (_isInteracting) return;
            
            PlayerStatistic.interactionData.Interact();
            _isInteracting = true;
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAbilityDone = true;
        }
    }
}