using Old.Player;
using UnityEngine;

namespace Old.Item
{
    [System.Serializable]
    public class Item
    {
        
        /// <summary>
        /// ID предмета
        /// </summary>
        public int id;
        
        /// <summary>
        /// Имя предмета
        /// </summary>
        public string name;
        
        /// <summary>
        /// Уровень предмета
        /// </summary>
        public int level;
         
        /// <summary>
        /// Список баффов
        /// </summary>
        public ItemBuff[] buffs;

        /// <summary>
        /// Конструктор для создания предмета
        /// </summary>
        /// <param name="itemObject"> Базовый предмет </param>
        public Item(ItemObject itemObject)
        {
            name = itemObject.itemName;
            id = itemObject.data.id;

            level = GameObject.FindWithTag("Player").GetComponent<PlayerStatistic>().Level < 3
                ? Random.Range(1, 4)
                : GameObject.FindWithTag("Player").GetComponent<PlayerStatistic>().Level + Random.Range(-2, 3);

            buffs = new ItemBuff[itemObject.data.buffs.Length];
            for (var i = 0; i < buffs.Length; i++)
            {
                buffs[i] = new ItemBuff(itemObject.data.buffs[i].Min, itemObject.data.buffs[i].Max, level)
                {
                    stat = itemObject.data.buffs[i].stat
                };
            }
        }

        /// <summary>
        /// Конструктор для создания пустого предмета
        /// </summary>
        public Item()
        {
            name = "";
            id = -1;
        }
    }
}