using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Inventory
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class UserInterface : MonoBehaviour
    {
        public GameObject inventoryPrefab;
        public GameObject panelInfoItemPrefab;
        [NonSerialized] public GameObject panelInfoItem;
        public InventoryObject inventory;
        protected Dictionary<GameObject, InventorySlot> SlotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        private InventoryObject _previousInventory;

        #region Unity Methods

        public void Start()
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

        public void Update()
        {
            if (inventory != _previousInventory)
            {
                UpdateInventoryLinks();
            }

            _previousInventory = inventory;
        }

        #endregion

        #region Inventory Methods

        private void UpdateInventoryLinks()
        {
            var i = 0;
            foreach (var key in SlotsOnInterface.Keys.ToList())
            {
                SlotsOnInterface[key] = inventory.GetSlots[i];
                i++;
            }
        }

        protected abstract void CreateSlots();

        public void AllSlotsInInventoryUpdate() => AllSlotsUpdate();

        private void AllSlotsUpdate()
        {
            foreach (var inventorySlot in inventory.GetSlots)
            {
                inventorySlot.UpdateSlot(inventorySlot.item, inventorySlot.amount);
            }
        }

        private static void OnSlotUpdate(InventorySlot slot)
        {
            if (slot.item.id >= 0)
            {
                slot.SlotDisplay.transform.GetChild(0).GetComponent<Image>().sprite = slot.ItemObject.uiDisplay;
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

        protected void PopupPanelItemInformation(GameObject obj)
        {
            if (SlotsOnInterface[obj].amount == 0)
                return;

            panelInfoItem = Instantiate(panelInfoItemPrefab, obj.transform.parent);
            panelInfoItem.transform.position = obj.transform.position + OffsetPositionPanelInfoItem();

            var buffsList = "";
            foreach (var buff in SlotsOnInterface[obj].item.buffs)
            {
                if (buff.value != 0)
                    buffsList += $"{buff.stat}: {buff.value} \n";
            }

            panelInfoItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buffsList;
        }

        protected void HiddenPanelItemInformation(GameObject obj)
        {
            Destroy(panelInfoItem);
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

        GameObject CreateTempItem(GameObject obj)
        {
            if (SlotsOnInterface[obj].item.id < 0)
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

        #endregion

        /*
            Pointer Enter	- Указатель Введите     |   Обработчик ввода указателя при вводе указателя.         
            Pointer Exit	- Указатель Выход       |   Обработчик выхода указателя при выходе указателя.
            Pointer Down	- Указатель вниз        |   Обработчик указателя вниз при указателе вниз.
            Pointer Up		- Указатель вверх       |   Обработчик указателя вверх при указателе вверх.
            Pointer Click	- Указатель Нажмите     |   Обработчик щелчка указателя при щелчке указателя.
            Drag			- Перетащите            |   Обработчик перетаскивания при перетаскивании
            Drop			- падение               |   Обработчик сброса при сбросе
            Scroll			- Прокрутите            |   Обработчик прокрутки при прокрутке.
            Update Selected	- Обновить выбранное    |   Обновить выбранный обработчик при обновлении выбранного.
            Select			- выбрать               |   Выберите обработчик при выборе
            Deselect		- выбрать               |   Обработчик отмены выбора при отмене выбора.
            Move			- двигаться             |   Обработчик перемещения при перемещении
            Initialize Potential Drag           	|   Инициализировать потенциальное перетаскивание.
            Begin Drag		- Начать перетаскивание |   Обработчик перетаскивания при перетаскивании.
            End Drag		- Конец перетаскивания  |   Обработчик перетаскивания при перетаскивании.
            Submit			- представить           |   Выберите обработчик при выборе.
            Cancel			- Отмена                |   Обработчик прокрутки при прокрутке.
         */
    }
}