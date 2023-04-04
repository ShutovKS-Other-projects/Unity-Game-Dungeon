namespace Player.Characteristics
{
    public class CharacteristicHealth : ICharacteristic
    {
        public int Value { get; set; }
        public int ValueMax { get; set; }

        public CharacteristicHealth(int value)
        {
            Value = value;
            ValueMax = value;
        }

        public void AddValue(int value)
        {
            Value += value;
        }

        public void AddValueMax(int value)
        {
            ValueMax += value;
        }

        public void AddValuePercentage(float value)
        {
            Value += (int)(Value * value / 100);
        }

        public void AddValueMaxPercentage(float value)
        {
            ValueMax += (int)(ValueMax * value / 100);
        }

        public void SetValue(int value)
        {
            Value = value;
        }

        public void SetValueMax(int value)
        {
            ValueMax = value;
        }
    }
}