namespace Inventory
{
    [System.Serializable]
    public class Inventory
    {
        public InventorySlot[] Slots = new InventorySlot[48];
        public void Clear()
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i].RemoveItem();
            }
        }
    }
}