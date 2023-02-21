using UnityEditor;
using UnityEngine;
namespace Item
{
    public class GroundItem : MonoBehaviour
    {
        public ItemObject item;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (item == null || item.characterDisplay != null)
                return;

            GetComponentInChildren<SpriteRenderer>().sprite = item.uiDisplay;
            EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
#endif
        }
    }
}
