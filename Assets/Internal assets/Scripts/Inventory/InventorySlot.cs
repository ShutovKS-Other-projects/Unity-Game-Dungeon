using System;
using UnityEngine;
using Item;
namespace Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemType[] allowedItems = new ItemType[0];
        
        [NonSerialized] public UserInterface Parent;
        [NonSerialized] public GameObject SlotDisplay;

        [NonSerialized] public Action<InventorySlot> OnAfterUpdated;
        [NonSerialized] public Action<InventorySlot> OnBeforeUpdated;
        
        public Item.Item item = new Item.Item();
        public int amount;
        
        public ItemObject GetItemObject()
        {
            return item.Id >= 0 ? Parent.inventory.database.ItemObjects[item.Id] : null;
        }
        
        public ItemObject ItemObject
        {
            get
            {
                return item.Id >= 0 ? Parent.inventory.database.ItemObjects[item.Id] : null;
            }
        }
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
            if (allowedItems.Length <= 0 || itemObject == null || itemObject.data.Id < 0)
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