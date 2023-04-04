using Interactable;
using Manager;
using Player.Home.FiniteStateMachine.SubState;
using UnityEngine;

namespace Player.Home.FiniteStateMachine.SuperState
{
    public class PlayerGroundedState : PlayerState
    {
        public PlayerGroundedState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        #region Variables

        protected Vector2 MovementInput;

        protected bool CrouchInput;
        protected bool RunInput;

        private bool _jumpInput;
        private bool _interactInput;

        private bool _isInteractable;
        private bool _isGrounded;

        #endregion

        #region StateMachine

        protected override void DoChecks()
        {
            base.DoChecks();

            _isGrounded = StateController.CheckIfGrounded();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            CrouchInput = ManagerInput.Instance.GetPlayerCrouchInput();
            RunInput = ManagerInput.Instance.GetPlayerSprintInput();
            MovementInput = ManagerInput.Instance.GetPlayerMovementInput();
            _interactInput = ManagerInput.Instance.GetPlayerInteractInput();
            _jumpInput = ManagerInput.Instance.GetPlayerJumpInput();
            _isInteractable = CheckVisibleIfInteractable();

            if (_jumpInput)
            {
                StateMachine.ChangeState(StateController.JumpState);
            }
            else if (_isInteractable && _interactInput)
            {
                StateMachine.ChangeState(StateController.InteractState);
            }
            else if (!_isGrounded)
            {
                StateMachine.ChangeState(StateController.InAirState);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            StateController.Rotation();
        }

        #endregion

        #region Check Methods

        private bool CheckVisibleIfInteractable()
        {
            var cameraTransform = UnityEngine.Camera.main!.transform;
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.SphereCast(ray, PlayerStatistic.InterCheckSphereRadius, out var hitInfo,
                    PlayerStatistic.InterCheckDistance, LayerMask.GetMask("Interactable")))
            {
                var interactable = hitInfo.collider.GetComponent<InteractableBase>();

                if (interactable != null)
                    if (PlayerStatistic.interactionObject.IsEmpty() ||
                        PlayerStatistic.interactionObject.IsSameInteractable(interactable))
                    {
                        PlayerStatistic.interactionObject.Interactable = interactable;
                        PlayerStatistic.Instance.interactionTransform = hitInfo.transform;
                        UIInteractionBare.Instance.SetTooltipText(interactable.TooltipText);

                        return true;
                    }
            }

            PlayerStatistic.interactionObject.ResetData();
            StateController.uiInteractionBare.SetTooltipText(" ");
            return false;
        }

        #endregion
    }
}