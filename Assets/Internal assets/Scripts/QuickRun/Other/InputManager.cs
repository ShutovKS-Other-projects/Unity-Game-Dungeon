using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Vector2 GetPlayerMovementInput() => _inputSystemGame.Player.Movement.ReadValue<Vector2>();
    public Vector2 GetLookInput() => _inputSystemGame.Player.Look.ReadValue<Vector2>();

    public bool GetPlayerAttackInput() => _inputSystemGame.Player.Attack.triggered;
    public bool GetPlayerBlockInput() => _inputSystemGame.Player.Block.triggered;
    public bool GetPlayerInteractInput() => _inputSystemGame.Player.Interact.triggered;
    public bool GetPlayerMenuInput() => _inputSystemGame.Player.Menu.triggered;
    public bool GetPlayerJumpInput() => _inputSystemGame.Player.Jump.triggered;
    public bool GetPlayerSprintInput() => _inputSystemGame.Player.Sprint.triggered;
}
