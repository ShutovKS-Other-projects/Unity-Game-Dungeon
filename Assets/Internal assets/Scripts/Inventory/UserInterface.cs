using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Inventory
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class UserInterface : MonoBehaviour
    {
        public GameObject inventoryPrefab;
        public InventoryObject inventory;
        private InventoryObject _previousInventory;
        protected Dictionary<GameObject, InventorySlot> SlotsOnInterface = new Dictionary<GameObject, InventorySlot>();

        public void OnEnable()
        {
            CreateSlots();
            foreach (var inventorySlot in inventory.GetSlots)
            {
                inventorySlot.Parent = this;
                inventorySlot.OnAfterUpdated += OnSlotUpdate;
            }
            AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
            AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
        }

        protected abstract void CreateSlots();

        private void UpdateInventoryLinks()
        {
            int i = 0;
            foreach (var key in SlotsOnInterface.Keys.ToList())
            {
                SlotsOnInterface[key] = inventory.GetSlots[i];
                i++;
            }
        }

        protected abstract void AllSlotsUpdate();

        private void OnSlotUpdate(InventorySlot slot)
        {
            if (slot.item.Id >= 0)
            {
                slot.SlotDisplay.transform.GetChild(0).GetComponent<Image>().sprite = slot.ItemObject.uiDisplay;
                slot.SlotDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slot.SlotDisplay.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                slot.SlotDisplay.transform.GetChild(2).GetComponent<Text>().text = slot.amount == 1 ? "" : slot.amount.ToString("n0");
            }
            else
            {
                slot.SlotDisplay.transform.GetChild(0).GetComponent<Image>().sprite = null;
                slot.SlotDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                slot.SlotDisplay.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slot.SlotDisplay.transform.GetChild(2).GetComponent<Text>().text = "";
            }
        }

        public void Update()
        {
            if (inventory != _previousInventory)
            {
                UpdateInventoryLinks();
            }
            _previousInventory = inventory;
        }

        protected static void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            var trigger = obj.GetComponent<EventTrigger>();
            if (!trigger)
            {
                Debug.LogWarning("No EventTrigger component found!");
                return;
            }
            var eventTrigger = new EventTrigger.Entry {
                eventID = type
            };
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }
        protected static void OnEnter(GameObject obj)
        {
            MouseData.SlotHoveredOver = obj;
        }
        protected static void OnExit(GameObject obj)
        {
            MouseData.SlotHoveredOver = null;
        }
        private static void OnEnterInterface(GameObject obj)
        {
            MouseData.InterfaceMouseIsOver = obj.GetComponent<UserInterface>();
        }
        private static void OnExitInterface(GameObject obj)
        {
            MouseData.InterfaceMouseIsOver = null;
        }
        protected void OnDragStart(GameObject obj)
        {
            MouseData.TempItemBeingDragged = CreateTempItem(obj);
        }
        private GameObject CreateTempItem(GameObject obj)
        {
            if (SlotsOnInterface[obj].item.Id < 0)
                return null;

            GameObject tempItem = null;
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(30, 30);
            tempItem.transform.SetParent(transform.parent.parent);
            var img = tempItem.AddComponent<Image>();
            img.sprite = SlotsOnInterface[obj].ItemObject.uiDisplay;
            img.raycastTarget = false;
            return tempItem;
        }
        protected void OnDragEnd(GameObject obj)
        {
            Destroy(MouseData.TempItemBeingDragged);

            if (MouseData.InterfaceMouseIsOver == null)
            {
                SlotsOnInterface[obj].RemoveItem();
                return;
            }
            if (MouseData.SlotHoveredOver)
            {
                var mouseHoverSlotData = MouseData.InterfaceMouseIsOver.SlotsOnInterface[MouseData.SlotHoveredOver];
                InventoryObject.SwapItems(SlotsOnInterface[obj], mouseHoverSlotData);
            }
        }
        protected static void OnDrag(GameObject obj)
        {
            if (MouseData.TempItemBeingDragged != null)
            {
                MouseData.TempItemBeingDragged.GetComponent<RectTransform>().position = UnityEngine.Input.mousePosition;
            }
        }
    }
}
