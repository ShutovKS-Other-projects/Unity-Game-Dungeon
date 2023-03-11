using System.Collections.Generic;
using Item;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Inventory
{
    public class DynamicInterface : UserInterface
    {
        [SerializeField] private Transform panelInventory;
        private const int X_START = -105;
        private const int Y_START = 75;
        private const int X_SPACE_BETWEEN_ITEM = 30;
        private const int Y_SPACE_BETWEEN_ITEM = 30;
        private const int NUMBER_OF_COLUMNS = 8;

        protected override void CreateSlots()
        {
            SlotsOnInterface = new Dictionary<GameObject, InventorySlot>();
            for (var i = 0; i < inventory.GetSlots.Length; i++)
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, panelInventory);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

                AddEvent(obj, EventTriggerType.PointerClick, delegate { });

                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
                AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });

                AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

                AddEvent(obj, EventTriggerType.PointerEnter, delegate { PopupPanelItemInformation(obj); });
                AddEvent(obj, EventTriggerType.PointerExit, delegate { HiddenPanelItemInformation(obj); });

                inventory.GetSlots[i].SlotDisplay = obj;
                SlotsOnInterface.Add(obj, inventory.GetSlots[i]);
            }
        }

        protected override Vector3 OffsetPositionPanelInfoItem()
        {
            return new Vector3(48f, 0f);
        }

        private static Vector3 GetPosition(int i)
        {   
            return new Vector3((X_START + X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMNS)),
                (Y_START - Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMNS)), (0f));
        }
    }
}