using System;
using UnityEngine;
using Item;
namespace Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemType[] allowedItems = Array.Empty<ItemType>();
        
        [NonSerialized] public UserInterface Parent;
        [NonSerialized] public GameObject SlotDisplay;

        [NonSerialized] public Action<InventorySlot> OnAfterUpdated;
        [NonSerialized] public Action<InventorySlot> OnBeforeUpdated;
        
        public Item.Item item = new Item.Item();
        public int amount;
        
        public ItemObject GetItemObject() => item.id >= 0 ? Parent.inventory.database.ItemObjects[item.id] : null;

        public ItemObject ItemObject => item.id >= 0 ? Parent.inventory.database.ItemObjects[item.id] : null;
        public InventorySlot() => UpdateSlot(new Item.Item(), 0);
        public InventorySlot(Item.Item item, int amount) => UpdateSlot(item, amount);
        public void RemoveItem() => UpdateSlot(new Item.Item(), 0);
        public void AddAmount(int value) => UpdateSlot(item, amount += value);

        public void UpdateSlot(Item.Item itemValue, int amountValue)
        {
            OnBeforeUpdated?.Invoke(this);
            item = itemValue;
            amount = amountValue;
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