using System.Collections;
using System.Collections.Generic;
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

        Attack();
        Jump();
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
            statistics.health -= 10;
        }
    }
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            statistics.isAttack = true;
            statistics.stamina -= 10;
        }
    }
    private void Block()
    {
        if(Input.GetMouseButtonDown(1))
        {
            statistics.isBlock = true;

        }
    }
    private void Jump()
    {
        if (_jumpInput > 0 && statistics.stamina > 0)
        {
            _rigidbody.AddForce(Vector3.up * statistics.jumpForce);
            statistics.stamina -= 10;
        }
    }
    private void Movement()
    {
        statistics.movement = _verticalInput * Running();
        Vector3 movement = new Vector3(_horizontalInput, 0f, _verticalInput);
        if (_horizontalInput != 0 && _verticalInput != 0)
        {
            movement /= 1.4f;
        }
        _rigidbody.AddRelativeForce(movement * statistics.speed * Running());

        float Running()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                statistics.acceleration = 1.25f;
                statistics.stamina -= 5f * Time.deltaTime;
            }
            else
            {
                statistics.acceleration = 1f;
                if(statistics.stamina < 100)
                {
                    statistics.stamina += 3f * Time.deltaTime;
                }
                if(statistics.stamina > 100)
                {
                    statistics.stamina = 100;
                }
            }
            return statistics.acceleration;
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