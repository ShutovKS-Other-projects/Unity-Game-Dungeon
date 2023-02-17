using Internal_assets.Scripts.QuickRun.Interactable;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState
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

        public PlayerGroundedState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(stateController, stateMachine, playerData, animBoolName)
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

            RecoveryStamina();
            StateController.Rotation();
        }

        #region Check Methods

        private bool CheckVisibleIfInteractable()
        {
            var cameraTransform = UnityEngine.Camera.main!.transform;
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);
        
            if (Physics.SphereCast(ray, playerData.interCheckSphereRadius, out var hitInfo, playerData.interCheckDistance, LayerMask.GetMask("Interactable")))
            {
                var interactable = hitInfo.collider.GetComponent<InteractableBase>();

                if (interactable != null)
                {
                    if (playerData.interactionData.IsEmpy() || playerData.interactionData.IsSameInteractable(interactable))
                    {
                        playerData.interactionData.Interactable = interactable;
                        StateController.uiInteractionBare.SetTooltipText(interactable.TooltipText);

                        return true;
                    }
                    else
                    {
                        playerData.interactionData.ResetData();
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
                playerData.interactionData.ResetData();
                StateController.uiInteractionBare.SetTooltipText("");
                return false;
            }
        }

        #endregion

        #region Other Methods

        private void RecoveryStamina()
        {
            if (playerData.stamina >= playerData.maxStamina)
            {
                playerData.stamina = playerData.maxStamina;
            }
            else
            {
                if (playerData.isFatigue)
                {
                    playerData.stamina += playerData.staminaRecoverySpeedIsFatigue * Time.deltaTime;
                    if (playerData.stamina >= playerData.maxStamina / 4)
                    {
                        playerData.isFatigue = false;
                    }
                }
                else
                {
                    playerData.stamina += playerData.staminaRecoverySpeed * Time.deltaTime;
                }
            }
        }

        private void RecoveryHealth()
        {
            if (playerData.health >= playerData.maxHealth)
            {
                playerData.health = playerData.maxHealth;
            }
            else
            {
                playerData.health += playerData.healthRecoverySpeed * Time.deltaTime;
            }
        }

        #endregion
    }
}
