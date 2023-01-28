using UnityEngine;

public class MobeAnimatorController : MonoBehaviour
{
    private Animator animator;
    private MobeStatistic statistic;
    private bool _animationDeadStart = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        statistic = GetComponentInParent<MobeStatistic>();
    }

    private void Update()
    {
        if (_animationDeadStart) return;

        animator.SetFloat("Speed", statistic.Movement);
        if (statistic.isAttack)
        {
            animator.SetTrigger("Attack");
            statistic.isAttack = false;
            statistic.AttackTimer = statistic.AttackCooldown;
        }

        if (statistic.isDead)
        {
            animator.SetTrigger("Dead");
            _animationDeadStart = true;
        }
    }
}