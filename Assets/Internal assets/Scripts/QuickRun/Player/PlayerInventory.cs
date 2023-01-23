using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventoryObject inventory;

    //public void OnTriggerEnter(Collider other)
    //{
    //    var item = other.GetComponent<GroundItem>();
    //    if (item)
    //    {
    //        Item _item = new Item(item.item);
    //        inventory.AddItem(_item, 1);
    //        Destroy(other.gameObject);
    //    }
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.Load();
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Items = new InventorySlot[48];
    }
}
