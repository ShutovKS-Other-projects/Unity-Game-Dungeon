using System;

public class MobeStatistics
{
    private readonly Random random = new();

    /// <summary>
    /// Урон параметр
    /// </summary>
    public float damage;

    /// <summary>
    /// Сила параметр
    /// </summary>
    public float force;

    /// <summary>
    /// ХП параметр
    /// </summary>
    public float health;

    /// <summary>
    /// Передвижение
    /// </summary>
    public float movement = 0;

    /// <summary>
    /// Скорость параметр
    /// </summary>
    public float speed;

    /// <summary>
    /// Выполняемая анимация
    /// </summary>
    public string isStateAnimation;

    /// <summary>
    /// Атака состояние
    /// </summary>
    public bool isAttack;

    /// <summary>
    /// Смерть состояние
    /// </summary>
    public bool isDead = false;

    /// <summary>
    /// Игрок обнаружен
    /// </summary>
    public bool isPlayerDetected = false;

    /// <summary>
    /// Таймер после атаки
    /// 
    public float attackTimer;

    /// <summary>
    /// Время между атаками
    /// </summary>
    public float attackCooldown;

    public MobeStatistics() 
    {
        this.force  = random.Next(75, 125) / 10f;
        this.health = random.Next(100, 150);
        this.speed  = random.Next(215, 235) / 10f;
        this.attackCooldown = random.Next(500, 750) / 100f;
        this.attackTimer = this.attackCooldown;
    }
}
