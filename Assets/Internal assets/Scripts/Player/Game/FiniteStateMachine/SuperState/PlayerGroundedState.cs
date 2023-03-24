using Interactable;
using Old.Enemy.FiniteStateMachine;
using UnityEngine;

namespace Player.Game.FiniteStateMachine.SuperState
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
        protected float RecoveryStaminaTime = 0;
        protected float RecoveryHealthTime = 0;
        protected float RecoveryManaTime = 0;

        protected bool CrouchInput;
        protected bool RunInput;

        private bool _attackInput;
        private bool _blockInput;
        private bool _jumpInput;
        private bool _interactInput;
        private bool _magicAttackInput;

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

            CrouchInput = StateController.InputManagerGame.GetPlayerCrouchInput();
            RunInput = StateController.InputManagerGame.GetPlayerSprintInput();
            MovementInput = StateController.InputManagerGame.GetPlayerMovementInput();
            _attackInput = StateController.InputManagerGame.GetPlayerAttackInput();
            _blockInput = StateController.InputManagerGame.GetPlayerBlockInput();
            _interactInput = StateController.InputManagerGame.GetPlayerInteractInput();
            _jumpInput = StateController.InputManagerGame.GetPlayerJumpInput();
            _magicAttackInput = StateController.InputManagerGame.GetPlayerMagicAttackInput();
            _isInteractable = CheckVisibleIfInteractable();

            if (_attackInput)
            {
                StateMachine.ChangeState(StateController.AttackState);
                RecoveryStaminaTime = 0;
            }
            else if (_magicAttackInput)
            {
                StateMachine.ChangeState(StateController.MagicAttackState);
                RecoveryStaminaTime = 0;
            }
            else if (_blockInput)
            {
                StateMachine.ChangeState(StateController.BlockState);
                RecoveryStaminaTime = 0;
            }
            else if (_jumpInput)
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
            if (!other.gameObject.CompareTag($"EnemyDamageObject")) return;
            if (other.transform.parent.TryGetComponent(out EnemyStateController enemyStateController))
                PlayerStatistic.Health -= enemyStateController.StrengthAttackFloat!();
            StateMachine.ChangeState(StateController.DamageState);
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
            return false;
            var cameraTransform = UnityEngine.Camera.main!.transform;
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.SphereCast(ray, PlayerStatistic.InterCheckSphereRadius, out var hitInfo,
                    PlayerStatistic.InterCheckDistance, LayerMask.GetMask("Interactable")))
            {
                var interactable = hitInfo.collider.GetComponent<InteractableBase>();

                if (interactable != null)
                    if (PlayerStatistic.interactionData.IsEmpty() ||
                        PlayerStatistic.interactionData.IsSameInteractable(interactable))
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
    }
}