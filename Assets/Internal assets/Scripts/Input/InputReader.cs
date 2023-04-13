using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader")]
    public class InputReader : ScriptableObject, InputSystem.IGameplayActions, InputSystem.IUIActions
    {
        private InputSystem _inputSystem;

        private void OnEnable()
        {
            if (_inputSystem == null)
            {
                _inputSystem = new InputSystem();
                _inputSystem.Gameplay.SetCallbacks(this);
                _inputSystem.UI.SetCallbacks(this);

                SetGameplay();
            }
        }

        #region Action Maps

        public void SetGameplay()
        {
            _inputSystem.Gameplay.Enable();
            _inputSystem.UI.Disable();
        }

        public void SetUI()
        {
            _inputSystem.UI.Enable();
            _inputSystem.Gameplay.Disable();
        }

        #endregion

        #region Actions

        //Gameplay
        public event Action<Vector2> LookEvent;
        public event Action<Vector2> MoveEvent;
        public event Action AttackEvent;
        public event Action AttackCancelledEvent;
        public event Action AttackSuperEvent;
        public event Action AttackSuperCancelledEvent;
        public event Action AttackMagicEvent;
        public event Action AttackMagicCancelledEvent;
        public event Action SprintEvent;
        public event Action SprintCancelledEvent;
        public event Action InteractEvent;
        public event Action InteractCancelledEvent;
        public event Action XRTrackingArmLeftEvent;
        public event Action XRTrackingArmLeftCancelledEvent;
        public event Action XRTrackingArmRightEvent;
        public event Action XRTrackingArmRightCancelledEvent;
        public event Action XRGripLeftEvent;
        public event Action XRGripLeftCancelledEvent;
        public event Action XRGripRightEvent;
        public event Action XRGripRightCancelledEvent;
        public event Action XRActionLeftEvent;
        public event Action XRActionLeftCancelledEvent;
        public event Action XRActionRightEvent;
        public event Action XRActionRightCancelledEvent;


        public event Action PauseEvent;

        //UI
        public event Action ResumeEvent;

        #endregion

        #region Actions Invoke

        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                AttackEvent?.Invoke();
            else
                AttackCancelledEvent?.Invoke();
        }

        public void OnAttackSuper(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                AttackSuperEvent?.Invoke();
            else
                AttackSuperCancelledEvent?.Invoke();
        }

        public void OnAttackMagic(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                AttackMagicEvent?.Invoke();
            else
                AttackMagicCancelledEvent?.Invoke();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                SprintEvent?.Invoke();
            else
                SprintCancelledEvent?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                InteractEvent?.Invoke();
            else
                InteractCancelledEvent?.Invoke();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            PauseEvent?.Invoke();
            SetUI();
        }

        public void OnTrackingHandLeft(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                XRTrackingArmLeftEvent?.Invoke();
            else
                XRTrackingArmLeftCancelledEvent?.Invoke();
        }

        public void OnTrackingHandRight(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                XRTrackingArmRightEvent?.Invoke();
            else
                XRTrackingArmRightCancelledEvent?.Invoke();
        }

        public void OnGripLeft(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                XRGripLeftEvent?.Invoke();
            else
                XRGripLeftCancelledEvent?.Invoke();
        }

        public void OnGripRight(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                XRGripRightEvent?.Invoke();
            else
                XRGripRightCancelledEvent?.Invoke();
        }

        public void OnTriggerLeft(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                XRActionLeftEvent?.Invoke();
            else
                XRActionLeftCancelledEvent?.Invoke();
        }

        public void OnTriggerRight(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                XRActionRightEvent?.Invoke();
            else
                XRActionRightCancelledEvent?.Invoke();
        }

        public void OnResume(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            ResumeEvent?.Invoke();
            SetGameplay();
        }

        #endregion
    }
}