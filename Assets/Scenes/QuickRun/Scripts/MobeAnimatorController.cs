using UnityEngine;

public class MobeAnimatorController : MonoBehaviour
{
    private Animator animator;
    private MobeController mobeController;

    private void Start()
    {
        animator = GetComponent<Animator>();
        mobeController = GetComponentInParent<MobeController>();
    }

    private void Update()
    {
        animator.SetFloat("Speed", mobeController.statistics.speed);
        animator.SetBool("Dead", mobeController.statistics.isDead);
    }


}
