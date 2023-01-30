using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;
    
    public Attribute[] attributes;

    public Transform helmet;
    private Transform chest;
    private Transform offhand;
    private Transform boots;
    private Transform weapon;
    private Transform sword;

    public Transform weaponTransform;
    public Transform offhandWristTransform;
    public Transform offhandHandTransform;

    private void Start()
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
        }
    }

    public void OnRemoveItem(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print($"Removed {_slot.ItemObject} on {_slot.parent.inventory.type}, Allowed Items: {string.Join(", ", _slot.AllowedItems)}");

                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute.type)
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                    }
                }
                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    private void OnAddItem(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
        {
            return;
        }
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute.type)
                        {
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);
                        }
                    }
                }

                if(_slot.ItemObject.characterDisplay != null)
                {
                    switch (_slot.AllowedItems[0])
                    {
                        case ItemType.Helmet:
                        case ItemType.Chest:
                        case ItemType.Pants:
                        case ItemType.Boots:
                        case ItemType.Sword:
                        default:
                            break;
                    }
                }

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
            equipment.Save();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.Load();
            equipment.Load();
        }
    }

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was modified to ", attribute.value.ModifiedValue));
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
        equipment.Container.Clear();
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public Player parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(Player _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }
    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}