using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStatistic statistic;
    private Rigidbody _rigidbody;
    private Transform _playerTransform;
    private Transform _cameraTransform;
    private float _horizontalInput;
    private float _verticalInput;
    private float _jumpInput;
    private float _mouseX;
    private float _mouseY;
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    private float _mouseSensitivity = 150f;

    void Start()
    {
        statistic = new PlayerStatistic();
        _playerTransform = transform;
        _cameraTransform = Camera.main.transform;
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _jumpInput = Input.GetAxis("Jump");
        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");

        Jump();
        Attack();
        Block();
    }
    private void FixedUpdate()
    {
        Movement();
        Rotation();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MobeWeapon")
        {
            statistic.Health -= 10;
        }
    }
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && statistic.Stamina > 10)
        {
            statistic.isAttack = true;
            statistic.Stamina -= 10;
        }
    }
    private void Block()
    {
        if (Input.GetMouseButtonDown(1))
        {
            statistic.isBlock = true;
        }
    }
    private void Jump()
    {
        if (_jumpInput > 0 && statistic.Stamina > 10)
        {
            _rigidbody.AddForce(Vector3.up * statistic.JumpForce);
            statistic.Stamina -= 10;
        }
    }
    private void Movement()
    {
        statistic.Movement = _verticalInput * Running();
        _rigidbody.AddRelativeForce(new Vector3(_horizontalInput, 0f, _verticalInput) * statistic.Speed * Running());

        float Running()
        {
            switch (statistic.Stamina, statistic.isFatigue, Input.GetKey(KeyCode.LeftShift))
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
                case ( > 0, true, false):
                    statistic.Stamina += 2f * Time.deltaTime;
                    if (statistic.Stamina > 15f)
                    {
                        statistic.isFatigue = false;
                    }
                    return statistic.Acceleration = 0.9f;
                case (100, true || false, false || true):
                    statistic.Stamina = 100;
                    return statistic.Acceleration = 1f;
                default:
                    return statistic.Acceleration = 1f;
            }
        }
    }
    private void Rotation()
    {
        _xRotation -= _mouseY * _mouseSensitivity * Time.deltaTime;
        _yRotation += _mouseX * _mouseSensitivity * Time.deltaTime;

        _xRotation = Mathf.Clamp(_xRotation, -70, 70f);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _playerTransform.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
        _rigidbody.MoveRotation(Quaternion.Euler(0f, _yRotation, 0f));
    }
}