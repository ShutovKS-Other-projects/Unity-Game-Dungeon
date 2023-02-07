using System;
using UnityEngine;

public class MobeController : MonoBehaviour
{
    private MobeStatistic _statistic;
    private GameObject _player;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _statistic = GetComponent<MobeStatistic>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_statistic.isDead)
        {
            if (CheckVisibleIfPlayer())
            {
                Rotation();
                Attack();
                Movement();
            }
            else
            {
                _statistic.Movement = 0f;
                _statistic.isAttack = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_statistic.isDead)
        {
            if (other.tag == "ObjectDamaging")
            {
                _statistic.Health = 0f;
                if (_statistic.isDead)
                {
                    Dead();
                }
            }
        }
    }

    private void Attack()
    {
        if (_statistic.Movement == 0)
        {
            if (_statistic.AttackTimer <= 0)
            {
                _statistic.isAttack = true;
            }
            else
            {
                _statistic.AttackTimer -= Time.deltaTime;
            }
        }
        else
        {
            _statistic.AttackTimer = _statistic.AttackCooldown;
        }
    }

    private void Dead()
    {
        DropItem(transform.position);
        Destroy(gameObject);
    }

    private void Movement()
    {
        _rigidbody.AddRelativeForce(Move() * _statistic.Speed);

        Vector3 Move()
        {
            float distance = (float)Math.Round(Vector3.Distance(transform.position, _player.transform.position), 1);

            switch (distance)
            {
                case > 2.5f:
                    _statistic.Movement = 1f;
                    return new Vector3(0f, 0f, 1f);
                case > 1.55f:
                    _statistic.Movement = 1f;
                    return new Vector3(0f, 0f, 0.75f);
                case < 0.85f:
                    _statistic.Movement = -1f;
                    return new Vector3(0f, 0f, 1f);
                default:
                    Attack();
                    _statistic.Movement = 0f;
                    return new Vector3(0f, 0f, 0f);
            }
        }
    }

    private void Rotation()
    {
        transform.LookAt(_player.transform.position);
    }

    private bool CheckVisibleIfPlayer()
    {
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), _player.transform.position - transform.position);
        Physics.Raycast(ray, out RaycastHit raycastHit, 6f);

        if (raycastHit.collider)
        {
            if (raycastHit.collider.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    private void DropItem(Vector3 position)
    {
        GameObject item = Instantiate(GameObject.Find("ItemDatabase").GetComponent<ItemDatabase>().GetRandomItemPrefab(), position, Quaternion.identity);
        item.layer = LayerMask.NameToLayer("Interactable");
        item.AddComponent<Rigidbody>();
        item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
}