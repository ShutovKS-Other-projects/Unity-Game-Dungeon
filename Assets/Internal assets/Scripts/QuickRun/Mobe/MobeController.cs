using System;
using UnityEngine;

public class MobeController : MonoBehaviour
{
    private MobeStatistic statistic;
    private GameObject _player;
    private Rigidbody _rigidbody;

    private void Start()
    {
        statistic = GetComponent<MobeStatistic>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!statistic.isDead)
        {
            GettingVisibility(out Collider collider);

            if (collider)
            {
                Rotation();
                Attack();
                Movement();   
            }
            else
            {
                statistic.Movement = 0f;
                statistic.isAttack = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!statistic.isDead)
        {
            if (other.tag == "ObjectDamaging")
            {
                statistic.Health = 0f;
                if (statistic.isDead)
                {
                    Dead();
                }
            }
        }
    }

    private void Attack()
    {
        if (statistic.Movement == 0)
        {
            if (statistic.AttackTimer <= 0)
            {
                statistic.isAttack = true;
            }
            else
            {
                statistic.AttackTimer -= Time.deltaTime;
            }
        }
        else
        {
            statistic.AttackTimer = statistic.AttackCooldown;
        }
    }

    private void Dead()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<CapsuleCollider>().direction = 2;
        GetComponent<CapsuleCollider>().center = new Vector3(0.25f, 0.35f, 1f);
        GetComponent<CapsuleCollider>().height = 0.3f;
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    private void Movement()
    {
        _rigidbody.AddRelativeForce(Move() * statistic.Speed);

        Vector3 Move()
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance > 2f)
            {
                statistic.Movement = 1f;
                return new Vector3(0f, 0f, 1f);
            }
            else if (distance < 1f)
            {
                statistic.Movement = -1f;
                return new Vector3(0f, 0f, -0.75f);
            }
            else
            {
                statistic.Movement = 0f;
                Attack();
                return new Vector3(0f, 0f, 0f);
            }
        }
    }

    private void Rotation()
    {
        transform.LookAt(_player.transform.position);
    }

    private void GettingVisibility(out Collider collider)
    {
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), _player.transform.position - transform.position);
        Physics.Raycast(ray, out RaycastHit raycastHit, 10f);
        collider = raycastHit.collider;
    }
}