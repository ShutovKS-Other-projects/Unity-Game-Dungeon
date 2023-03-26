using UnityEngine;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private InputSystemGame _inputSystemGame;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            _inputSystemGame = new InputSystemGame();
        }


        private void OnEnable()
        {
            _inputSystemGame.Enable();
        }

        private void OnDisable()
        {
            _inputSystemGame.Disable();
        }


        //UI
        public bool GetAllMenuInput() => _inputSystemGame.UI.Menu.triggered;
        public bool GetAllPlayerInfoInput() => _inputSystemGame.UI.PlayerInfo.triggered;

        //Game
        public Vector2 GetPlayerMovementInput() => _inputSystemGame.Player.Movement.ReadValue<Vector2>();
        public Vector2 GetLookInput() => _inputSystemGame.Player.Look.ReadValue<Vector2>();

        public bool GetPlayerSprintInput() => _inputSystemGame.Player.Sprint.inProgress;
        public bool GetPlayerBlockInput() => _inputSystemGame.Player.Block.inProgress;
        public bool GetPlayerCrouchInput() => _inputSystemGame.Player.Crouch.inProgress;

        public bool GetPlayerAttackInput() => _inputSystemGame.Player.Attack.triggered;
        public bool GetPlayerMagicAttackInput() => _inputSystemGame.Player.MagicAttack.triggered;
        public bool GetPlayerJumpInput() => _inputSystemGame.Player.Jump.triggered;
        public bool GetPlayerInteractInput() => _inputSystemGame.Player.Interact.triggered;
    }
}