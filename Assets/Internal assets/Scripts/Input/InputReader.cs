using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

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
        public event Action XRTriggeredHandLeftEvent;
        public event Action XRTriggeredHandLeftCancelledEvent;
        public event Action XRTriggeredHandRightEvent;
        public event Action XRTriggeredHandRightCancelledEvent;
        public event Action PauseEvent;

        //UI
        public event Action ResumeEvent;

        //Scheme

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

        public void OnXRTriggeredHandLeft(InputAction.CallbackContext context)
        {
            if (context.ReadValue<int>() != 0)
                XRTriggeredHandLeftEvent?.Invoke();
            else
                XRTriggeredHandLeftCancelledEvent?.Invoke();
        }

        public void OnXRTriggeredHandRight(InputAction.CallbackContext context)
        {
            if (context.ReadValue<int>() != 0)
                XRTriggeredHandRightEvent?.Invoke();
            else
                XRTriggeredHandRightCancelledEvent?.Invoke();
        }

        public void OnResume(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            ResumeEvent?.Invoke();
            SetGameplay();
        }
    }
}