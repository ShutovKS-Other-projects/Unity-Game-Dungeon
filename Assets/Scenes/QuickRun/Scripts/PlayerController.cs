using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerStatistics statistics;

    Rigidbody rb;

    private float _horizontalInput;
    private float _verticalInput;
    private float _jumpInput;

    void Start()
    {
        statistics = new PlayerStatistics();

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _jumpInput = Input.GetAxis("Jump");
    }

    private void FixedUpdate()
    {
        if (_horizontalInput != 0 && _verticalInput != 0)
        {
            _horizontalInput /= 2;
        }
        Vector3 movement = new Vector3(_horizontalInput, 0f, _verticalInput);
        rb.AddRelativeForce(movement * statistics.speed * Boost());
    }

    private float Boost()
    {
        float boost = 0.7f;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            boost = 0.9f;
        }

        return boost;
    }
}
