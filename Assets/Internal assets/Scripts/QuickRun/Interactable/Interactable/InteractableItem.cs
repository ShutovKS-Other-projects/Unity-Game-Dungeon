using UnityEngine;

public class InteractableItem : InteractableBase
{
    [SerializeField] private InventoryObject inventory;

    public override void OnInteract()
    {
        var groundItem = gameObject.GetComponent<GroundItem>();

        if (groundItem)
        {
            Item _item = new Item(groundItem.item);
            if (inventory.AddItem(_item, 1))
            {
                Destroy(gameObject);
            }
        }
    }
}
