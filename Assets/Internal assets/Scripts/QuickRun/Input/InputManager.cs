using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Input
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;

        public static InputManager Instance { get { return _instance; } }

        private InputSystemGame _inputSystemGame;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
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


        //All
        public bool GetAllMenuInput() => _inputSystemGame.All.Menu.triggered;
        public bool GetAllPlayerInfoInput() => _inputSystemGame.All.PlayerInfo.triggered;
    
        //Game
        public Vector2 GetPlayerMovementInput() => _inputSystemGame.Player.Movement.ReadValue<Vector2>();
        public Vector2 GetLookInput() => _inputSystemGame.Player.Look.ReadValue<Vector2>();

        public bool GetPlayerSprintInput() => _inputSystemGame.Player.Sprint.inProgress;
        public bool GetPlayerBlockInput() => _inputSystemGame.Player.Block.inProgress;
        public bool GetPlayerCrouchInput() => _inputSystemGame.Player.Crouch.inProgress;

        public bool GetPlayerAttackInput() => _inputSystemGame.Player.Attack.triggered;
        public bool GetPlayerJumpInput() => _inputSystemGame.Player.Jump.triggered;
        public bool GetPlayerInteractInput() => _inputSystemGame.Player.Interact.triggered;
    }
}
