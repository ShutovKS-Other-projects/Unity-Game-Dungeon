﻿using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;

    public Attribute[] attributes;

    private Transform helmet;
    private Transform chest;
    private Transform pants;
    private Transform boots;
    private Transform sword;

    public Transform weaponTransform;

    private BoneCombiner boneCombiner;

    private void Start()
    {
        boneCombiner = new BoneCombiner(gameObject);

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
                print(string.Concat("Removed ", _slot.ItemObject, " on ", _slot.parent.inventory.type, ", Allowed Items: ", string.Join(", ", _slot.AllowedItems)));

                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                    }
                }

                if (_slot.ItemObject.characterDisplay != null)
                {
                    switch (_slot.AllowedItems[0])
                    {
                        //case ItemType.Helmet:
                        //    Destroy(helmet.gameObject);
                        //    break;
                        //case ItemType.Chest:
                        //    Destroy(chest.gameObject);
                        //    break;
                        //case ItemType.Pants:
                        //    Destroy(pants.gameObject);
                        //    break;
                        //case ItemType.Boots:
                        //    Destroy(boots.gameObject);
                        //    break;
                        case ItemType.Weapon:
                            Destroy(sword.gameObject);
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

    public void OnAddItem(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print($"Placed {_slot.ItemObject}  on {_slot.parent.inventory.type}, Allowed Items: {string.Join(", ", _slot.AllowedItems)}");

                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);
                    }
                }

                if (_slot.ItemObject.characterDisplay != null)
                {
                    switch (_slot.AllowedItems[0])
                    {
                        //case ItemType.Helmet:
                        //    helmet = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames);
                        //    break;
                        //case ItemType.Chest:
                        //    chest = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames);
                        //    break;
                        //case ItemType.Pants:
                        //    pants = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames);
                        //    break;
                        //case ItemType.Boots:
                        //    boots = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames);
                        //    break;
                        case ItemType.Weapon:
                            sword = Instantiate(_slot.ItemObject.characterDisplay, weaponTransform).transform;
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
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
    }


    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized] public Player parent;
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