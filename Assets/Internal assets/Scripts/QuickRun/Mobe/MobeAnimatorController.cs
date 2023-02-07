using UnityEngine;

public class MobeAnimatorController : MonoBehaviour
{
    private Animator _animator;
    private MobeStatistic _statistic;
    
    private bool _animationDeadStart = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _statistic = GetComponentInParent<MobeStatistic>();
    }

    private void Update()
    {
        if (_animationDeadStart) return;

        _animator.SetFloat("Speed", _statistic.Movement);
        if (_statistic.isAttack)
        {
            _animator.SetTrigger("Attack");
            _statistic.isAttack = false;
            _statistic.AttackTimer = _statistic.AttackCooldown;
        }

        if (_statistic.isDead)
        {
            _animator.SetTrigger("Dead");
            _animationDeadStart = true;
        }
    }
}