using UnityEngine.Serialization;
namespace Item
{
    [System.Serializable]
    public class Item
    {
        public string name;
        public int id;
        public ItemBuff[] buffs;
        public Item(ItemObject item)
        {
            name = item.name;
            id = item.data.id;
            buffs = new ItemBuff[item.data.buffs.Length];
            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i] = new ItemBuff(item.data.buffs[i].Min, item.data.buffs[i].Max) {
                    stat = item.data.buffs[i].stat
                };
            }
        }
        public Item()
        {
            name = "";
            id = -1;
        }
    }
}
