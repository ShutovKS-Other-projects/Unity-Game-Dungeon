using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Inventory
{
    public class DynamicInterface : UserInterface
    {
        [SerializeField] private GameObject panelInvertory;
        [SerializeField] private int X_START = -105;
        [SerializeField] private int Y_START = 75;
        [SerializeField] private int X_SPACE_BETWEEN_ITEM = 30;
        [SerializeField] private int Y_SPACE_BETWEEN_ITEM = 30;
        [SerializeField] private int NUMBER_OF_COLUMNS = 8;


        public override void CreateSlots()
        {
            slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
            for (int i = 0; i < inventory.GetSlots.Length; i++)
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, panelInvertory.transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

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
            for (int i = 0; i < inventory.GetSlots.Length; i++)
            {
                inventory.GetSlots[i].UpdateSlot(inventory.GetSlots[i].item, inventory.GetSlots[i].amount);
            }
        }

        private Vector3 GetPosition(int i)
        {
            return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMNS)), Y_START + (-Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMNS)), 0f);
        }
    }
}
