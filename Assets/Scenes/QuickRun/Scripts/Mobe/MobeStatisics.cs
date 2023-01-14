using System;

public class MobeStatistics
{
    private readonly Random random = new Random();
    /// <summary>
    /// Damage of the mob
    /// </summary>
    public float damage;

    /// <summary>
    /// Knockback force of the mob
    /// </summary>
    public float force;

    /// <summary>
    /// Health of the mob
    /// </summary>
    public float health;

    /// <summary>
    /// Movement speed of the mob
    /// </summary>
    public float movement = 0;

    /// <summary>
    /// Speed of the mob
    /// </summary>
    public float speed;

    /// <summary>
    /// Current animation state of the mob
    /// </summary>
    public string isStateAnimation;

    /// <summary>
    /// Is the mob attacking
    /// </summary>
    public bool isAttack;

    /// <summary>
    /// Is the mob dead
    /// </summary>
    public bool isDead = false;

    /// <summary>
    /// Is the player detected by the mob
    /// </summary>
    public bool isPlayerDetected = false;

    /// <summary>
    /// Timer for the attack
    /// </summary>
    public float attackTimer;

    /// <summary>
    /// Attack cooldown
    /// </summary>
    public float attackCooldown;

    public MobeStatistics()
    {
        this.force = random.Next(75, 125) / 10f;
        this.health = random.Next(100, 150);
        this.speed = random.Next(150, 175) / 10f;
        this.attackCooldown = random.Next(500, 750) / 100f;
        this.attackTimer = this.attackCooldown;
    }
}