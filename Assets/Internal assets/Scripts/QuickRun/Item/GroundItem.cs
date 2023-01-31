using UnityEngine;
using UnityEditor;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public ItemObject item;

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (item != null && item.characterDisplay == null)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = item.uiDisplay;
            EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
        }
#endif
    }
}
