using System;

public class MobeStatistics
{
    Random random = new Random();

    /// <summary>
    /// Скорость
    /// </summary>
    public float speed;
    /// <summary>
    /// ХП
    /// </summary>
    public float xp;
    /// <summary>
    /// Сила
    /// </summary>
    public float force;

    public MobeStatistics() 
    {
        speed = random.Next(90, 110)/100;
        xp = random.Next(75, 150);
        force = random.Next(3, 7);
    }
}
