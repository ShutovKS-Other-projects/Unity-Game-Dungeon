using System;

public class MobeStatistics
{
    Random random = new Random();

    public float speed;
    public float xp;
    public float force;

    public MobeStatistics() 
    {
        this.speed = random.Next(90, 110)/100;
        this.xp = random.Next(75, 150);
        this.force = random.Next(3, 7);
    }
}
