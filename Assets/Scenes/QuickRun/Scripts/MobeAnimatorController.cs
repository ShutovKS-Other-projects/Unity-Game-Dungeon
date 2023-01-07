using UnityEngine;

public class MobeAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
}
