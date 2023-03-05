using System.Collections.Generic;
using UnityEngine;
namespace Item
{

    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
    public class ItemObject : ScriptableObject
    {
        public Sprite uiDisplay;
        public GameObject characterDisplay;
        public bool stackable;
        public ItemType type;
        [TextArea(15, 20)] public string description;
        public Item data = new();
        
        public List<string> boneNames = new();

        private void OnValidate()
        {
            boneNames.Clear();
            if (characterDisplay == null)
                return;
            
            if (!characterDisplay.GetComponent<SkinnedMeshRenderer>())
                return;

            var renderer = characterDisplay.GetComponent<SkinnedMeshRenderer>();
            var bones = renderer.bones;

            foreach (var transform in bones)
            {
                boneNames.Add(transform.name);
            }

        }
    }
}