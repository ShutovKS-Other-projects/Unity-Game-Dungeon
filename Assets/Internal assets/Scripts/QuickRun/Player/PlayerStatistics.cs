public class PlayerStatistic
{
    string _class = "Никто";
    float _acceleration = 1f;
    float _attack = 5f;
    float _experience = 0f;
    float _health = 100f;
    float _jumpForce = 50f;
    int _сollectCrystal = 0;
    float _movement = 0f;
    float _speed = 6.2f;
    float _stamina = 100f;

    public string Class
    {
        get { return _class; }
    }
    public float Acceleration
    {
        get { return _acceleration; }
        set { _acceleration = value; }
    }
    public float Damage
    {
        get { return _attack; }
    }
    public float Experience
    {
        get { return _experience; }
        set { _experience = value; }
    }
    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }
    public float JumpForce
    {
        get { return _jumpForce; }
    }
    public int CollectCrystal
    {
        get { return _сollectCrystal; }
        set { _сollectCrystal = value; }
    }
    public float Movement
    {
        get { return _movement; }
        set { _movement = value; }
    }
    public float Speed
    {
        get { return _speed; }
    }
    public float Stamina
    {
        get { return _stamina; }
        set
        {
            if (value > 100)
            {
                _stamina = 100;
            }
            else if (value < 0)
            {
                _stamina = 0;
            }
            else if (value >= 0 && value <= 100)
            {
                _stamina = value;
            }
        }
    }

    public bool isAttack = false;
    public bool isBlock = false;
    public bool isJump = false;
    public bool isAction => isAttack || isBlock || isJump;
    public bool isDead = false;
    public bool isFatigue = false;
}