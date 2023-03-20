using System.Collections;
using System.Collections.Generic;
using Old.Inventory;
using Old.Item;
using Old.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Old.Chest
{
    public class Chest : MonoBehaviour
    {
        private UIController _uiController;

        #region Singleton

        public bool isGenerated;

        public int chestSize;

        public ItemObject[] itemsInChest;
        public List<Item.Item> generatedItems;

        public InventoryObject chestData;
        public DynamicInterface chestUI;

        public ItemDatabaseObjects itemDatabase;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _uiController = GameObject.Find("Canvas").GetComponent<UIController>();
            chestUI = GameObject.Find("Canvas").transform.GetChild(1).GetChild(2).GetComponent<DynamicInterface>();
        }

        private void OnEnable()
        {
            _uiController.InputGame += CloseChest;
            StartCoroutine(WaitOneFrame());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator WaitOneFrame()
        {
            yield return new WaitForSeconds(0.2f);
            GenerateItemsInChest();
            CreatedSlotsInUIInventory();
            UpdateChestData();
            chestUI.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!chestUI.gameObject.activeSelf) return;

            for (var i = 0; i < generatedItems.Count; i++)
            {
                var found = false;
                foreach (var inventorySlot in chestData.GetSlots)
                {
                    if (generatedItems[i] != inventorySlot.item) continue;
                    found = true;
                    break;
                }

                if (found) continue;
                generatedItems.Remove(generatedItems[i]);
            }
        }

        private void OnApplicationQuit()
        {
            chestData.Clear();
        }

        #endregion

        #region Chest Methods

        private void CloseChest()
        {
            _uiController.InputPlayerInfo -= CloseChest;
            chestUI.SlotsOnInterface.Clear();
            chestData.Clear();
            foreach (Transform child in chestUI.transform.GetChild(0))
            {
                Destroy(child.gameObject);
            }

            chestUI.gameObject.SetActive(false);
            gameObject.GetComponent<Chest>().enabled = false;
        }

        private void GenerateItemsInChest()
        {
            if (isGenerated) return;

            itemsInChest = new ItemObject[chestSize];

            for (var i = 0; i < chestSize; i++)
            {
                var itemObject = itemDatabase.itemObjects[Random.Range(0, itemDatabase.itemObjects.Length)];
                itemsInChest[i] = itemObject;
            }

            foreach (var itemObject in itemsInChest)
            {
                var item = new Item.Item(itemObject);
                generatedItems.Add(item);
            }

            isGenerated = true;
        }

        private void UpdateChestData()
        {
            foreach (var item in generatedItems)
            {
                chestData.AddItem(item, 1);
            }
        }

        private void CreatedSlotsInUIInventory()
        {
            chestUI.AllSlotsUpdate();
        }

        #endregion
    }
}