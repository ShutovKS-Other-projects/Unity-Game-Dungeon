using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Item
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
    public class ItemObject : ScriptableObject
    {
        /// <summary>
        /// Тип предмета
        /// </summary>
        public ItemType type;

        /// <summary>
        /// UI отображение предмета
        /// </summary>
        public Sprite uiDisplay;

        /// <summary>
        /// 3D Модель предмета
        /// </summary>
        public GameObject swordModel;

        /// <summary>
        /// Можно ли стакать предметы
        /// </summary>
        public bool stackable;

        /// <summary>
        /// Имя предмета
        /// </summary>
        public string itemName;

        /// <summary>
        /// Описание предмета
        /// </summary>
        [TextArea(15, 20)] public string description;

        /// <summary>
        /// Данные предмета
        /// </summary>
        public Item data = new();

        /// <summary>
        /// Список имен костей
        /// </summary>
        public List<string> boneNames = new();

        private void OnValidate()
        {
            boneNames.Clear();

            if (swordModel == null || swordModel.GetComponent<SkinnedMeshRenderer>() == null)
                return;

            var renderer = swordModel.GetComponent<SkinnedMeshRenderer>();
            var bones = renderer.bones;

            foreach (var transform in bones)
            {
                boneNames.Add(transform.name);
            }
        }
    }
}