using System;
using Input;
using Manager;
using Scene;
using UI.Home_Scene;
using UnityEngine;

namespace Skills.SkillsBook
{
    public class UISkillsBook : MonoBehaviour
    {
        private InputReader _inputReader;

        private void Awake()
        {
            _inputReader = Resources.Load<InputReader>($"ScriptableObject/Input/InputReader");
            _inputReader.MoveEvent += HandlerMovement;
            _inputReader.InteractEvent += HandlerInteract;
            _inputReader.InteractCancelledEvent += HandlerInteractCancelled;
        }
        
        public void LateUpdate()
        {
            if (MovementInput == Vector2.zero || InteractInput) return;
            SceneController.SwitchCursor(false);
            UIHomeSceneController.Instance.SwitchSkillsBookUI(false);
        }
        
        #region Handler Input

        private Vector2 MovementInput { get; set; }
        private bool InteractInput { get; set; }

        private void HandlerMovement(Vector2 value) => MovementInput = value;
        private void HandlerInteract() => InteractInput = true;
        private void HandlerInteractCancelled() => InteractInput = false;

        #endregion
    }
}