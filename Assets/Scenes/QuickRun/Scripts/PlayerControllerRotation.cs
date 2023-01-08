using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlerRotation : MonoBehaviour
{
    private float _mouseSensitivity = 150f;
    private Rigidbody _rb;

    private Transform _playerTransform;
    private Transform _cameraTransform;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _playerTransform = transform;
        _cameraTransform = Camera.main.transform;
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;

        if (xRotation > -90f && xRotation < 90f)
            _cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        _playerTransform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        _rb.MoveRotation(Quaternion.Euler(0f, yRotation, 0f));
    }
}
