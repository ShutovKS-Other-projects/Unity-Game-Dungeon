using System;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using Item;

namespace Player
{
    public class PlayerAttribute : MonoBehaviour
    {
        [SerializeField] private readonly PlayerData _playerData;
        private InventoryObject _equipment;

        public Attribute[] attributes;

        #region Attributes
        public float Armor => attributes[0].Value ?? 0f;
        public float Health => attributes[1].Value ?? 0f;
        public float HealthRecoverySpeed => attributes[2].Value ?? 0f;
        public float Stamina => attributes[3].Value ?? 0f;
        public float StaminaRecoverySpeed => attributes[4].Value ?? 0f;
        public float Strength => attributes[5].Value ?? 0f;
        public float AttackSpeed => attributes[6].Value ?? 0f;
        public float MoveSpeed => attributes[7].Value ?? 0f;
        public float Agility => attributes[8].Value ?? 0f;
        public float Manna => attributes[9].Value ?? 0f;
        public float MannaRecoverySpeed => attributes[10].Value ?? 0f;
        public float CriticalDamage => attributes[11].Value ?? 0f;
        public float CriticalDamageChance  => attributes[12].Value ?? 0f;
        public float Fortune => attributes[13].Value ?? 0f;
        #endregion

        #region Unity Callbacks Functions

        private void Start()
        {
            _equipment = GetComponent<PlayerInventory>().equipment;

            foreach (var attribute in attributes)
            {
                attribute.SetParent(this);
            }

            foreach (var inventorySlot in _equipment.GetSlots)
            {
                inventorySlot.onBeforeUpdated += OnRemoveItem;
                inventorySlot.onAfterUpdated += OnEquipItem;
            }
        }

        #endregion

        #region Functions

        public static void AttributeModified(Attribute attribute)
        {
            Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.modifiableFloat.ModifiedValue));
        }

        private void OnRemoveItem(InventorySlot slot)
        {
            if (slot.GetItemObject() == null)
                return;
            switch (slot.parent.inventory.type)
            {
                case InterfaceType.Inventory:
                    print("Removed " + slot.GetItemObject() + " on: " + slot.parent.inventory.type + ", Allowed items: " +
                        string.Join(", ", slot.AllowedItems));
                    break;

                case InterfaceType.Equipment:
                    //    print("Removed " + slot.GetItemObject() + " on: " + slot.parent.inventory.type + ", Allowed items: " +
                    //          string.Join(", ", slot.AllowedItems));
                    foreach (var itemBuff in slot.item.buffs)
                    {
                        foreach (var attribute in attributes)
                        {
                            if (attribute.type == itemBuff.stat)
                                attribute.modifiableFloat.RemoveModifier(itemBuff);
                        }
                    }
                    break;

                case InterfaceType.Chest:
                    print("Removed " + slot.GetItemObject() + " on: " + slot.parent.inventory.type + ", Allowed items: " +
                        string.Join(", ", slot.AllowedItems));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnEquipItem(InventorySlot slot)
        {
            if (slot.GetItemObject() == null)
                return;
            switch (slot.parent.inventory.type)
            {
                case InterfaceType.Inventory:
                    print("Placed " + slot.GetItemObject() + " on: " + slot.parent.inventory.type + ", Allowed items: " +
                        string.Join(", ", slot.AllowedItems));
                    break;

                case InterfaceType.Equipment:
                    // print("Placed " + _slot.GetItemObject() + " on: " + _slot.parent.inventory.type + ", Allowed items: " +
                    //      string.Join(", ", _slot.AllowedItems));
                    foreach (var itemBuff in slot.item.buffs)
                    {
                        foreach (var attribute in attributes)
                        {
                            if (attribute.type == itemBuff.stat)
                                attribute.modifiableFloat.AddModifier(itemBuff);
                        }
                    }
                    break;

                case InterfaceType.Chest:
                    print("Placed " + slot.GetItemObject() + " on: " + slot.parent.inventory.type + ", Allowed items: " +
                        string.Join(", ", slot.AllowedItems));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
