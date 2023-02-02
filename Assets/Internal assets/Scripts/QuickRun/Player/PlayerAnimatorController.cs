using UnityEngine;

public class PlayerAnimacionController : MonoBehaviour
{
    private Animator animator;
    private PlayerController controller;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        controller = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        animator.SetFloat("Speed", controller.statistic.DirectionMovement * controller.statistic.Acceleration);
        HandleTriggers();
    }

    private void HandleTriggers()
    {
        if (controller.statistic.isAttack)
        {
            animator.SetTrigger("Attack");
            controller.statistic.isAttack = false;
            controller.statistic.Stamina -= 10;
        }

        if (controller.statistic.isBlock)
        {
            
        }

        if (controller.statistic.isJump)
        {
            animator.SetTrigger("Jump");
            controller.statistic.isJump = false;
            controller.statistic.Stamina -= 10;
        }

        if (controller.statistic.isDead)
        {
            animator.SetTrigger("Dead");
        }
    }
}