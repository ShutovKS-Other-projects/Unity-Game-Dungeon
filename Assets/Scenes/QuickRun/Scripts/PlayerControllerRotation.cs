using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlerRotation : MonoBehaviour
{
    public float mouseSensitivity = 1000f;

    private GameObject _player;
    private GameObject _camera;

    float xRotation = 0f;
    float yRotation = 0f;

    float xRotationMin = -90f;
    float xRotationMax = 90f;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _player = transform.gameObject;
        _camera = GameObject.Find("Camera");
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;

        if (xRotation > -90f && xRotation < 90f)
            _camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        _player.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
