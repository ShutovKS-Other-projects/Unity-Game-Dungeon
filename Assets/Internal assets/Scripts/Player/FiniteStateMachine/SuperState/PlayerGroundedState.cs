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

        private GameObject _gameObjectInteractable;

        #endregion

        #region StateMachine

        // ReSharper disable Unity.PerformanceAnalysis
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            var isInteractable = CheckVisibleIfInteractable();

            if (StateController.AttackInput && SceneController.currentSceneType != SceneType.Home)
            {
                StateMachine.ChangeState(StateController.AttackState);
            }
            else if (StateController.AttackMagicInput && SceneController.currentSceneType != SceneType.Home)
            {
                StateMachine.ChangeState(StateController.AttackMagicState);
            }
            else if (StateController.AttackSuperInput && SceneController.currentSceneType != SceneType.Home)
            {
                StateMachine.ChangeState(StateController.AttackSuperState);
            }
            else if (isInteractable && StateController.InteractInput)
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