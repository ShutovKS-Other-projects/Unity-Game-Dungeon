using System.Collections.Generic;
using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Inventory
{
    public class StaticInterface : UserInterface
    {
        public GameObject[] slots;

        protected override void CreateSlots()
        {
            SlotsOnInterface = new Dictionary<GameObject, InventorySlot>();
            for (var i = 0; i < slots.Length; i++)
            {
                var obj = slots[i];

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
            return new Vector3(-48f, 0f);
        }
    }
}