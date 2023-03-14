//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.LevelStreaming
{
    public class DungeonItemGroup : DungeonEventListener
    {
        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            var dungeonObjects = DungeonUtils.GetDungeonObjects(dungeon);

            // Group the dungeon items by cell ids
            Dictionary<int, List<GameObject>> gameObjectsByCellId = new Dictionary<int, List<GameObject>>();
            foreach (var dungeonObject in dungeonObjects)
            {
                var data = dungeonObject.GetComponent<DungeonSceneProviderData>();
                var cellId = data.userData;
                if (cellId == -1) continue;

                if (!gameObjectsByCellId.ContainsKey(cellId))
                {
                    gameObjectsByCellId.Add(cellId, new List<GameObject>());
                }

                gameObjectsByCellId[cellId].Add(dungeonObject);
            }

            // Create new prefabs and group them under it
            foreach (var cellId in gameObjectsByCellId.Keys)
            {
                var cellItems = gameObjectsByCellId[cellId];
                var groupName = "Group_Cell_" + cellId;
                GroupItems(cellItems.ToArray(), groupName, dungeon, cellId);
            }

            // Destroy the old group objects
            DestroyOldGroupObjects(dungeon);

            // Subclasses will override this and perform builder specific grouping
        }

        /// <param name="model">The dungeon model</param>
        public override void OnDungeonDestroyed(Dungeon dungeon)
        {
            DestroyOldGroupObjects(dungeon);
        }

        protected DungeonItemGroupInfo GroupItems(GameObject[] items, string groupName, Dungeon dungeon, int groupId)
        {
            if (items.Length == 0) return null;
            var position = items[0].transform.position;
            for (int i = 1; i < items.Length; i++)
            {
                position += items[i].transform.position;
            }

            position /= items.Length;

            var groupObject = new GameObject(groupName);
            groupObject.transform.position = position;

            // Re-parent all the cell items to this group object
            foreach (var cellItem in items)
            {
                cellItem.transform.SetParent(groupObject.transform, true);
            }

            var groupInfo = groupObject.AddComponent<DungeonItemGroupInfo>();
            groupInfo.dungeon = dungeon;
            groupInfo.groupId = groupId;

            GameObject dungeonItemParent = null;
            var sceneProvider = dungeon.GetComponent<DungeonSceneProvider>();
            if (sceneProvider != null)
            {
                dungeonItemParent = sceneProvider.itemParent;
            }

            groupInfo.transform.SetParent(dungeonItemParent.transform, true);

            return groupInfo;
        }

        void DestroyOldGroupObjects(Dungeon dungeon)
        {
            var groupInfoArray = GameObject.FindObjectsOfType<DungeonItemGroupInfo>();

            foreach (var groupInfo in groupInfoArray)
            {
                if (groupInfo.dungeon == dungeon)
                {
                    var go = groupInfo.gameObject;
                    if (go.transform.childCount == 0)
                    {
                        EditorDestroyObject(go);
                    }
                }
            }
        }

        protected void EditorDestroyObject(Object obj)
        {
            if (Application.isPlaying)
            {
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj);
            }
        }

    }

}
