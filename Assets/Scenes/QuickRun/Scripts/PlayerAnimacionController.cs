using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimacionController : MonoBehaviour
{
    private Animator _animator;

    private float _horizontalInput;
    private float _verticalInput;

    void Start()
	{
        _animator = gameObject.GetComponent<Animator>();
    }

	private void Update()
	{

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");


        _animator.SetFloat("Speed", Movement(_verticalInput));

        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger("Attack");
        }
        if (Input.GetAxis("Jump") > 0.99)
        {
            _animator.SetTrigger("Jump");
            return;
        }
    }

    private float Movement(float _verticalInput)
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            return 2;
        }
        return _verticalInput;
    }
}

/*
    private Animator _animator;

    private float _horizontalInput;
    private float _verticalInput;

    void Start()
	{
        _animator = gameObject.GetComponent<Animator>();
    }

	private void Update()
	{

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");


        _animator.SetBool("Walking", WalkingCheck());
        _animator.SetBool("Running", RunningCheck());


        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger("Attack");
        }
    }

    private bool WalkingCheck()
    {
        if (_verticalInput > 0)
        {
            return true;
        }
        if (_verticalInput <= 0)
        {
            return false;
        }
        return false;
    }
    private bool RunningCheck()
    {
        if (Input.GetAxis("Fire3") != 0)
        {
            return true;
        }
        if (Input.GetAxis("Fire3") == 0)
        {
            return false;
        }
        return false;
    }
    */