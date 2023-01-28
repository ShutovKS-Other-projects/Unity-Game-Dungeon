using UnityEngine;

public class MobeStatistic : MonoBehaviour
{
    public MobeStatisticObject mobeStatisticObject;
    public string Name { get { return mobeStatisticObject.Name; } }
    public float Damage { get { return mobeStatisticObject.Damage; } }
    public float Health
    {
        get { return mobeStatisticObject.Health; }
        set { mobeStatisticObject.Health = value; }
    }
    public float Speed { get { return mobeStatisticObject.Speed; } }
    public float AttackCooldown { get { return mobeStatisticObject.AttackCooldown; } }
    public bool isDead { get { return mobeStatisticObject.Health <= 0; } }
    
    [System.NonSerialized] public float Movement = 0 ;
    [System.NonSerialized] public float AttackTimer;
    [System.NonSerialized] public bool isAttack = false;
    [System.NonSerialized] public bool isPlayerDetected = false;

    private void Awake()
    {
        AttackTimer = AttackCooldown;
    }

}
