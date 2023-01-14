using System;
using UnityEngine;

public class MobeController : MonoBehaviour
{
    public MobeStatistics statistics;
    private GameObject _player;
    private Rigidbody _rigidbody;

    private void Start()
    {
        statistics = new MobeStatistics();
        _player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!statistics.isDead)
        {
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), _player.transform.position - transform.position);
            Physics.Raycast(ray, out RaycastHit raycastHit, 7.5f);
            if (raycastHit.collider.gameObject == _player)
            {
                Attack();
                Movement();
                Rotation();
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
                    Dead();
                }
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

    private void Dead()
    {
        statistics.isDead = true;
        _rigidbody.isKinematic = true;
        gameObject.GetComponent<CapsuleCollider>().direction = 2;
        gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0.25f, 0.5f, 1f);
    }

    private void Movement()
    {
        _rigidbody.AddRelativeForce(Move());

        Vector3 Move()
        {
            if (statistics.isStateAnimation == "Attack")
                return new Vector3(0f, 0f, 0f);
            float distance = (float)Math.Round(Vector3.Distance(transform.position, _player.transform.position), 2);
            if (distance > 0.65f)
            {
                statistics.movement = 1f;
                return new Vector3(0f, 0f, statistics.speed);
            }
            else if (distance < 0.4f)
            {
                statistics.movement = -1f;
                return new Vector3(0f, 0f, -0.75f * statistics.speed);
            }
            else
            {
                statistics.movement = 0f;
                Attack();
                return new Vector3(0f, 0f, 0f);
            }
        }
    }
    private void Rotation()
    {
        transform.LookAt(_player.transform.position);
    }
}