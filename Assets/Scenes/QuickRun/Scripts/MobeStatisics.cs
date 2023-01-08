using System;

public class MobeStatistics
{
    Random random = new Random();

    public float speed;
    public float health;
    public bool isDead;
    

    public MobeStatistics() 
    {
        this.speed = random.Next(90, 110)/100;
        this.health = random.Next(75, 150);
        this.isDead = false;
    }
}
