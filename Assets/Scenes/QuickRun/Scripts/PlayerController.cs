using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerStatistics playerStatistics;

    private float _speed;
    private float _horizontalInput;
    private float _verticalInput;
    private float _jumpInput;

    void Start()
    {
        playerStatistics = new PlayerStatistics();

        _speed = playerStatistics.speed;
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _jumpInput = Input.GetAxis("Jump");

        Vector3 movement = new Vector3 (_horizontalInput, 0f, _verticalInput);
        transform.Translate(movement * _speed * Boost() * Time.deltaTime);
    }


    private float Boost()
    {
        float _speed = 0.75f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _speed = 1.5f;
            return _speed;
        }
        return _speed;
    }
}