using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Item;
using UnityEngine;
namespace Inventory
{

    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public string savePath;
        public ItemDatabaseObjects database;
        public InterfaceType type;
        public Inventory Container;
        public InventorySlot[] GetSlots { get { return Container.Slots; } }
        public bool AddItem(Item.Item _item, int _amount)
        {
            if (EmptySlotCount <= 0)
            {
                return false;
            }
            InventorySlot slot = FindItemOnInventory(_item);
            if (!database.ItemObjects[_item.Id].stackable || slot == null)
            {
                SetEmptySlot(_item, _amount);
                return true;
            }
            slot.AddAmount(_amount);
            return true;
        }

        public int EmptySlotCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < GetSlots.Length; i++)
                {
                    if (GetSlots[i].item.Id <= -1)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public InventorySlot FindItemOnInventory(Item.Item _item)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id == _item.Id)
                {
                    return GetSlots[i];
                }
            }
            return null;
        }

        public InventorySlot SetEmptySlot(Item.Item _item, int amount)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id <= -1)
                {
                    GetSlots[i].UpdateSlot(_item, amount);
                    return GetSlots[i];
                }
            }
            return null;
        }

        public void SwapItems(InventorySlot item1, InventorySlot item2)
        {
            if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
            {
                InventorySlot temp = new InventorySlot(item2.item, item2.amount);
                item2.UpdateSlot(item1.item, item1.amount);
                item1.UpdateSlot(temp.item, temp.amount);
            }
        }

        public void RemoveItem(Item.Item _item)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item == _item)
                {
                    GetSlots[i].UpdateSlot(null, 0);
                }
            }
        }


        [ContextMenu("Save")]
        public void Save()
        {
            //string saveData = JsonUtility.ToJson(this, true);   
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            //bf.Serialize(file, saveData);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, Container);
            stream.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
            {
                //BinaryFormatter bf = new BinaryFormatter();
                //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
                //JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), this);
                //file.Close();

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
                Inventory newContainer = (Inventory)formatter.Deserialize(stream);
                for (int i = 0; i < GetSlots.Length; i++)
                {
                    GetSlots[i].UpdateSlot(newContainer.Slots[i].item, newContainer.Slots[i].amount);
                }
                stream.Close();
            }
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            Container.Clear();
        }
    }

}