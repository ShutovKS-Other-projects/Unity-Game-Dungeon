namespace Player.Game.Characteristics
{
    public interface ICharacteristic
    {
        public int Value { get; set; }

        public int GetValue() => Value;

        public void AddValue(int value);

        public void AddValuePercentage(float value);

        public void SetValue(int value);
    }
}