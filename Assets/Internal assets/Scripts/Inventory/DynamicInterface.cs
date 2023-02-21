using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
namespace Inventory
{
    public class DynamicInterface : UserInterface
    {
        [SerializeField] private GameObject panelInventory;
        private const int XStart = -105;
        private const int YStart = 75;
        private const int XSpaceBetweenItem = 30;
        private const int YSpaceBetweenItem = 30;
        private const int NumberOfColumns = 8;

        protected override void CreateSlots()
        {
            SlotsOnInterface = new Dictionary<GameObject, InventorySlot>();
            for (int i = 0; i < inventory.GetSlots.Length; i++)
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, panelInventory.transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
                AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
                AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
                inventory.GetSlots[i].SlotDisplay = obj;
                SlotsOnInterface.Add(obj, inventory.GetSlots[i]);
            }
        }
        
        public void AllSlotsInInventoryUpdate() => AllSlotsUpdate();

        protected override void AllSlotsUpdate()
        {
            foreach (var inventorySlot in inventory.GetSlots)
            {
                inventorySlot.UpdateSlot(inventorySlot.item, inventorySlot.amount);
            }
        }
        
        private static Vector3 GetPosition(int i)
        {
            return new Vector3((XStart + XSpaceBetweenItem * (i % NumberOfColumns)), (YStart - YSpaceBetweenItem * (i / NumberOfColumns)), (0f));
        }
    }
}
