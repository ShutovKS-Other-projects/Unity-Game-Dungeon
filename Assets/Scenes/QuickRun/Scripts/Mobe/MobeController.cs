using System;
using UnityEngine;

public class MobeController : MonoBehaviour
{
    public MobeStatistics statistics;

    private GameObject Player;

    private Rigidbody _rigidbody;

    private void Start()
    {
        statistics = new MobeStatistics();
        Player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {if (statistics.isDead) return;
        

        Ray ray = new(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Player.transform.position - transform.position);
        Physics.Raycast(ray, out RaycastHit raycastHit, 7.5f);
        if (raycastHit.collider.gameObject == Player)
        {
            Attack();
            Movement();
            Rotation();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!statistics.isDead)
        {
            if (other.gameObject.name == "mixamorig:RightHand" || other.gameObject.name == "mixamorig:LeftHand")
            {
                statistics.health = -1f;
                statistics.isDead = true;
                //Destroy(gameObject, 10f);
                _rigidbody.isKinematic = true;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                Debug.Log("Dead");
            }
        }
    }

    private void Attack()
    {
        if (statistics.movement == 0)
        {
            if (statistics.attackTimer <= 0)
            {
                statistics.isAttack = true;
            }
            else
            {
                statistics.attackTimer -= Time.deltaTime;
            }
        }
        else
        {
            statistics.attackTimer = statistics.attackCooldown;
        }
    }
    private void Movement()
    {
        _rigidbody.AddRelativeForce(Move());

        Vector3 Move()
        {
            if (statistics.isStateAnimation == "Attack")
                return new Vector3(0f, 0f, 0f);
            switch ((float)Math.Round(Vector3.Distance(transform.position, Player.transform.position), 2))
            {
                case > 0.65f:
                    statistics.movement = 1f;
                    return new Vector3(0f, 0f, statistics.speed);
                case < 0.4f:
                    statistics.movement = -1f;
                    return new Vector3(0f, 0f, -0.75f * statistics.speed);
                default:
                    statistics.movement = 0f;
                    Attack();
                    return new Vector3(0f, 0f, 0f);
            }
        }
    }
    private void Rotation()
    {
        transform.LookAt(Player.transform.position);
    }
}
