using System;

public class MobeStatistics
{
    Random random = new Random();

    /// <summary>
    /// ��������
    /// </summary>
    public float speed;
    /// <summary>
    /// ��
    /// </summary>
    public float xp;
    /// <summary>
    /// ����
    /// </summary>
    public float force;

    public MobeStatistics() 
    {
        speed = random.Next(90, 110)/100;
        xp = random.Next(75, 150);
        force = random.Next(3, 7);
    }
}
