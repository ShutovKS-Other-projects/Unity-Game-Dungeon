namespace Player.Characteristics
{
    public class CharacteristicBase : ICharacteristic
    {
        public int Value { get; set; }

        public CharacteristicBase(int value)
        {
            Value = value;
        }

        public void AddValue(int value)
        {
            Value += value;
        }

        public void AddValuePercentage(float value)
        {
            Value += (int)(Value * value / 100);
        }

        public void SetValue(int value)
        {
            Value = value;
        }
    }
}