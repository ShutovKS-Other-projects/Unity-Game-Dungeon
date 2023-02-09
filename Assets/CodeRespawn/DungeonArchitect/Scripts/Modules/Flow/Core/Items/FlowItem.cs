//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Items
{
    [System.Serializable]
    public enum FlowGraphItemType
    {
        Key = 0,
        Lock = 1,
        Enemy = 2,
        Bonus = 3,
        Entrance = 4,
        Exit = 5,
        Custom = 6,
        Teleporter = 7,
    }

    [System.Serializable]
    public class FlowItem
    {
        public DungeonUID itemId;

        /// <summary>
        /// The item type
        /// </summary>
        public FlowGraphItemType type;

        public string markerName = "";

        /// <summary>
        /// Reference to other items (e.g. key locks from other nodes)
        /// </summary>
        public List<DungeonUID> referencedItemIds = new List<DungeonUID>();

        public bool editorSelected = false;

        public FlowGraphItemCustomInfo customInfo = FlowGraphItemCustomInfo.Default;

        [HideInInspector]
        public FlowDomainDataRegistry domainData = new FlowDomainDataRegistry();

        public FlowItem()
        {
            itemId = DungeonUID.NewUID();
        }
        
        public FlowItem Clone()
        {
            var newItem = new FlowItem();
            newItem.itemId = itemId;
            newItem.type = type;
            newItem.markerName = markerName;
            newItem.referencedItemIds = new List<DungeonUID>(referencedItemIds);
            newItem.customInfo = customInfo;
            newItem.domainData = domainData.Clone();
            return newItem;
        }

        public T GetDomainData<T>() where T : IFlowDomainData, new()
        {
            if (domainData == null)
            {
                return default;
            }

            return domainData.Get<T>();
        }
        
        public void SetDomainData<T>(T data) where T : IFlowDomainData, new()
        {
            if (domainData != null)
            {
                domainData.Set(data);
            }
        }
    }

    [System.Serializable]
    public struct FlowGraphItemCustomInfo
    {
        public string itemType;
        public string text;
        public Color textColor;
        public Color backgroundColor;

        public static readonly FlowGraphItemCustomInfo Default = new FlowGraphItemCustomInfo("custom", "", Color.white, Color.black);

        public FlowGraphItemCustomInfo(string itemType, string text, Color textColor, Color backgroundColor)
        {
            this.itemType = itemType;
            this.text = text;
            this.textColor = textColor;
            this.backgroundColor = backgroundColor;
        }
    }
}
