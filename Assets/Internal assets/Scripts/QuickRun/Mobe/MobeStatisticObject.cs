using UnityEngine;

[CreateAssetMenu(fileName = "MobeStatistic", menuName = "Mobe/Statistic", order = 0)]
public class MobeStatisticObject : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _damageMin;
    [SerializeField] private float _damageMax;
    [SerializeField] private float _health;
    [SerializeField] private float _speed;
    [SerializeField] private float _attackCooldown;

    public string Name 
    { 
        get { return _name; } 
    }
    public float Damage 
    { 
        get { return Random.Range(_damageMin, _damageMax); } 
    }
    public float Health 
    { 
        get { return _health; } 
        set { _health = value; }
    }
    public float Speed 
    { 
        get { return _speed; } 
    }
    public float AttackCooldown 
    { 
        get { return _attackCooldown; } 
    }
}
