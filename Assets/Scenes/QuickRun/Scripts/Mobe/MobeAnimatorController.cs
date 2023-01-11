using UnityEngine;

public class MobeAnimatorController : MonoBehaviour
{
    private Animator animator;
    private MobeController controller;

    private bool animationDeadStart = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<MobeController>();
    }

    private void Update()
    {if (animationDeadStart) return;
        State();

        animator.SetFloat("Speed", controller.statistics.movement);
        if (controller.statistics.isAttack)
        {
            animator.SetTrigger("Attack");
            controller.statistics.isAttack = false;
            controller.statistics.attackTimer = controller.statistics.attackCooldown;
        }

        if (controller.statistics.isDead)
        {
            animator.SetTrigger("Dead");
            animationDeadStart = true;
        }

    }

    void State()
    {
        controller.statistics.isStateAnimation = animator.GetCurrentAnimatorStateInfo(0).nameHash.ToString();
    }
}