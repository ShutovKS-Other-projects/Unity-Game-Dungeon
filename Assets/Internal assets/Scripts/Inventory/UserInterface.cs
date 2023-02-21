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
        public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

        public void OnEnable()
        {
            CreateSlots();
            for (int i = 0; i < inventory.GetSlots.Length; i++)
            {
                inventory.GetSlots[i].parent = this;
                inventory.GetSlots[i].onAfterUpdated += OnSlotUpdate;
            }
            AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
            AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
        }

        public abstract void CreateSlots();
        public abstract void AllSlotsUpdate();

        public void UpdateInventoryLinks()
        {
            int i = 0;
            foreach (var key in slotsOnInterface.Keys.ToList())
            {
                slotsOnInterface[key] = inventory.GetSlots[i];
                i++;
            }
        }

        private void OnSlotUpdate(InventorySlot _slot)
        {
            if (_slot.item.Id >= 0)
            {
                _slot.slotDisplay.transform.GetChild(0).GetComponent<Image>().sprite = _slot.ItemObject.uiDisplay;
                _slot.slotDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                _slot.slotDisplay.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                _slot.slotDisplay.transform.GetChild(2).GetComponent<Text>().text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
            }
            else
            {
                _slot.slotDisplay.transform.GetChild(0).GetComponent<Image>().sprite = null;
                _slot.slotDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                _slot.slotDisplay.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                _slot.slotDisplay.transform.GetChild(2).GetComponent<Text>().text = "";
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

        protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
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
        public void OnEnter(GameObject obj)
        {
            MouseData.SlotHoveredOver = obj;
        }
        public void OnExit(GameObject obj)
        {
            MouseData.SlotHoveredOver = null;
        }
        public void OnEnterInterface(GameObject obj)
        {
            MouseData.InterfaceMouseIsOver = obj.GetComponent<UserInterface>();
        }
        public void OnExitInterface(GameObject obj)
        {
            MouseData.InterfaceMouseIsOver = null;
        }
        public void OnDragStart(GameObject obj)
        {
            MouseData.TempItemBeingDragged = CreateTempItem(obj);
        }
        public GameObject CreateTempItem(GameObject obj)
        {
            GameObject tempItem = null;
            if (slotsOnInterface[obj].item.Id >= 0)
            {
                tempItem = new GameObject();
                var rt = tempItem.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(30, 30);
                tempItem.transform.SetParent(transform.parent.parent);
                var img = tempItem.AddComponent<Image>();
                img.sprite = slotsOnInterface[obj].ItemObject.uiDisplay;
                img.raycastTarget = false;
            }
            return tempItem;
        }
        public void OnDragEnd(GameObject obj)
        {
            Destroy(MouseData.TempItemBeingDragged);

            if (MouseData.InterfaceMouseIsOver == null)
            {
                slotsOnInterface[obj].RemoveItem();
                return;
            }
            if (MouseData.SlotHoveredOver)
            {
                InventorySlot mouseHoverSlotData = MouseData.InterfaceMouseIsOver.slotsOnInterface[MouseData.SlotHoveredOver];
                InventoryObject.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
            }
        }
        public void OnDrag(GameObject obj)
        {
            if (MouseData.TempItemBeingDragged != null)
            {
                MouseData.TempItemBeingDragged.GetComponent<RectTransform>().position = UnityEngine.Input.mousePosition;
            }
        }
    }
}
