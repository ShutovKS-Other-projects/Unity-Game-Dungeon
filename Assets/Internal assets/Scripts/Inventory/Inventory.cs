using System;
using System.Linq;
using Item;

namespace Inventory
{
    [System.Serializable]
    public class Inventory
    {
        public InventorySlot[] slots = new InventorySlot[48];
        public void Clear()
        {
            foreach (var inventorySlot in slots)
            {
                inventorySlot.item = new Item.Item();
                inventorySlot.amount = 0;
            }
        }

        public bool ContainsItem(ItemObject itemObject)
        {
            return Array.Find(slots, i => i.item.Id == itemObject.data.Id) != null;
        }

        public bool ContainsItem(int id)
        {
            return slots.FirstOrDefault(i => i.item.Id == id) != null;
        }
    }
}
