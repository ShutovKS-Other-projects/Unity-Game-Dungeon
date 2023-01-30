using UnityEngine;

public class MobeStatistic : MonoBehaviour
{
    public MobeStatisticObject statisticObject;

    private float _health;
    public string Name { get { return statisticObject.Name; } }
    public float Damage { get { return statisticObject.Damage; } }
    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }
    public float AttackCooldown { get { return statisticObject.AttackCooldown; } }
    public float Speed { get { return statisticObject.Speed; } }
    public bool isDead { get { return _health <= 0; } }
    
    [System.NonSerialized] public float AttackTimer;
    [System.NonSerialized] public float Movement = 0 ;
    [System.NonSerialized] public float RotationSpeed = 5f;
    [System.NonSerialized] public bool isAttack = false;
    [System.NonSerialized] public bool isPlayerDetected = false;

    private void Awake()
    {
        _health = statisticObject.Health;
        AttackTimer = AttackCooldown;
    }
}
