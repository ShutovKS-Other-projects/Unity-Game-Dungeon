using Enemy.FiniteStateMachine;
using Interactable;
using UnityEngine;
namespace Player.FiniteStateMachine.SuperState
{
    public class PlayerGroundedState : PlayerState
    {
        #region Variables

        protected Vector2 MovementInput;
        protected float RecoveryStaminaTime = 0;

        protected bool CrouchInput;
        protected bool RunInput;
        protected bool IsTouchingCelling;

        private bool _attackInput;
        private bool _blockInput;
        private bool _jumpInput;
        private bool _interactInput;

        private bool _isInteractable;
        private bool _isGrounded;

        private float _recoveryHealthTime = 0;

        #endregion

        public PlayerGroundedState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        #region StateMachine

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
                RecoveryStaminaTime = 0;
            }
            else if (_blockInput && !IsTouchingCelling)
            {
                StateMachine.ChangeState(StateController.BlockState);
                RecoveryStaminaTime = 0;
            }
            else if (_jumpInput && !IsTouchingCelling)
            {
                StateMachine.ChangeState(StateController.JumpState);
                RecoveryStaminaTime = 0;
            }
            else if (_isInteractable && _interactInput)
            {
                StateMachine.ChangeState(StateController.InteractState);
            }
            else if (!_isGrounded)
            {
                StateMachine.ChangeState(StateController.InAirState);
                RecoveryStaminaTime = 0;
            }
        }

        public override void TriggerEnter(Collider other)
        {
            base.TriggerEnter(other);
            if (other.gameObject.CompareTag($"EnemyDamageObject"))
            {
                if (other.transform.parent.TryGetComponent(out EnemyStateController enemyStateController))
                    PlayerStatistic.Health -= enemyStateController.StrengthAttackFloat!();
                StateMachine.ChangeState(StateController.DamageState);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            //RecoveryHealth();
            RecoveryStamina();
            StateController.Rotation();
        }

        #endregion

        #region Check Methods

        private bool CheckVisibleIfInteractable()
        {
            var cameraTransform = UnityEngine.Camera.main!.transform;
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.SphereCast(ray, PlayerStatistic.InterCheckSphereRadius, out var hitInfo, PlayerStatistic.InterCheckDistance, LayerMask.GetMask("Interactable")))
            {
                var interactable = hitInfo.collider.GetComponent<InteractableBase>();

                if (interactable != null)
                    if (PlayerStatistic.interactionData.IsEmpy() || PlayerStatistic.interactionData.IsSameInteractable(interactable))
                    {
                        PlayerStatistic.interactionData.Interactable = interactable;
                        StateController.uiInteractionBare.SetTooltipText(interactable.TooltipText);

                        return true;
                    }
            }
            PlayerStatistic.interactionData.ResetData();
            StateController.uiInteractionBare.SetTooltipText(" ");
            return false;
        }

        #endregion

        #region Recovery Methods

        void RecoveryStamina()
        {
            if (PlayerStatistic.Stamina >= PlayerStatistic.StaminaMax)
            {
                PlayerStatistic.Stamina = PlayerStatistic.StaminaMax;
            }
            else if (RecoveryStaminaTime > 5f)
            {
                if (PlayerStatistic.IsFatigue)
                {
                    PlayerStatistic.Stamina += PlayerStatistic.StaminaRecoverySpeedIsFatigue * Time.deltaTime;
                    if (PlayerStatistic.Stamina >= PlayerStatistic.StaminaMax / 4)
                    {
                        PlayerStatistic.IsFatigue = false;
                    }
                }
                else
                {
                    PlayerStatistic.Stamina += PlayerStatistic.StaminaRecoverySpeed * Time.deltaTime;
                }
            }
            else
            {
                RecoveryStaminaTime += Time.deltaTime;
            }
        }

        private void RecoveryHealth()
        {
            if (PlayerStatistic.Health >= PlayerStatistic.HealthMax)
            {
                PlayerStatistic.Health = PlayerStatistic.HealthMax;
            }
            else if (_recoveryHealthTime >= 10f)
            {
                PlayerStatistic.Health += PlayerStatistic.HealthRecoverySpeed * Time.deltaTime;
            }
            else
            {
                RecoveryStaminaTime += Time.deltaTime;
            }
        }

        #endregion
    }
}
