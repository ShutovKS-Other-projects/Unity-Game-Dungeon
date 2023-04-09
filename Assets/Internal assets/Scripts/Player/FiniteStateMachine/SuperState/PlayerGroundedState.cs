using Interactable;
using Manager;
using Scene;
using UnityEngine;

namespace Player.FiniteStateMachine.SuperState
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

        private GameObject _gameObjectInteractable;

        private bool _attackInput;
        private bool _attackSuperInput;
        private bool _jumpInput;
        private bool _isInteractable;
        private bool _isGrounded;
        private bool _interactInput;
        private bool _magicAttackInput;

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
            _attackInput = ManagerInput.Instance.GetPlayerAttackInput();
            _attackSuperInput = ManagerInput.Instance.GetPlayerAttackSuperInput();
            _interactInput = ManagerInput.Instance.GetPlayerInteractInput();
            _jumpInput = ManagerInput.Instance.GetPlayerJumpInput();
            _magicAttackInput = ManagerInput.Instance.GetPlayerMagicAttackInput();
            _isInteractable = CheckVisibleIfInteractable();
            
            
            if (_attackInput && SceneController.currentSceneType != SceneType.Home)
            {
                StateMachine.ChangeState(StateController.AttackState);
                RecoveryStaminaTime = 0;
            }
            else if (_magicAttackInput && SceneController.currentSceneType != SceneType.Home)
            {
                StateMachine.ChangeState(StateController.AttackMagicState);
                RecoveryStaminaTime = 0;
            }
            else if (_attackSuperInput && SceneController.currentSceneType != SceneType.Home)
            {
                StateMachine.ChangeState(StateController.AttackSuperState);
                RecoveryStaminaTime = 0;
            }
            else if (_isInteractable && _interactInput)
            {
                StateMachine.ChangeState(StateController.InteractState);
            }
        }

        public override void TriggerEnter(Collider other)
        {
            base.TriggerEnter(other);
            if (!other.gameObject.CompareTag($"EnemyDamageObject")) return;
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
            UIInteractionBare.Instance.SetTooltipText(" ");
            return false;
        }

        #endregion
    }
}