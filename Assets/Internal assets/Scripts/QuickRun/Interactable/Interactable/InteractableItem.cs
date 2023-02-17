using Internal_assets.Scripts.QuickRun.Inventory;
using Internal_assets.Scripts.QuickRun.Item;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Interactable.Interactable
{
    public class InteractableItem : InteractableBase
    {
        [SerializeField] private InventoryObject inventory;

        public override void OnInteract()
        {
            var groundItem = gameObject.GetComponent<GroundItem>();

            if (groundItem)
            {
                Item.Item _item = new Item.Item(groundItem.item);
                if (inventory.AddItem(_item, 1))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
