using UnityEngine;

public class PlayerAnimacionController : MonoBehaviour
{
    private Animator animator;
    private PlayerController controller;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        animator.SetFloat("Speed", controller.statistic.Movement);
        HandleTriggers();
    }

    private void HandleTriggers()
    {
        if (controller.statistic.isAttack)
        {
            animator.SetTrigger("Attack");
            controller.statistic.isAttack = false;
        }

        if (controller.statistic.isBlock)
        {
            
        }

        if (controller.statistic.isJump)
        {
            animator.SetTrigger("Jump");
            controller.statistic.isJump = false;
        }

        if (controller.statistic.isDead)
        {
            animator.SetTrigger("Dead");
            controller.statistic.isDead = false;
        }
    }
}