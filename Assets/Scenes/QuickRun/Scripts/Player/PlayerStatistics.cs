public class PlayerStatistics
{
    /// <summary>
    /// Player's acceleration/ускорение
    /// </summary>
    public float acceleration;

    /// <summary>
    /// Player's damage/урон
    /// </summary>
    public float damage;

    /// <summary>
    /// Player's attack/сила
    /// </summary>
    public float attack;

    /// <summary>
    /// Player's health/здоровье
    /// </summary>
    public float health;

    /// <summary
    /// Player's manna/мана
    /// </summary>
    public float manna;

    /// <summary>
    /// Player's jump force/сила прыжка
    /// </summary>
    public float jumpForce;

    /// <summary
    /// Player's armor/броня
    /// </summary>
    public float armor;

    /// <summary>
    /// Player's direction movement/направление движения
    /// </summary>
    public float movement;

    /// <summary>
    /// Player's speed/скорость
    /// </summary>
    public float speed;

    /// <summary>
    /// Player's stamina/выносливость
    /// </summary>
    public float stamina;

    /// <summary>
    /// Indicates if the player is currently attacking/показывает, атакует ли игрок 
    /// </summary>
    public bool isAttack;

    /// <sumary>
    /// Indicates if the player is currently block/показывает, блокирует ли игрок
    /// </sumary>
    public bool isBlock;

    /// <summary>
    /// Indicates if the player is currently dead/показывает, мертв ли игрок
    /// </summary>
    public bool isDead;

    /// <summary>
    /// Indicates if the player is currently jumping/показывает, прыгает ли игрок
    /// </summary>
    public bool isJump;

    /// <summary>
    /// Number of enemies killed by the player/количество убитых врагов игроком
    /// </summary> 
    public int killCount;

    public PlayerStatistics()
    {
        this.acceleration = 1f;
        this.damage = 0f;
        this.attack = 5f;
        this.health = 100f;
        this.manna = 100f;
        this.jumpForce = 500f;
        this.armor = 0f;
        this.movement = 0;
        this.speed = 500f;
        this.stamina = 100f;
        this.isAttack = false;
        this.isBlock = false;
        this.isDead = false;
        this.isJump = false;
        this.killCount = 0;
    }
}