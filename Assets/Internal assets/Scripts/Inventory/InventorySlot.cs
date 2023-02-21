using System;
using UnityEngine;
using Item;
namespace Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemType[] AllowedItems = new ItemType[0];
        
        [NonSerialized] public UserInterface parent;
        [NonSerialized] public GameObject slotDisplay;

        [NonSerialized] public Action<InventorySlot> onAfterUpdated;
        [NonSerialized] public Action<InventorySlot> onBeforeUpdated;
        
        public Item.Item item = new Item.Item();
        public int amount;
        
        public ItemObject GetItemObject()
        {
            return item.Id >= 0 ? parent.inventory.database.ItemObjects[item.Id] : null;
        }
        
        public ItemObject ItemObject
        {
            get
            {
                if (item.Id >= 0)
                {
                    return parent.inventory.database.ItemObjects[item.Id];
                }
                return null;
            }
        }
        public InventorySlot() => UpdateSlot(new Item.Item(), 0);
        public InventorySlot(Item.Item item, int amount) => UpdateSlot(item, amount);
        public void RemoveItem() => UpdateSlot(new Item.Item(), 0);
        public void AddAmount(int value) => UpdateSlot(item, amount += value);

        public void UpdateSlot(Item.Item itemValue, int amountValue)
        {
            onBeforeUpdated?.Invoke(this);
            item = itemValue;
            amount = amountValue;
            onAfterUpdated?.Invoke(this);
        }
        
        public bool CanPlaceInSlot(ItemObject itemObject)
        {
            if (AllowedItems.Length <= 0 || itemObject == null || itemObject.data.Id < 0)
                return true;
            for (int i = 0; i < AllowedItems.Length; i++)
            {
                if (itemObject.type == AllowedItems[i])
                    return true;
            }
            return false;
        }
    }
}