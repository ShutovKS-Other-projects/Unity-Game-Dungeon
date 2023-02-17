using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Item
{
    [CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObjects : ScriptableObject, ISerializationCallbackReceiver
    {
        public ItemObject[] ItemObjects;

        [ContextMenu("Update ID's")]
        public void UpdateID()
        {
            for (int i = 0; i < ItemObjects.Length; i++)
            {
                ItemObjects[i].data.Id = i;
            }
        }

        public void OnAfterDeserialize()
        {
            UpdateID();
        }
        public void OnBeforeSerialize() { }
    }
}