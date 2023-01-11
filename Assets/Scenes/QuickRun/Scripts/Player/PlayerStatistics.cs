public class PlayerStatistics
{
    /// <summary>
    /// Ускорение параметр
    /// </summary>
    public float acceleration = 1f;
    
    /// <summary>
    /// Урон параметр
    /// </summary>
    public float damage = 0f;

    /// <summary>
    /// Сила параметр
    /// </summary>
    public float force = 5f;

    /// <summary>
    /// ХП параметр
    /// </summary>
    public float health = 100f;
	
	/// <summary>
    /// Передвижение
    /// </summary>
    public float movement = 0;

    /// <summary>
    /// Скорость параметр
    /// </summary>
    public float speed = 500f;

    /// <summary>
    /// Выносливость параметр
    /// </summary>
    public float stamina = 100f;

    /// <summary>
    /// Атака состояние
    /// </summary>
    public bool isAttack = false;

    /// <summary>
    /// Смерть состояние
    /// </summary>
    public bool isDead = false;

    /// <summary>
    /// Прыжок состояние
    /// </summary>
    public bool isJump = false;
}