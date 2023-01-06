using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobeAnimacionController : MonoBehaviour
{
    Animator animator;
    private MobeController mobeController;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float speed = mobeController.speed;
        animator.SetFloat("Speed", speed);
        animator.SetFloat("XP", mobeController.xp);
        
        if(mobeController.attack)
        {
            animator.SetTrigger("Attack");
        }
    }
}
