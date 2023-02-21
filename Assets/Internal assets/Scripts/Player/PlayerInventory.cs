using Inventory;
using Item;
using Other;
using UnityEngine;
namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        public InventoryObject inventory;
        public InventoryObject equipment;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                inventory.Save();
                equipment.Save();
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                inventory.Load();
                equipment.Load();
            }
        }

        private void OnApplicationQuit()
        {
            inventory.Clear();
            equipment.Clear();
        }
    }
}
