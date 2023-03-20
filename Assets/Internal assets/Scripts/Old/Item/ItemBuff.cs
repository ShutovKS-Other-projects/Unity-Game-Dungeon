using System;
using Old.Other;
using UnityEngine;

namespace Old.Item
{
    
    [Serializable]
    public class ItemBuff : IModifiers
    {
        /// <summary>
        /// Стат, к которому применяется баф
        /// </summary>
        public Attributes stat;
        
        /// <summary>
        /// Значение бафа
        /// </summary>
        public float value;
        
        [SerializeField] private float min;
        [SerializeField] private float max;
        
        /// <summary>
        /// Минимальное значение бафа
        /// </summary>
        public float Min => min;
        
        /// <summary>
        /// Максимальное значение бафа
        /// </summary>
        public float Max => max;
        
        /// <summary>
        /// Конструктор бафа предмета
        /// </summary>
        /// <param name="min"> Минимальное значение бафа </param>
        /// <param name="max"> Максимальное значение бафа </param>
        /// <param name="level"> Уровень предмета </param>
        public ItemBuff(float min, float max, int level) 
        {
            this.min = min;
            this.max = max;
            GenerateField(level);
        }

        /// <summary>
        /// Добавление значения бафа к стату 
        /// </summary>
        /// <param name="v"> Стат </param>
        public void AddValue(ref float v)
        {
            v += value;
        }

        /// <summary>
        /// Генерация значения бафа
        /// </summary>
        public void GenerateField(int level)
        {
            value = (float)Math.Round(UnityEngine.Random.Range(min, max) * level, 2);
        }
    }
}
