using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStatistic statistic;
    private InputManager _inputManager;
    private Rigidbody _rigidbody;
    private Transform _playerTransform;
    private Transform _cameraTransform;

    private float _yRotation = 0f;
    private float _xRotation = 0f;
    private float _mouseSensitivity = 50;

    private void Awake()
    {
        _playerTransform = transform;
        _cameraTransform = Camera.main.transform;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        statistic = new PlayerStatistic();
        _inputManager = InputManager.Instance;
    }

    private void Update()
    {if (statistic.isDead) return;

        Rotation();
        Movement();

        Jump();
        Attack();
        Block();
    }


    private void Movement()
    {
        Vector2 movementInput = _inputManager.GetPlayerMovementInput();

        statistic.Movement = movementInput.y * Running();
        _rigidbody.AddRelativeForce(new Vector3(movementInput.x, 0, movementInput.y) * statistic.Speed * Running());

        float Running()
        {
            switch (statistic.Stamina, statistic.isFatigue, _inputManager.GetPlayerSprintInput())
            {
                case ( > 0, false, true):
                    statistic.Stamina -= 2.5f * Time.deltaTime;
                    return statistic.Acceleration = 1.25f;
                case ( < 100, false, false):
                    if (statistic.Stamina == 0)
                    {
                        statistic.isFatigue = true;
                    }
                    statistic.Stamina += 5f * Time.deltaTime;
                    return statistic.Acceleration = 1f;
                case (100, true || false, false || true):
                    statistic.Stamina = 100;
                    return statistic.Acceleration = 1f;
                case ( > 0, true, false):
                    statistic.Stamina += 2f * Time.deltaTime;
                    if (statistic.Stamina > 15f)
                    {
                        statistic.isFatigue = false;
                    }
                    return statistic.Acceleration = 0.9f;
                default:
                    return statistic.Acceleration = 1f;
            }
        }
    }
    private void Rotation()
    {
        Vector2 lookInput = _inputManager.GetLookInput();

        _yRotation += lookInput.x * _mouseSensitivity * Time.deltaTime;
        _xRotation -= lookInput.y * _mouseSensitivity * Time.deltaTime;

        _xRotation = Mathf.Clamp(_xRotation, -70, 70f);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _playerTransform.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
        _rigidbody.MoveRotation(Quaternion.Euler(0f, _yRotation, 0f));
    }
    private void Jump()
    {
        if (_inputManager.GetPlayerJumpInput() && !statistic.isJump && statistic.Stamina > 10)
        {
            _rigidbody.AddForce(Vector3.up * statistic.JumpForce);
        }
    }
    private void Attack()
    {
        if (_inputManager.GetPlayerAttackInput() && !statistic.isAttack && statistic.Stamina > 10)
        {
            statistic.isAttack = true;
        }
    }
    private void Block()
    {
        if (_inputManager.GetPlayerBlockInput() && !statistic.isBlock)
        {
            statistic.isBlock = true;
        }
    }

}