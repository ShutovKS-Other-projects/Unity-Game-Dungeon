using UnityEngine;

public class PlayerAnimacionController : MonoBehaviour
{
    private Animator animator;
    private PlayerStatistic statistic;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        statistic = GameObject.FindWithTag("Player").GetComponent<PlayerController>().statistic;
    }

    void Update()
    {
        animator.SetFloat("Speed", statistic.Movement);
        HandleTriggers();
    }

    private void HandleTriggers()
    {
        if (statistic.isAttack)
        {
            animator.SetTrigger("Attack");
            statistic.isAttack = false;
            statistic.Stamina -= 10;
        }

        if (statistic.isBlock)
        {
            
        }

        if (statistic.isJump)
        {
            animator.SetTrigger("Jump");
            statistic.isJump = false;
            statistic.Stamina -= 10;
        }

        if (statistic.isDead)
        {
            animator.SetTrigger("Dead");
        }
    }
}