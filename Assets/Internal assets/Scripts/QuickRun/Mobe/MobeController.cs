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
            Physics.Raycast(ray, out RaycastHit raycastHit, 4f);
            if (raycastHit.collider.gameObject == _player)
            {
                Rotation();
                Attack();
                Movement();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!statistics.isDead)
        {
            if (other.gameObject.name == "mixamorig:RightHand" || other.gameObject.name == "mixamorig:LeftHand")
            {
                statistics.Health = -1f;
                if (statistics.isDead)
                {
                    Dead();
                }
            }
        }
    }

    private void Attack()
    {
        if (statistics.Movement == 0)
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
        _rigidbody.isKinematic = true;
        gameObject.GetComponent<CapsuleCollider>().direction = 2;
        gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0.25f, 0.5f, 1f);
    }
    private void Movement()
    {
        _rigidbody.AddRelativeForce(Move() * statistics.Speed);

        Vector3 Move()
        {
            if (statistics.isStateAnimation == "Attack")
                return new Vector3(0f, 0f, 0f);
            float distance = (float)Math.Round(Vector3.Distance(transform.position, _player.transform.position), 2);
            if (distance > 0.65f)
            {
                statistics.Movement = 1f;
                return new Vector3(0f, 0f, 1f);
            }
            else if (distance < 0.4f)
            {
                statistics.Movement = -1f;
                return new Vector3(0f, 0f, -0.75f);
            }
            else
            {
                statistics.Movement = 0f;
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