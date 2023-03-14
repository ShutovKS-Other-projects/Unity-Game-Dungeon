//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect.RoomDesigner
{
    [System.Serializable]
    public struct DungeonRoomDoorDesigner
    {
        [SerializeField]
        public Vector3 logicalCursorPosition;

        [SerializeField]
        public Vector3 logicalPosition;

        [SerializeField]
        public Quaternion rotation;

        [SerializeField]
        public IntVector logicalSize;

        [SerializeField]
        public string markerName;
    }

    public class DungeonRoomDesigner : MonoBehaviour
    {
        public Vector3 gridSize = new Vector3(4, 2, 4);
        public IntVector roomPosition = IntVector.Zero;
        public IntVector roomSize = new IntVector(2, 1, 2);
        public Dungeon dungeon;
        public bool realtimeUpdate = true;
        public bool generateBoundaryMarkers = true;

        public DungeonRoomDoorDesigner[] doors;
        DungeonRoomVoxel voxelWorld = null;

        List<FloorIsland> islands = new List<FloorIsland>();

        public void GenerateLayout()
        {
            voxelWorld = new DungeonRoomVoxel(roomSize.x, roomSize.y, roomSize.z);

            CreateDoorPlatforms();
            FillSmallGaps();

            islands = new List<FloorIsland>();
            for (int y = roomSize.y - 1; y >= 0; y--)
            {
                islands.AddRange(FloorIsland.FindIslands(voxelWorld, y));
            }
            
        }

        void OnDrawGizmosSelected()
        {
            DebugDrawGizmos();
        }

        public void CreateDoorPlatforms()
        {
            var doorCarvings = new CarvingCommand[]
            {
                // Make sure the entrance is empty
                new CarvingCommand(new Vector3(0, 0, 0.5f), DungeonRoomVoxelCellType.Empty, true),
                new CarvingCommand(new Vector3(0, 1, 0.5f), DungeonRoomVoxelCellType.Empty, true),

                // Place a platform beneath the door
                new CarvingCommand(new Vector3( 0, -1, 0.5f), DungeonRoomVoxelCellType.Occupied, true), // Make sure we have a platform underneath the door entrance
                new CarvingCommand(new Vector3(-1, -1, 0.5f), DungeonRoomVoxelCellType.Occupied, false),
                new CarvingCommand(new Vector3( 1, -1, 0.5f), DungeonRoomVoxelCellType.Occupied, false),
            };

            // Carve out the door platforms
            foreach (var door in doors)
            {
                foreach (var carvingCommand in doorCarvings)
                {
                    var platformPos = door.logicalPosition + door.rotation * carvingCommand.localPosition;
                    var coord = MathUtils.ToIntVector(platformPos);
                    voxelWorld.SetState(coord, carvingCommand.cellType, carvingCommand.stateLocked);
                    voxelWorld.SetData(coord, DungeonRoomVoxelCellData.Door);
                }
            }
        }

        public void FillSmallGaps()
        {
            voxelWorld.IterateCells((cell, coord) =>
            {
                if (voxelWorld.IsEmpty(coord.x, coord.y, coord.z) &&
                    !voxelWorld.IsEmpty(coord.x, coord.y - 1, coord.z) &&
                    !voxelWorld.IsEmpty(coord.x, coord.y + 1, coord.z))
                {
                    cell.cellType = DungeonRoomVoxelCellType.Occupied;
                }
            });
        }

        public void EmitMarkers(LevelMarkerList markerList)
        {
            if (voxelWorld != null)
            {
                voxelWorld.EmitMarkers(markerList, roomPosition, doors, gridSize);
            }
        }

        void DebugDrawGizmos()
        {
            var debugColors = new Color[]
            {
                Color.red,
                Color.green,
                Color.blue,
                Color.cyan,
                Color.magenta,
                Color.yellow,
            };
            if (islands != null)
            {
                var slabSize = new Vector3(gridSize.x, 0.1f, gridSize.z);
                for (int i = 0; i < islands.Count; i++)
                {
                    var island = islands[i];

                    var drawColor = (i < debugColors.Length) ? debugColors[i] : Random.ColorHSV();
                    drawColor.a = 0.5f;
                    Gizmos.color = drawColor;

                    foreach (var cellPosition in island.IslandCells)
                    {
                        var position = Vector3.Scale((roomPosition + cellPosition).ToVector3(), gridSize);
                        Gizmos.DrawCube(position + slabSize / 2.0f, slabSize);
                    }
                }
            }
        }

    }

    struct CarvingCommand
    {
        public CarvingCommand(Vector3 localPosition, DungeonRoomVoxelCellType cellType, bool stateLocked)
        {
            this.localPosition = localPosition;
            this.cellType = cellType;
            this.stateLocked = stateLocked;
        }

        public Vector3 localPosition;
        public DungeonRoomVoxelCellType cellType;
        public bool stateLocked;
    }

    class FloorIsland
    {
        List<IntVector> islandCells = new List<IntVector>();
        public List<IntVector> IslandCells
        {
            get { return islandCells; }
        }
        
        /// <summary>
        /// Merges the islands in the same level
        /// </summary>
        public static FloorIsland MergeIslands(FloorIsland[] islands)
        {
            return null;
        }
        
        

        public static FloorIsland[] FindIslands(DungeonRoomVoxel voxelWorld, int y)
        {
            int width = voxelWorld.Cells.GetLength(0);
            int height = voxelWorld.Cells.GetLength(2);

            bool[,] visited = new bool[width, height];
            var islands = new List<FloorIsland>();

            var bfsOffset = new IntVector[]
            {
                    new IntVector(-1, 0,  0),
                    new IntVector( 1, 0,  0),
                    new IntVector( 0, 0, -1),
                    new IntVector( 0, 0,  1)
            };

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    if (voxelWorld.IsValidPlatform(new IntVector(x, y, z)) && !visited[x, z])
                    {
                        // Find an island from this occupied cell
                        var island = new FloorIsland();

                        var queue = new Queue<IntVector>();
                        queue.Enqueue(new IntVector(x, y, z));
                        visited[x, z] = true;

                        while (queue.Count > 0)
                        {
                            var position = queue.Dequeue();
                            island.IslandCells.Add(position);

                            for (int io = 0; io < bfsOffset.Length; io++)
                            {
                                var p = position + bfsOffset[io];
                                if (voxelWorld.IsValidPlatform(p) && !visited[p.x, p.z])
                                {
                                    visited[p.x, p.z] = true;
                                    queue.Enqueue(p);
                                }
                            }
                        }
                        if (island.IslandCells.Count > 0)
                        {
                            islands.Add(island);
                        }
                    }
                }
            }

            return islands.ToArray();
        }
    }
}
