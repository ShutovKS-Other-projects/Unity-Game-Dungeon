using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStatistic statistic;
    private InputManager _inputManager;
    private Rigidbody _rigidbody;
    private Transform _playerTransform;
    private Transform _cameraTransform;

    private void Start()
    {
        statistic = new PlayerStatistic();
        _rigidbody = GetComponent<Rigidbody>();
        _inputManager = InputManager.Instance;
        _cameraTransform = Camera.main.transform;
        _playerTransform = transform;
    }

    private void Update()
    {
        if (statistic.isDead) return;
        Rotat();
        Movement();
        Jump();
        Attack();
        Block();
    }


    private void Movement()
    {
        Vector2 movementInput = _inputManager.GetPlayerMovementInput();
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);

        statistic.DirectionMovement = movementInput.y;
        _rigidbody.AddRelativeForce(move * statistic.Speed * Running());
        float Running()
        {
            if (!statistic.isFatigue)
            {
                if (_inputManager.GetPlayerSprintInput() && _inputManager.GetPlayerMovementInput().y > 0)
                {
                    statistic.Stamina -= 3f * Time.deltaTime;
                    if (statistic.Stamina <= 0)
                    {
                        statistic.isFatigue = true;
                    }
                    return statistic.Acceleration = 1.25f;
                }
                else
                {
                    statistic.Stamina += 5f * Time.deltaTime;
                    return statistic.Acceleration = 1f;
                }
            }
            else
            {
                statistic.Stamina += 3f * Time.deltaTime;
                if (statistic.Stamina > 15)
                {
                    statistic.isFatigue = false;
                }
                return statistic.Acceleration = 0.9f;
            }
        }
    }
    private void Rotat()
    {
        //_playerTransform.localRotation = Quaternion.Euler(0f, _cameraTransform.rotation.y * 180, 0f);
        //_rigidbody.MoveRotation(Quaternion.Euler(0f, _cameraTransform.rotation.y * 180, 0f));

        _playerTransform.rotation = new Quaternion(0f, _cameraTransform.rotation.y, 0f, _cameraTransform.rotation.w);
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