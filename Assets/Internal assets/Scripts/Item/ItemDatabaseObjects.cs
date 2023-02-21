using UnityEngine;
namespace Item
{
    [CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObjects : ScriptableObject
    {
        public ItemObject[] ItemObjects;

        public void OnValidate()
        {
            for (int i = 0; i < ItemObjects.Length; i++)
            {
                ItemObjects[i].data.Id = i;
            }
        }
    }
}