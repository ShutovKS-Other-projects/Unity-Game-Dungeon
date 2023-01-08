using System;
using UnityEngine;

public class MobeController : MonoBehaviour
{
    private MobeAnimatorController animatorController;
    public MobeStatistics statistics;

    private GameObject Player;

    private void Start()
    {
        animatorController = transform.Find("Animator").GetComponent<MobeAnimatorController>();
        statistics = new MobeStatistics();
        Player = GameObject.Find("Player(Clone)");
    }

    private void Update()
    {
        if (statistics.isDead)
        {
            return;
        }
        statistics.speed = 0f;
        Ray ray = new(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Player.transform.position - transform.position);
        Physics.Raycast(ray, out RaycastHit raycastHit, 7.5f);
        if (raycastHit.collider.gameObject == Player)
        {
            transform.LookAt(Player.transform.position);

            if (Vector3.Distance(transform.position, Player.transform.position) >= 0.5f)
            {
                transform.Translate(new Vector3(0f, 0f, 0.5f) * Time.deltaTime);
                statistics.speed = 1f;
                return;
            }
            else if (Vector3.Distance(transform.position, Player.transform.position) <= 0.3f)
            {
                transform.Translate(new Vector3(0f, 0f, -0.25f) * Time.deltaTime);
                statistics.speed = -1f;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!statistics.isDead)
        {

            if (other.gameObject.name == "mixamorig:RightHand" || other.gameObject.name == "mixamorig:LeftHand")
            {
                statistics.health = -1f;
                if (statistics.health <= 0)
                {
                    statistics.isDead = true;
                }
                Destroy(gameObject, 10f);
            }
        }
    }
}
