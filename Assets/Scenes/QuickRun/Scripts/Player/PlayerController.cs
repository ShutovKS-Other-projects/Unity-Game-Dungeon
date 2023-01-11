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

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {Stop();
     
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _jumpInput = Input.GetAxis("Jump");
        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {Stop();
     
        Attack();
        Jump();
        Movement();
        Rotation();
    }

    private void OnTriggerEnter(Collider other)
    {Stop();
     
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
    private void Jump()
    {
        if (_jumpInput > 0 && statistics.stamina > 0)
        {
            _rigidbody.AddForce(Vector3.up * statistics.force, ForceMode.Impulse);
            statistics.stamina -= 10;
            statistics.isJump = true;
        }
    }
    private void Movement()
    {
		statistics.movement = _verticalInput * Running();
        Vector3 movement = new Vector3(_horizontalInput, 0f, _verticalInput);
        if (_horizontalInput != 0 && _verticalInput != 0)
        {
            movement /= 1.5f;
        }
        _rigidbody.AddRelativeForce(movement * statistics.speed * Running());

        float Running()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                statistics.acceleration = 1.25f;
                statistics.stamina -= 1f * Time.deltaTime;
            }
            else
            {
                statistics.acceleration = 1f;
            }
            return statistics.acceleration;
        }
    }
    private void Rotation()
    {
        _xRotation -= _mouseY * _mouseSensitivity * Time.deltaTime;
        _yRotation += _mouseX * _mouseSensitivity * Time.deltaTime;

        if (_xRotation > -90f && _xRotation < 90f)
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        _playerTransform.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
        _rigidbody.MoveRotation(Quaternion.Euler(0f, _yRotation, 0f));
    }

    private void Stop()
    {
        if (statistics.isDead)
        {
            return;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Health: " + statistics.health);
        GUI.Label(new Rect(10, 30, 100, 20), "Stamina: " + statistics.stamina);
        GUI.Label(new Rect(10, 50, 100, 20), "Speed: " + statistics.speed);
        GUI.Label(new Rect(10, 70, 100, 20), "Force: " + statistics.force);
        GUI.Label(new Rect(10, 90, 100, 20), "Acceleration: " + statistics.acceleration);
        GUI.Label(new Rect(10, 110, 100, 20), "Movement: " + statistics.movement);
        GUI.Label(new Rect(10, 130, 100, 20), "IsAttack: " + statistics.isAttack);
        GUI.Label(new Rect(10, 150, 100, 20), "IsJump: " + statistics.isJump);
        GUI.Label(new Rect(10, 170, 100, 20), "IsDead: " + statistics.isDead);
    }
}