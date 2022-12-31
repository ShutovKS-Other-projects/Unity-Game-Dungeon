using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerStatistics playerStatistics = new PlayerStatistics(); 
    private float _speed;
    private float _horizontalInput;
    private float _verticalInput;

    void Start()
    {
        _speed = playerStatistics._speed;
    }

    private void Update()
    {
        Moving();

    }

    private void Moving()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(_horizontalInput, 0f, _verticalInput);

        transform.Translate(movement * _speed * Time.deltaTime);
    }
}