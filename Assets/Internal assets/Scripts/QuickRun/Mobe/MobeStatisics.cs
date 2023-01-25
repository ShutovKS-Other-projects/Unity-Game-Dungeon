using System;

public class MobeStatistics
{
    private readonly Random random = new Random();
    private float _damage = 10f;
    private float _health = 100f;
    private float _movement = 0;
    private static float _speed = 10f;

    public float Damage { get { return _damage; } }
    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }
    public float Movement
    {
        get { return _movement; }
        set { _movement = value; }
    }
    public float Speed { get { return _speed; } }

    public bool isAttack = false;
    public bool isDead { get { return _health <= 0; } }
    public bool isPlayerDetected = false;
    public float attackCooldown = 5f;
    public float attackTimer = 5f;
}