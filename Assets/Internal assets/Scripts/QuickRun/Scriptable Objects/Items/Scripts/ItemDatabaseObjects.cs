using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObjects : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] Items;

    [ContextMenu("Update ID's")]
    public void UpdateID()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].data.Id = i;
        }
    }

    public void OnAfterDeserialize()
    {
        UpdateID();
    }
    public void OnBeforeSerialize() { }
}