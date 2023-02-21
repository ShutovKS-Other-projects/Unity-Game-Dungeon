using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Item;
using UnityEngine;
using UnityEngine.Serialization;
namespace Inventory
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public string savePath;
        public ItemDatabaseObjects database;
        public InterfaceType type;
        [SerializeField] private Inventory container = new Inventory();
        public InventorySlot[] GetSlots => container.slots;

        public bool AddItem(Item.Item item, int amount)
        {
            if (EmptySlotCount <= 0)
                return false;

            var slot = FindItemOnInventory(item);
            if (!database.ItemObjects[item.Id].stackable || slot == null)
            {
                GetEmptySlot().UpdateSlot(item, amount);
                return true;
            }
            slot.AddAmount(amount);
            return true;
        }

        public int EmptySlotCount
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < GetSlots.Length; i++)
                {
                    if (GetSlots[i].item.Id <= -1)
                    {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public InventorySlot FindItemOnInventory(Item.Item item)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id == item.Id)
                {
                    return GetSlots[i];
                }
            }
            return null;
        }

        public bool IsItemInInventory(ItemObject item)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id == item.data.Id)
                {
                    return true;
                }
            }
            return false;
        }

        public InventorySlot GetEmptySlot()
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id <= -1)
                {
                    return GetSlots[i];
                }
            }
            return null;
        }

        public static void SwapItems(InventorySlot item1, InventorySlot item2)
        {
            if (!item2.CanPlaceInSlot(item1.ItemObject) || !item1.CanPlaceInSlot(item2.ItemObject) || item1 == item2)
                return;
            
            var temp = new InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }

        [ContextMenu("Save")]
        public void Save()
        {
            //string saveData = JsonUtility.ToJson(container, true);
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            //bf.Serialize(file, saveData);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, container);
            stream.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            if (!File.Exists(string.Concat(Application.persistentDataPath, savePath)))
                return;
            
            //var bf = new BinaryFormatter();
            //var file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), container);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < GetSlots.Length; i++)
            {
                GetSlots[i].UpdateSlot(newContainer.slots[i].item, newContainer.slots[i].amount);
            }
            stream.Close();
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            container.Clear();
        }
    }

}
