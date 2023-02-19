using Item;
using UnityEngine;
namespace Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemType[] AllowedItems = new ItemType[0];
        [System.NonSerialized] public UserInterface parent;
        [System.NonSerialized] public GameObject slotDisplay;
        [System.NonSerialized] public SlotUpdated OnAfterUpdate;
        [System.NonSerialized] public SlotUpdated OnBeforeUpdate;
        public Item.Item item = new Item.Item();
        public int amount;
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
        public InventorySlot()
        {
            UpdateSlot(new Item.Item(), 0);
        }
        public InventorySlot(Item.Item _item, int _amount)
        {
            UpdateSlot(_item, _amount);
        }
        public void AddAmount(int value)
        {
            UpdateSlot(item, amount += value);
        }
        public void UpdateSlot(Item.Item _item, int _amount)
        {
            if (OnBeforeUpdate != null)
            {
                OnBeforeUpdate.Invoke(this);
            }
            item = _item;
            amount = _amount;
            if (OnAfterUpdate != null)
            {
                OnAfterUpdate.Invoke(this);
            }
        }
        public void RemoveItem()
        {
            item = new Item.Item();
            amount = 0;
        }
        public bool CanPlaceInSlot(ItemObject _itemObject)
        {
            if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0)
            {
                return true;
            }
            for (int i = 0; i < AllowedItems.Length; i++)
            {
                if (_itemObject.type == AllowedItems[i])
                {
                    return true;
                }
            }
            return false;
        }
    }
}