using System;
using System.Linq;
using Old.Item;

namespace Old.Inventory
{
    [Serializable]
    public class Inventory
    {
        public InventorySlot[] slots;

        public void Clear()
        {
            foreach (var inventorySlot in slots)
            {
                inventorySlot.item = new Item.Item();
                inventorySlot.amount = 0;
            }
        }

        public bool ContainsItem(ItemObject itemObject) =>
            Array.Find(slots, inventorySlot => inventorySlot.item.id == itemObject.data.id) != null;

        public bool ContainsItem(int id) => slots.FirstOrDefault(inventorySlot => inventorySlot.item.id == id) != null;
    }
}