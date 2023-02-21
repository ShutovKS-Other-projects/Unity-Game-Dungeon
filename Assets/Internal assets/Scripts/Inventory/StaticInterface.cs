using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Inventory
{
    public class StaticInterface : UserInterface
    {
        public GameObject[] slots;

        public override void CreateSlots()
        {
            slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
            for (int i = 0; i < slots.Length; i++)
            {
                var obj = slots[i];
                
                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
                AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
                AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
                inventory.GetSlots[i].slotDisplay = obj;
                slotsOnInterface.Add(obj, inventory.GetSlots[i]);
            }
        }

        public override void AllSlotsUpdate()
        {
            for (int index = 0; index < slots.Length; index++)
            {
                var inventorySlot = inventory.GetSlots[index];
                inventorySlot.UpdateSlot(inventorySlot.item, inventorySlot.amount);
            }
        }

    }
}
