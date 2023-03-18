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
        /// <summary>
        /// Путь сохранения
        /// </summary>
        public string savePath;

        /// <summary>
        /// База данных предметов
        /// </summary>
        public ItemDatabaseObjects database;

        /// <summary>
        /// Тип инвентаря
        /// </summary>
        public InterfaceType type;

        [SerializeField] private Inventory container = new();

        public InventorySlot[] GetSlots
        {
            get => container.slots;
            set => container.slots = value;
        }

        /// <summary>
        /// Количество пустых слотов
        /// </summary>
        private int EmptySlotCount
        {
            get
            {
                var counter = 0;
                foreach (var inventorySlot in GetSlots)
                {
                    if (inventorySlot.item.id <= -1)
                    {
                        counter++;
                    }
                }

                return counter;
            }
        }

        /// <summary>
        /// Добавление предмета
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool AddItem(Item.Item item, int amount)
        {
            if (EmptySlotCount <= 0)
                return false;

            var slot = FindItemOnInventory(item);
            if (!database.itemObjects[item.id].stackable || slot == null)
            {
                GetEmptySlot().UpdateSlot(item, amount);
                return true;
            }

            slot.AddAmount(amount);
            return true;
        }

        /// <summary>
        /// Найти предмет в инвентаре
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private InventorySlot FindItemOnInventory(Item.Item item)
        {
            foreach (var inventorySlot in GetSlots)
            {
                if (inventorySlot.item.id == item.id)
                {
                    return inventorySlot;
                }
            }

            return null;
        }

        /// <summary>
        /// Есть ли в инвентаре такой предмет
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsItemInInventory(ItemObject item)
        {
            foreach (var inventorySlot in GetSlots)
            {
                if (inventorySlot.item.id == item.data.id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Получить первый пустой слот
        /// </summary>
        /// <returns>Пустой слот в инвенторе</returns>
        private InventorySlot GetEmptySlot()
        {
            foreach (var inventorySlot in GetSlots)
            {
                if (inventorySlot.item.id <= -1)
                {
                    return inventorySlot;
                }
            }

            return null;
        }

        /// <summary>
        /// Поменять местами предметы
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        public static void SwapItems(InventorySlot item1, InventorySlot item2)
        {
            if (item1 == item2)
                return;

            if (!item2.CanPlaceInSlot(item1.GetItemObject()) || !item1.CanPlaceInSlot(item2.GetItemObject()))
                return;

            var temp = new InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }

        /// <summary>
        /// Сохранить инвентарь
        /// </summary>
        [ContextMenu("Save")]
        public void Save()
        {
            //string saveData = JsonUtility.ToJson(container, true);
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            //bf.Serialize(file, saveData);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create,
                FileAccess.Write);
            formatter.Serialize(stream, container);
            stream.Close();
        }

        /// <summary>
        /// Загрузить сохраненный инвентарь
        /// </summary>
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
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open,
                FileAccess.Read);
            var newContainer = (Inventory)formatter.Deserialize(stream);
            for (var i = 0; i < GetSlots.Length; i++)
            {
                GetSlots[i].UpdateSlot(newContainer.slots[i].item, newContainer.slots[i].amount);
            }

            stream.Close();
        }

        /// <summary>
        /// Очистить инвентарь
        /// </summary>
        [ContextMenu("Clear")]
        public void Clear()
        {
            container.Clear();
        }
    }
}