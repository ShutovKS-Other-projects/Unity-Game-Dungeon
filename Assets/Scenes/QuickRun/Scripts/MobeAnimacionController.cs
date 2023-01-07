using UnityEngine;

public class MobeAnimacionController : MonoBehaviour
{
    Animator animator;
    private MobeController mobeController;

    float speed;
    bool dead;
    bool attack;

    private void Start()
    {
        mobeController = new MobeController();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateStatistics();

        animator.SetFloat("Speed", speed);
        if(dead)
        {
            animator.SetTrigger("Dead");
        }

        if (mobeController.attack)
        {
            animator.SetTrigger("Attack");
        }
    }

    void UpdateStatistics()
    {
        speed = mobeController.speed;
        dead = mobeController.dead;
        attack = mobeController.attack;
    }
}
