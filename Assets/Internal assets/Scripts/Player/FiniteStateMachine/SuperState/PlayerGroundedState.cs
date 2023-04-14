using Interactive;
using Manager;
using Scene;
using UI;
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

            if (SceneController.currentSceneType != SceneType.Home)
            {
                if (StateController.AttackInput)
                {
                    StateMachine.ChangeState(StateController.AttackState);
                }
                else if (StateController.AttackMagicInput)
                {
                    StateMachine.ChangeState(StateController.AttackMagicState);
                }
                else if (StateController.AttackSuperInput)
                {
                    StateMachine.ChangeState(StateController.AttackSuperState);
                }
            }
            else if (isInteractable)
            {
                if (StateController.InteractInput)
                {
                    StateMachine.ChangeState(StateController.InteractState);
                }
                else if (StateController.TakeInput)
                {
                    StateMachine.ChangeState(StateController.TakeState);
                }
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
                    PlayerStatistic.InterCheckDistance, LayerMask.GetMask($"Interactive")))
            {
                var interactive = hitInfo.collider.GetComponent<IInteractive>();

                if (interactive != null)
                    if (PlayerStatistic.interactionObject.IsEmpty() ||
                        PlayerStatistic.interactionObject.IsSameInteractable(interactive))
                    {
                        PlayerStatistic.interactionObject.Interactive = interactive;
                        PlayerStatistic.Instance.interactionTransform = hitInfo.transform;
                        UIInteractionBare.Instance.SetTooltipText(interactive.TooltipTextInteract + " " +
                                                                  interactive.TooltipTextTake);
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