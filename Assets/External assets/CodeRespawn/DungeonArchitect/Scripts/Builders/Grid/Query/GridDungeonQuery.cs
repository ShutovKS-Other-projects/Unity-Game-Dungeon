//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DungeonArchitect.Builders.Grid
{
    public class GridDungeonQuery : DungeonEventListener
    {
        [HideInInspector]
        public Dictionary<int, GameObject[]> DoorObjectsByCellId = new Dictionary<int, GameObject[]>();

        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            GenerateQuery();
        }

        public override void OnPreDungeonDestroy(Dungeon dungeon)
        {
            DoorObjectsByCellId.Clear();
        }

        public void GenerateQuery()
        {
            DoorObjectsByCellId.Clear();

            var dungeon = GetComponent<Dungeon>();
            if (dungeon == null)
            {
                return;
            }
            var doorsByCells = new Dictionary<int, HashSet<GameObject>>();
            var doorMetaArray = GameObject.FindObjectsOfType<GridItemDoorMetadata>();
            foreach (var doorMeta in doorMetaArray)
            {
                // Make sure this belongs to the same dungeon
                var itemData = doorMeta.gameObject.GetComponent<DungeonSceneProviderData>();
                if (itemData && itemData.dungeon == dungeon)
                {
                    if (!doorsByCells.ContainsKey(doorMeta.cellA))
                    {
                        doorsByCells.Add(doorMeta.cellA, new HashSet<GameObject>());
                    }
                    if (!doorsByCells.ContainsKey(doorMeta.cellB))
                    {
                        doorsByCells.Add(doorMeta.cellB, new HashSet<GameObject>());
                    }

                    doorsByCells[doorMeta.cellA].Add(doorMeta.gameObject);
                    doorsByCells[doorMeta.cellB].Add(doorMeta.gameObject);
                }
            }

            foreach (var entry in doorsByCells)
            {
                DoorObjectsByCellId.Add(entry.Key, entry.Value.ToArray());
            }
        }

        public void GetDoorsForCell(int cellId, out GameObject[] doorGameObjects)
        {
            if (DoorObjectsByCellId.ContainsKey(cellId))
            {
                doorGameObjects = DoorObjectsByCellId[cellId];
            }
            else
            {
                doorGameObjects = new GameObject[0];
            }
        }

        public bool GetCellAtPosition(Vector3 position, out Cell outCell)
        {
            var config = GetComponent<GridDungeonConfig>();
            var model = GetComponent<GridDungeonModel>();
            if (config != null && model != null)
            {
                var bounds = new Bounds();
                foreach (var cell in model.Cells)
                {
                    bounds = cell.GetWorldBounds(config.GridCellSize);
                    if (bounds.Contains(position))
                    {
                        outCell = cell;
                        return true;
                    }
                }
            }
            outCell = null;
            return false;
        }

        public Bounds GetCellBounds(Cell cell)
        {
            var bounds = new Bounds();
            var config = GetComponent<GridDungeonConfig>();
            if (config != null)
            {
                bounds = cell.GetWorldBounds(config.GridCellSize);
            }
            return bounds;
        }
        
        public Cell GetRandomCell()
        {
            var model = GetComponent<GridDungeonModel>();
            if (model == null || model.Cells.Count == 0)
            {
                return null;
            }

            return model.Cells[Random.Range(0, model.Cells.Count)];
        }

        public Cell[] FindFurthestRooms()
        {
            var model = GetComponent<GridDungeonModel>();
            return GridDungeonModelUtils.FindFurthestRooms(model);
        }
    }
}
