using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObjects : ScriptableObject
    {
        public ItemObject[] itemObjects;

        public void OnValidate()
        {
            for (var i = 0; i < itemObjects.Length; i++)
            {
                itemObjects[i].data.id = i;
            }
        }
    }
}