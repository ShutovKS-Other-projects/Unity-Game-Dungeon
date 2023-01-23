using UnityEngine;

public class MobeAnimatorController : MonoBehaviour
{
    private Animator animator;
    private MobeController controller;
    private bool _animationDeadStart = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<MobeController>();
    }

    private void Update()
    {
        if (_animationDeadStart) return;

        animator.SetFloat("Speed", controller.statistics.Movement);
        if (controller.statistics.isAttack)
        {
            animator.SetTrigger("Attack");
            controller.statistics.isAttack = false;
            controller.statistics.attackTimer = controller.statistics.attackCooldown;
        }

        if (controller.statistics.isDead)
        {
            animator.SetTrigger("Dead");
            _animationDeadStart = true;
        }

        State();
    }

    void State()
    {
        controller.statistics.isStateAnimation = animator.GetCurrentAnimatorStateInfo(0).nameHash.ToString();
    }
}