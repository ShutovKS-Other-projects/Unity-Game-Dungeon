//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.LevelStreaming;

namespace DungeonArchitect.Builders.Grid
{
    public class GridDungeonItemGroup : DungeonItemGroup
    {
        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            base.OnPostDungeonBuild(dungeon,  model);
            
            GridDungeonModel gridModel = dungeon.ActiveModel as GridDungeonModel;
            
        var _groupInfoArray = GameObject.FindObjectsOfType<DungeonItemGroupInfo>();

        Dictionary<int, DungeonItemGroupInfo> groupObjectByCellId = new Dictionary<int, DungeonItemGroupInfo>();

        foreach (var groupInfo in _groupInfoArray)
        {
            if (groupInfo.dungeon == dungeon)
            {
                var cellId = groupInfo.groupId;
                var cell = gridModel.GetCell(cellId);
                if (cell == null || cell.CellType == CellType.Unknown)
                {
                    continue;
                }


                string objectNamePrefix = "";
                if (cell.CellType == CellType.Room)
                {
                    objectNamePrefix = "Room_";
                }
                else
                {
                    groupObjectByCellId[cell.Id] = groupInfo;

                    objectNamePrefix = (cell.CellType == CellType.Corridor) ? "CorridorBlock_" : "CorridorPad_";
                }

                if (objectNamePrefix.Length == 0)
                {
                    objectNamePrefix = "Cell_";
                }

                string groupName = objectNamePrefix + cell.Id;
                groupInfo.gameObject.name = groupName;
            }
        }


        var visited = new HashSet<int>();
        int clusterCounter = 1;
        var oldGroupsToDelete = new List<GameObject>();

        foreach (var groupInfo in groupObjectByCellId.Values)
        {
            var cellId = groupInfo.groupId;
            if (visited.Contains(cellId))
            {
                continue;
            }

            var clusters = GridBuilderUtils.GetCellCluster(gridModel, cellId);
            var itemsToGroup = new List<GameObject>();

            // Mark all cluster cells as visited
            foreach (var clusterItemId in clusters)
            {
                visited.Add(clusterItemId);
                if (groupObjectByCellId.ContainsKey(clusterItemId))
                {
                    var clusterItemGroupInfo = groupObjectByCellId[clusterItemId];
                    for (int i = 0; i < clusterItemGroupInfo.transform.childCount; i++)
                    {
                        var childObject = clusterItemGroupInfo.transform.GetChild(i);
                        itemsToGroup.Add(childObject.gameObject);
                    }
                    oldGroupsToDelete.Add(clusterItemGroupInfo.gameObject);
                }
            }

            int clusterId = clusterCounter++;
            GroupItems(itemsToGroup.ToArray(), "Corridor_" + clusterId, dungeon, clusterId);
        }

        groupObjectByCellId.Clear();

        // Destroy the inner group info objects
        foreach (var itemToDestory in oldGroupsToDelete)
        {
            EditorDestroyObject(itemToDestory);
        }
        }
    }
}
