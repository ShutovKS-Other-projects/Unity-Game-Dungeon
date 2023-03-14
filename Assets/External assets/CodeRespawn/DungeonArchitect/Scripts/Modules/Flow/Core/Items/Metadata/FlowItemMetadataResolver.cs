//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Flow.Items
{
    public class FlowItemMetadataResolver : DungeonEventListener
    {
        T[] GetDungeonOwnedComponents<T>(Dungeon dungeon) where T : Component
        {
            var result = new List<T>();
            var allItems = GameObject.FindObjectsOfType<T>();
            foreach (var item in allItems)
            {
                if (item == null) continue;
                var dungeonData = item.gameObject.GetComponent<DungeonSceneProviderData>();
                if (dungeonData == null) continue;
                if (dungeonData.dungeon == dungeon)
                {
                    result.Add(item);
                }
            }

            return result.ToArray();
        }

        T FindOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            var items = GetDungeonOwnedComponents<FlowItemMetadataComponent>(dungeon);
            var itemMap = new Dictionary<string, FlowItemMetadataComponent>();
            foreach (var item in items)
            {
                if (item.itemId == null || item.itemId.Length == 0) continue;

                if (itemMap.ContainsKey(item.itemId))
                {
                    //Debug.LogError("Duplicate id: " + item.itemId);
                }

                itemMap[item.itemId] = item;
            }

            // clear out the key locks
            {
                var oldKeys = GetDungeonOwnedComponents<FlowDoorKeyComponent>(dungeon);
                foreach (var key in oldKeys)
                {
                    key.lockRefs = new FlowDoorLockComponent[0];
                }

                var oldLocks = GetDungeonOwnedComponents<FlowDoorLockComponent>(dungeon);
                foreach (var lockComponent in oldLocks)
                {
                    lockComponent.validKeyRefs = new FlowDoorKeyComponent[0];
                    lockComponent.validKeyIds = new string[0];
                }
            }

            foreach (var item in items)
            {
                if (item.itemType == FlowGraphItemType.Key)
                {
                    var keyComponent = FindOrAddComponent<FlowDoorKeyComponent>(item.gameObject);
                    keyComponent.keyId = item.itemId;

                    var lockComponents = new List<FlowDoorLockComponent>();
                    for (int i = 0; i < item.referencedItemIds.Length; i++)
                    {
                        var refItemId = item.referencedItemIds[i];
                        if (!itemMap.ContainsKey(refItemId)) continue;

                        var refItem = itemMap[refItemId];
                        if (refItem.itemType == FlowGraphItemType.Lock)
                        {
                            var lockComponent = FindOrAddComponent<FlowDoorLockComponent>(refItem.gameObject);
                            lockComponents.Add(lockComponent);

                            var keyRefs = new List<FlowDoorKeyComponent>(lockComponent.validKeyRefs);
                            keyRefs.Add(keyComponent);
                            lockComponent.validKeyRefs = keyRefs.ToArray();

                            var keyIds = new List<string>(lockComponent.validKeyIds);
                            keyIds.Add(keyComponent.keyId);
                            lockComponent.validKeyIds = keyIds.ToArray();
                        }
                    }

                    keyComponent.lockRefs = lockComponents.ToArray();
                    keyComponent.keyId = item.itemId;
                    keyComponent.validLockIds = item.referencedItemIds;
                }
                else if (item.itemType == FlowGraphItemType.Lock)
                {
                    var lockComponent = FindOrAddComponent<FlowDoorLockComponent>(item.gameObject);
                    lockComponent.lockId = item.itemId;
                }
            }
        }
    }
}
