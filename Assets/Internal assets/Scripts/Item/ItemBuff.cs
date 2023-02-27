using System;
using UnityEngine;
using Other;
namespace Item
{
    [System.Serializable]
    public class ItemBuff : IModifiers
    {
        public Attributes stat;
        public float value;
        [SerializeField] private float min;
        public float Min => min;
        [SerializeField] private float max;
        public float Max => max;
        public ItemBuff(float min, float max)
        {
            this.min = min;
            this.max = max;
            GenerateField();
        }

        public void AddValue(ref float v)
        {
            v += value;
        }

        public void GenerateField()
        {
            value = (float)Math.Round(UnityEngine.Random.Range(min, max), 2);
        }
    }
}
