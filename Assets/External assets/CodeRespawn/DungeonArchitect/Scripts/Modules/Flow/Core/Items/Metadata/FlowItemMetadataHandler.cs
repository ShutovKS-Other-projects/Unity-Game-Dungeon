//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Items
{
    [SerializeField]
    public class FlowItemMetadata
    {
        public FlowGraphItemType itemType;
        public DungeonUID itemId = DungeonUID.Empty;
        public DungeonUID[] referencedItems = new DungeonUID[0];
        public Transform parentTransform = null;
    }

    public class FlowItemMetadataHandler : DungeonItemSpawnListener
    {
        T FindOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null)
            {
                return null;
            }
            
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        public override void SetMetadata(GameObject dungeonItem, DungeonNodeSpawnData spawnData)
        {
            if (dungeonItem != null)
            {
                var marker = spawnData.socket;
                if (marker.metadata is FlowItemMetadata)
                {
                    var itemData = marker.metadata as FlowItemMetadata;
                    if (itemData != null)
                    {
                        var component = FindOrAddComponent<FlowItemMetadataComponent>(dungeonItem);
                        if (component != null)
                        {
                            component.itemType = itemData.itemType;
                            component.itemId = itemData.itemId.ToString();

                            var referencedIds = new List<string>();
                            foreach (var referencedGuidId in itemData.referencedItems)
                            {
                                referencedIds.Add(referencedGuidId.ToString());
                            }

                            component.referencedItemIds = referencedIds.ToArray();
                        }
                    }

                    if (itemData.parentTransform != null)
                    {
                        dungeonItem.transform.SetParent(itemData.parentTransform, true); 
                    }
                }
            }
        }
    }
}
