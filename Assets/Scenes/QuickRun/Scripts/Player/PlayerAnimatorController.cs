using UnityEngine;

public class PlayerAnimacionController : MonoBehaviour
{
    private Animator animator;
	private PlayerController controller;
    
    void Start()
	{
        animator    = gameObject.GetComponent<Animator>();
        controller  = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

	private void FixedUpdate()
	{
        animator.SetFloat("Speed", controller.statistics.movement);
        if (controller.statistics.isAttack)
        {
            animator.SetTrigger("Attack");
            controller.statistics.isAttack = false;
        }
        if (controller.statistics.isJump)
        {
            animator.SetTrigger("Jump");
            controller.statistics.isJump = false;
        }
        if (controller.statistics.isDead)
        {
            animator.SetTrigger("Dead");
            controller.statistics.isDead = false;
        }
    }
}