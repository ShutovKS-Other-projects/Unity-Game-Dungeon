using System;
using UnityEngine;
using Inventory;
using Item;
using Other;
namespace Player
{
    public class PlayerEquipment : MonoBehaviour
    {
        private InventoryObject _equipment;

        [Header("Equip Transforms")]
        [SerializeField] private Transform offhandWristTransform;
        [SerializeField] private Transform offhandHandTransform;
        [SerializeField] private Transform weaponTransform;

        private BoneCombiner _boneCombiner;
        private Transform _pants;
        private Transform _gloves;
        private Transform _boots;
        private Transform _chest;
        private Transform _helmet;
        private Transform _sword;


        void Start()
        {
            _equipment = GetComponent<PlayerInventory>().equipment;

            //_boneCombiner = new BoneCombiner(gameObject);

            foreach (var inventorySlot in _equipment.GetSlots)
            {
                inventorySlot.OnBeforeUpdated += OnRemoveItem;
                inventorySlot.OnAfterUpdated += OnEquipItem;
            }
        }

        private void OnEquipItem(InventorySlot slot)
        {
            var itemObject = slot.GetItemObject();
            if (itemObject == null)
                return;
            switch (slot.Parent.inventory.type)
            {
                case InterfaceType.Equipment:
                    if (itemObject.characterDisplay != null)
                    {
                        switch (slot.allowedItems[0])
                        {
                            case ItemType.Gloves:
                                // _gloves = _boneCombiner.AddLimb(itemObject.characterDisplay, itemObject.boneNames);
                                break;

                            case ItemType.Boots:
                                // _boots = _boneCombiner.AddLimb(itemObject.characterDisplay, itemObject.boneNames);
                                break;

                            case ItemType.Chest:
                                // _chest = _boneCombiner.AddLimb(itemObject.characterDisplay, itemObject.boneNames);
                                break;

                            case ItemType.Pants:
                                // _pants = _boneCombiner.AddLimb(itemObject.characterDisplay, itemObject.boneNames);
                                break;

                            case ItemType.Helmet:
                                // _helmet = _boneCombiner.AddLimb(itemObject.characterDisplay, itemObject.boneNames);
                                break;

                            case ItemType.Weapon:
                                GetComponent<Animator>().SetLayerWeight(1, 1);
                                _sword = Instantiate(itemObject.characterDisplay, weaponTransform).transform;
                                break;
                            default:
                                Debug.LogWarning("Item Equipment non search type");
                                break;
                        }
                    }
                    break;
                case InterfaceType.Inventory:
                    break;
                case InterfaceType.Chest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnRemoveItem(InventorySlot slot)
        {
            if (slot.GetItemObject() == null)
                return;
            switch (slot.Parent.inventory.type)
            {
                case InterfaceType.Equipment:
                    if (slot.GetItemObject().characterDisplay != null)
                    {
                        switch (slot.allowedItems[0])
                        {
                            case ItemType.Gloves:
                                //Destroy(_gloves.gameObject);
                                break;

                            case ItemType.Boots:
                                // Destroy(_boots.gameObject);
                                break;

                            case ItemType.Chest:
                                // Destroy(_chest.gameObject);
                                break;

                            case ItemType.Pants:
                                // Destroy(_pants.gameObject);
                                break;

                            case ItemType.Helmet:
                                // Destroy(_helmet.gameObject);
                                break;

                            case ItemType.Weapon:
                                GetComponent<Animator>().SetLayerWeight(1, 0);
                                Destroy(_sword.gameObject);
                                break;
                            default:
                                Debug.LogWarning("Item Equipment non search type");
                                break;
                        }
                    }
                    break;
                case InterfaceType.Inventory:
                    break;
                case InterfaceType.Chest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
