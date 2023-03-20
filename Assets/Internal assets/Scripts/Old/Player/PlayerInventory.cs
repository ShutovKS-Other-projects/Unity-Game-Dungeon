using Old.Inventory;
using UnityEngine;

namespace Old.Player
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
