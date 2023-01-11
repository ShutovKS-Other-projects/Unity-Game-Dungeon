using System;

public class MobeStatistics
{
    private readonly Random random = new();

    /// <summary>
    /// ���� ��������
    /// </summary>
    public float damage;

    /// <summary>
    /// ���� ��������
    /// </summary>
    public float force;

    /// <summary>
    /// �� ��������
    /// </summary>
    public float health;

    /// <summary>
    /// ������������
    /// </summary>
    public float movement = 0;

    /// <summary>
    /// �������� ��������
    /// </summary>
    public float speed;

    /// <summary>
    /// ����������� ��������
    /// </summary>
    public string isStateAnimation;

    /// <summary>
    /// ����� ���������
    /// </summary>
    public bool isAttack;

    /// <summary>
    /// ������ ���������
    /// </summary>
    public bool isDead = false;

    /// <summary>
    /// ����� ���������
    /// </summary>
    public bool isPlayerDetected = false;

    /// <summary>
    /// ������ ����� �����
    /// 
    public float attackTimer;

    /// <summary>
    /// ����� ����� �������
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
