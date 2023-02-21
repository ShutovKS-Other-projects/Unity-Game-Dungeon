using Interactable;
using UnityEngine;
namespace Player.FiniteStateMachine.SuperState
{
    public class PlayerGroundedState : PlayerState
    {
        protected Vector2 MovementInput;

        protected bool CrouchInput;
        protected bool RunInput;
        protected bool IsTouchingCelling;

        private bool _attackInput;
        private bool _blockInput;
        private bool _jumpInput;
        private bool _interactInput;
        
        private bool _isInteractable;
        private bool _isGrounded;

        public PlayerGroundedState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        protected override void DoChecks()
        {
            base.DoChecks();

            _isGrounded = StateController.CheckIfGrounded();
            IsTouchingCelling = StateController.CheckIfTouchingCelling();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            CrouchInput = StateController.InputManager.GetPlayerCrouchInput();
            RunInput = StateController.InputManager.GetPlayerSprintInput();
            MovementInput = StateController.InputManager.GetPlayerMovementInput();
            _attackInput = StateController.InputManager.GetPlayerAttackInput();
            _blockInput = StateController.InputManager.GetPlayerBlockInput();
            _interactInput = StateController.InputManager.GetPlayerInteractInput();
            _jumpInput = StateController.InputManager.GetPlayerJumpInput();
            _isInteractable = CheckVisibleIfInteractable();

            if (_attackInput && !IsTouchingCelling)
            {
                StateMachine.ChangeState(StateController.AttackState);
            }
            else if (_blockInput && !IsTouchingCelling)
            {
                StateMachine.ChangeState(StateController.BlockState);
            }
            else if (_jumpInput && !IsTouchingCelling)
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

        //public override void TriggerEnter(Collider other)
        //{
        //    if (other.gameObject.CompareTag("Enemy"))
        //    {
        //        StateMachine.ChangeState(StateController.DamageState);
        //    }
        //}

        // ReSharper disable Unity.PerformanceAnalysis
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            RecoveryHealth();
            RecoveryStamina();
            StateController.Rotation();
        }

        #region Check Methods

        private bool CheckVisibleIfInteractable()
        {
            var cameraTransform = UnityEngine.Camera.main!.transform;
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);
        
            if (Physics.SphereCast(ray, playerStatistic.InterCheckSphereRadius, out var hitInfo, playerStatistic.InterCheckDistance, LayerMask.GetMask("Interactable")))
            {
                var interactable = hitInfo.collider.GetComponent<InteractableBase>();

                if (interactable != null)
                {
                    if (playerStatistic.InteractionData.IsEmpy() || playerStatistic.InteractionData.IsSameInteractable(interactable))
                    {
                        playerStatistic.InteractionData.Interactable = interactable;
                        StateController.uiInteractionBare.SetTooltipText(interactable.TooltipText);

                        return true;
                    }
                    else
                    {
                        playerStatistic.InteractionData.ResetData();
                        StateController.uiInteractionBare.SetTooltipText("");

                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                playerStatistic.InteractionData.ResetData();
                StateController.uiInteractionBare.SetTooltipText("");
                return false;
            }
        }

        #endregion

        #region Other Methods

        private void RecoveryStamina()
        {
            if (playerStatistic.Stamina >= playerStatistic.StaminaMax)
            {
                playerStatistic.Stamina = playerStatistic.StaminaMax;
            }
            else
            {
                if (playerStatistic.IsFatigue)
                {
                    playerStatistic.Stamina += playerStatistic.StaminaRecoverySpeedIsFatigue * Time.deltaTime;
                    if (playerStatistic.Stamina >= playerStatistic.StaminaMax / 4)
                    {
                        playerStatistic.IsFatigue = false;
                    }
                }
                else
                {
                    playerStatistic.Stamina += playerStatistic.StaminaRecoverySpeed * Time.deltaTime;
                }
            }
        }

        private void RecoveryHealth()
        {
            if (playerStatistic.Health >= playerStatistic.HealthMax)
            {
                playerStatistic.Health = playerStatistic.HealthMax;
            }
            else
            {
                playerStatistic.Health += playerStatistic.HealthRecoverySpeed * Time.deltaTime;
            }
        }

        #endregion
    }
}
