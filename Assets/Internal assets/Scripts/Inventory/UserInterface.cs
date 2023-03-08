using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Item;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

namespace Inventory
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class UserInterface : MonoBehaviour
    {
        public GameObject inventoryPrefab;
        public GameObject panelInfoItemPrefab;
        public Dictionary<GameObject, InventorySlot> SlotsOnInterface = new();
        public InventoryObject inventory;
        private GameObject _panelInfoItem;
        private InventoryObject _previousInventory;
        [SerializeField] private GameObject itemPrefab;

        #region Unity Methods

        public void Start()
        {
            foreach (var inventorySlot in inventory.GetSlots)
            {
                inventorySlot.Parent = this;
                inventorySlot.OnAfterUpdated += OnSlotUpdate;
            }

            CreateSlots();

            AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
            AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
        }

        public void Update()
        {
            if (inventory != _previousInventory)
            {
                UpdateInventoryLinks();
            }

            _previousInventory = inventory;
        }

        #endregion

        public void AllSlotsUpdate()
        {
            CreateSlots();
        }

        #region Inventory Methods

        private void UpdateInventoryLinks()
        {
            for (var i = 0; i < SlotsOnInterface.Keys.ToList().Count; i++)
            {
                var key = SlotsOnInterface.Keys.ToList()[i];
                SlotsOnInterface[key] = inventory.GetSlots[i];
            }
        }

        protected abstract void CreateSlots();

        private static void OnSlotUpdate(InventorySlot slot)
        {
            if (slot.item.id >= 0)
            {
                slot.SlotDisplay.transform.GetChild(0).GetComponent<Image>().sprite = slot.GetItemObject().uiDisplay;
                slot.SlotDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slot.SlotDisplay.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                slot.SlotDisplay.transform.GetChild(2).GetComponent<Text>().text =
                    slot.amount == 1 ? "" : slot.amount.ToString("n0");
            }
            else
            {
                slot.SlotDisplay.transform.GetChild(0).GetComponent<Image>().sprite = null;
                slot.SlotDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                slot.SlotDisplay.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slot.SlotDisplay.transform.GetChild(2).GetComponent<Text>().text = "";
            }
        }

        #endregion

        #region AddEvent

        protected static void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            var trigger = obj.GetComponent<EventTrigger>();
            if (!trigger)
            {
                Debug.LogWarning("No EventTrigger component found!");
                return;
            }

            var eventTrigger = new EventTrigger.Entry
            {
                eventID = type
            };
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        #endregion

        #region Events Slot

        protected static void OnEnter(GameObject obj)
        {
            MouseData.SlotHoveredOver = obj;
        }

        protected static void OnExit(GameObject obj)
        {
            MouseData.SlotHoveredOver = null;
        }

        protected void OnDragStart(GameObject obj)
        {
            MouseData.TempItemBeingDragged = CreateTempItem(obj);
        }

        protected void OnDragEnd(GameObject obj)
        {
            Destroy(MouseData.TempItemBeingDragged);

            if (MouseData.InterfaceMouseIsOver == null)
            {
                DropItem(SlotsOnInterface[obj]);
                SlotsOnInterface[obj].RemoveItem();
                return;
            }

            if (MouseData.SlotHoveredOver)
            {
                var mouseHoverSlotData = MouseData.InterfaceMouseIsOver.SlotsOnInterface[MouseData.SlotHoveredOver];
                InventoryObject.SwapItems(SlotsOnInterface[obj], mouseHoverSlotData);
            }
        }

        private void DropItem(InventorySlot slot)
        {
            var item = slot.item;

            if (item.id < 0) return;

            GameObject itemDrop;
            if (slot.GetItemObject().swordModel == null)
            {
                itemDrop = Instantiate(itemPrefab);
                itemDrop.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = slot.GetItemObject().uiDisplay;
            }
            else
            {
                itemDrop = Instantiate(slot.GetItemObject().swordModel);
            }

            itemDrop.layer = LayerMask.NameToLayer("Interactable");
            itemDrop.AddComponent<Rigidbody>();
            itemDrop.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            itemDrop.GetComponent<Rigidbody>().constraints =
                RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            itemDrop.transform.position =
                GameObject.FindWithTag("Player").transform.position + new Vector3(Random.Range(-1,2), 0, Random.Range(-1,2))*Random.Range(1f,2f) + new Vector3(0.75f,1.5f,0);
            itemDrop.GetComponent<GroundItem>().item = slot.GetItemObject();
        }

        protected static void OnDrag(GameObject obj)
        {
            if (MouseData.TempItemBeingDragged != null)
            {
                MouseData.TempItemBeingDragged.GetComponent<RectTransform>().position = UnityEngine.Input.mousePosition;
            }
        }

        protected void PopupPanelItemInformation(GameObject obj)
        {
            if (SlotsOnInterface[obj].amount == 0)
                return;

            _panelInfoItem = Instantiate(panelInfoItemPrefab, obj.transform.parent);
            _panelInfoItem.transform.position = obj.transform.position + OffsetPositionPanelInfoItem();

            var buffsList = "";
            foreach (var buff in SlotsOnInterface[obj].item.buffs)
            {
                if (buff.value != 0)
                    buffsList += $"{buff.stat}: {buff.value} \n";
            }

            _panelInfoItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buffsList;
        }

        protected void HiddenPanelItemInformation(GameObject obj)
        {
            Destroy(_panelInfoItem);
        }

        #endregion

        #region Events Interface

        static void OnEnterInterface(GameObject obj)
        {
            MouseData.InterfaceMouseIsOver = obj.GetComponent<UserInterface>();
        }

        static void OnExitInterface(GameObject obj)
        {
            MouseData.InterfaceMouseIsOver = null;
        }

        #endregion

        #region Other Methods

        protected abstract Vector3 OffsetPositionPanelInfoItem();

        private GameObject CreateTempItem(GameObject obj)
        {
            if (SlotsOnInterface[obj].item.id < 0)
                return null;

            GameObject tempItem = null;
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(30, 30);
            tempItem.transform.SetParent(transform.parent.parent);
            var img = tempItem.AddComponent<Image>();
            img.sprite = SlotsOnInterface[obj].GetItemObject().uiDisplay;
            img.raycastTarget = false;
            return tempItem;
        }

        #endregion
    }
}