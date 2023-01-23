using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIInventory : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();
    public InventoryObject inventory;
    public Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
    [SerializeField] private GameObject SlotPanel;
    [SerializeField] private GameObject inventoryPrefab;

    private int X_START = -105;
    private int Y_START = 75;

    private int X_SPACE_BETWEEN_ITEM = 30;
    private int Y_SPACE_BETWEEN_ITEM = 30;

    private int NUMBER_OF_COLUMNS = 8;
    
    private void Start()
    {
        CreateSlots();
    }

    private void Update()
    {
        UpdateSlots();
    }

    private void CreateSlots()
    {
        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, SlotPanel.transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            itemsDisplayed.Add(obj, inventory.Container.Items[i]);
        }
    }

    private void UpdateSlots()
    {
        foreach(KeyValuePair<GameObject, InventorySlot> _slot in itemsDisplayed)
        {
            if(_slot.Value.ID >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = inventory.database.GetItem[_slot.Value.ID].uiDisplay;
                _slot.Key.transform.GetChild(1).GetComponent<Text>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = inventoryPrefab.GetComponent<Image>().sprite;
                _slot.Key.transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
    private void OnEnter(GameObject obj)
    {
        mouseItem.hoverObj = obj;
        if(itemsDisplayed.ContainsKey(obj))
        {
            mouseItem.hoverItem = itemsDisplayed[obj];
        }
    }
    private void OnExit(GameObject obj)
    {
        mouseItem.hoverObj = null;
        mouseItem.hoverItem = null;
    }
    private void OnDragStart(GameObject obj)
    {
        var mouseOject = new GameObject();
        var rt = mouseOject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(30, 30);
        mouseOject.transform.SetParent(transform.parent);
        if(itemsDisplayed[obj].ID >= 0)
        {
            var img = mouseOject.AddComponent<Image>();
            img.sprite = inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay;
            img.raycastTarget = false;
        }
        mouseItem.obj = mouseOject;
        mouseItem.item = itemsDisplayed[obj];
    }
    private void OnDragEnd(GameObject obj)
    {
        if(mouseItem.hoverObj)
        {
            inventory.MoveItem(itemsDisplayed[obj], itemsDisplayed[mouseItem.hoverObj]);
        }
        else
        {
            inventory.RemoveItem(itemsDisplayed[obj].item);
        }
        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }
    private void OnDrag(GameObject obj)
    {
        if(mouseItem.obj != null)
        {
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    private Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMNS)), Y_START + (-Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMNS)), 0f);
    }
}

public class MouseItem
{
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}