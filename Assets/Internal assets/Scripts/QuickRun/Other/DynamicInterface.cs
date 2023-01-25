using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;


public class DynamicInterface : UserInterface
{
    [SerializeField] private int X_START = -105;
    [SerializeField] private int Y_START = 75;
    [SerializeField] private int X_SPACE_BETWEEN_ITEM = 30;
    [SerializeField] private int Y_SPACE_BETWEEN_ITEM = 30;
    [SerializeField] private int NUMBER_OF_COLUMNS = 8;


    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            slotsOnInterface.Add(obj, inventory.Container.Items[i]);
        }
    }

    private Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMNS)), Y_START + (-Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMNS)), 0f);
    }
}
