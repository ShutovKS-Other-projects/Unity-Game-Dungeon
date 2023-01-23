using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStatistics statistics;
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
        statistics = new PlayerStatistics();
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
            statistics.Health -= 10;
        }
    }
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && statistics.Stamina > 10)
        {
            statistics.isAttack = true;
            statistics.Stamina -= 10;
        }
    }
    private void Block()
    {
        if (Input.GetMouseButtonDown(1))
        {
            statistics.isBlock = true;
        }
    }
    private void Jump()
    {
        if (_jumpInput > 0 && statistics.Stamina > 10)
        {
            _rigidbody.AddForce(Vector3.up * statistics.JumpForce);
            statistics.Stamina -= 10;
        }
    }
    private void Movement()
    {
        statistics.Movement = _verticalInput * Running();
        _rigidbody.AddRelativeForce(new Vector3(_horizontalInput, 0f, _verticalInput) * statistics.Speed * _rigidbody.mass * Running());

        float Running()
        {
            switch (statistics.Stamina, statistics.isFatigue, Input.GetKey(KeyCode.LeftShift))
            {
                case ( > 0, false, true):
                    statistics.Stamina -= 2.5f * Time.deltaTime;
                    return statistics.Acceleration = 1.25f;
                case ( < 100, false, false):
                    if (statistics.Stamina == 0)
                    {
                        statistics.isFatigue = true;
                    }
                    statistics.Stamina += 5f * Time.deltaTime;
                    return statistics.Acceleration = 1f;
                case ( > 0, true, false):
                    statistics.Stamina += 2f * Time.deltaTime;
                    if (statistics.Stamina > 15f)
                    {
                        statistics.isFatigue = false;
                    }
                    return statistics.Acceleration = 0.9f;
                case (100, true || false, false || true):
                    statistics.Stamina = 100;
                    return statistics.Acceleration = 1f;
                default:
                    return statistics.Acceleration = 1f;
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