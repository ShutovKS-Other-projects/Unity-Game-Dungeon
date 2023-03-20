using System;
using Old.Item;
using UnityEngine;

namespace Old.Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemType[] allowedItems = Array.Empty<ItemType>();

        [NonSerialized] public UserInterface Parent;
        [NonSerialized] public GameObject SlotDisplay;

        [NonSerialized] public Action<InventorySlot> OnAfterUpdated;
        [NonSerialized] public Action<InventorySlot> OnBeforeUpdated;

        public Item.Item item = new();
        public int amount;

        public ItemObject GetItemObject() => item.id >= 0 ? Parent.inventory.database.itemObjects[item.id] : null;

        public InventorySlot() => UpdateSlot(new Item.Item(), 0);
        public InventorySlot(Item.Item item, int amount) => UpdateSlot(item, amount);
        
        public void AddAmount(int value) => UpdateSlot(item, amount += value);
        public void RemoveItem() => UpdateSlot(new Item.Item(), 0);

        public void UpdateSlot(Item.Item itemValue, int amountValue)
        {
            OnBeforeUpdated?.Invoke(this);
            item = itemValue;
            amount = amountValue;
            OnAfterUpdated?.Invoke(this);
        }
        
        public void UpdateSlot()
        {
            OnBeforeUpdated?.Invoke(this);
            OnAfterUpdated?.Invoke(this);
        }

        public bool CanPlaceInSlot(ItemObject itemObject)
        {
            if (allowedItems.Length <= 0 || itemObject == null || itemObject.data.id < 0)
                return true;
            foreach (var itemType in allowedItems)
            {
                if (itemObject.type == itemType)
                    return true;
            }
            return false;
        }
    }
}
