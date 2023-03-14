//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonArchitect.RoomDesigner
{
    class DungeonRoomDesignerConstants
    {
        public static readonly string Ground    = "Ground";
        public static readonly string Wall      = "Wall";
        public static readonly string WallHalf  = "WallHalf";
        public static readonly string Door      = "Door";
        public static readonly string Ceiling   = "Ceiling";
    }

    public enum DungeonRoomVoxelCellData
    {
        None    = 0,
        Door    = 1 << 0,
        Stair   = 1 << 1,
    }

    public enum DungeonRoomVoxelCellType
    {
        Empty,
        Occupied
    }

    [System.Serializable]
    public class DungeonRoomVoxelCell
    {
        [SerializeField]
        public DungeonRoomVoxelCellType cellType = DungeonRoomVoxelCellType.Empty;

        [SerializeField]
        public bool stateLocked = false;

        [SerializeField]
        public int cellData = (int)DungeonRoomVoxelCellData.None;
    }

    public class DungeonRoomVoxel
    {
        DungeonRoomVoxelCell[,,] cells;
        public DungeonRoomVoxelCell[,,] Cells
        {
            get { return cells; }
        }

        public DungeonRoomVoxel(int sizeX, int sizeY, int sizeZ)
        {
            cells = new DungeonRoomVoxelCell[sizeX, sizeY, sizeZ];
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    for (int z = 0; z < cells.GetLength(2); z++)
                    {
                        var cell = new DungeonRoomVoxelCell();
                        cell.cellType = DungeonRoomVoxelCellType.Empty;
                        cell.cellData = 0;
                        cells[x, y, z] = cell;
                    }
                }
            }
        }
        
        public void TagDoorCells(Vector3 logicalPosition)
        {

        }

        public bool IsValidCoord(IntVector coord)
        {
            return IsValidCoord(coord.x, coord.y, coord.z);
        }

        public bool IsValidCoord(int x, int y, int z)
        {
            return !(x < 0 || y < 0 || z < 0 || x >= cells.GetLength(0) || y >= cells.GetLength(1) || z >= cells.GetLength(2));
        }

        public void SetState(IntVector coord, DungeonRoomVoxelCellType cellType)
        {
            SetState(coord, cellType, false);
        }

        public void SetData(IntVector coord, DungeonRoomVoxelCellData cellData)
        {
            if (IsValidCoord(coord.x, coord.y, coord.z))
            {
                var cell = cells[coord.x, coord.y, coord.z];
                cell.cellData |= (int)cellData;
            }
        }

        public bool IsValidPlatform(IntVector p)
        {
            return IsEmpty(p.x, p.y, p.z)
                && IsEmpty(p.x, p.y + 1, p.z)
                && !IsEmpty(p.x, p.y - 1, p.z);
        }

        public bool ContainsData(IntVector coord, DungeonRoomVoxelCellData cellData)
        {
            if (!IsValidCoord(coord.x, coord.y, coord.z))
            {
                return false;
            }
            var cell = cells[coord.x, coord.y, coord.z];
            return (cell.cellData &= (int)cellData) > 0;
        }

        public void SetState(IntVector coord, DungeonRoomVoxelCellType cellType, bool lockState)
        {
            if (IsValidCoord(coord.x, coord.y, coord.z))
            {
                var cell = cells[coord.x, coord.y, coord.z];
                if (!cell.stateLocked)
                {
                    cell.cellType = cellType;
                    cell.stateLocked = lockState;
                }
            }
        }

        public bool IsEmpty(int x, int y, int z)
        {
            if (!IsValidCoord(x, y, z))
            {
                return false;
            }
            return cells[x, y, z].cellType == DungeonRoomVoxelCellType.Empty;
        }

        class MarkerEmitCommand
        {
            public string markerName;
            public Vector3 position;
            public Quaternion rotation;
        }

        class MarkerEmitCommandList
        {
            public Dictionary<Vector3, MarkerEmitCommand> Map = new Dictionary<Vector3, MarkerEmitCommand>();
            public void Add(MarkerEmitCommand command)
            {
                Map.Add(command.position, command);
            }

            public void Remove(Vector3 position)
            {
                Map.Remove(position);
            }

            public bool Contains(Vector3 position, string markerName)
            {
                if (Map.ContainsKey(position))
                {
                    return Map[position].markerName == markerName;
                }
                return false;
            }
        }

        public void EmitMarkers(LevelMarkerList markerList, IntVector roomPosition, DungeonRoomDoorDesigner[] doors, Vector3 gridSize)
        {
            var commands = new MarkerEmitCommandList();

            for (int x = -1; x < cells.GetLength(0); x++)
            {
                for (int y = -1; y < cells.GetLength(1); y++)
                {
                    for (int z = -1; z < cells.GetLength(2); z++)
                    {
                        bool empty0 = IsEmpty(x, y, z);
                        bool emptyX = IsEmpty(x + 1, y, z);
                        bool emptyY = IsEmpty(x, y + 1, z);
                        bool emptyZ = IsEmpty(x, y, z + 1);

                        var markerName = DungeonRoomDesignerConstants.WallHalf;
                        if (!empty0 && emptyX) EmitMarker(markerName, x + 1, y, z + 0.5f, 90, commands);
                        if (empty0 && !emptyX) EmitMarker(markerName, x + 1, y, z + 0.5f, 270, commands);

                        if (!empty0 && emptyZ) EmitMarker(markerName, x + 0.5f, y, z + 1, 0, commands);
                        if (empty0 && !emptyZ) EmitMarker(markerName, x + 0.5f, y, z + 1, 180, commands);
                        
                        if (!empty0 && emptyY) EmitMarker(DungeonRoomDesignerConstants.Ground, x + 0.5f, y + 1, z + 0.5f, 0, commands);
                        if (empty0 && !emptyY) EmitMarker(DungeonRoomDesignerConstants.Ceiling, x + 0.5f, y + 1, z + 0.5f, Quaternion.Euler(180, 0, 0), commands);
                    }
                }
            }

            Pass_AddDoorMarkers(commands, doors);
            Pass_UpgradeWalls(commands);

            var roomPositionF = roomPosition.ToVector3();
            foreach (var command in commands.Map.Values)
            {
                var worldPosition = Vector3.Scale(command.position + roomPositionF, gridSize);

                var marker = new PropSocket();
                marker.Id = 0;
                marker.SocketType = command.markerName;
                marker.Transform = Matrix4x4.TRS(worldPosition, command.rotation, Vector3.one);
                markerList.Add(marker);
            }
        }

        void Pass_UpgradeWalls(MarkerEmitCommandList commands)
        {
            // Convert WallHalf to Walls where ever possible
            // This is done by checking if the existing WallHalf contains another WallHalf above it

            // First sort all the markers by Y
            var markers = commands.Map.Values.ToArray();
            markers.OrderBy(c => c.position.y);

            foreach (var marker in markers)
            {
                if (marker.markerName == DungeonRoomDesignerConstants.WallHalf)
                {
                    if (!commands.Contains(marker.position, DungeonRoomDesignerConstants.WallHalf))
                    {
                        // Already removed in the previous pass
                        continue;
                    }

                    var positionAbove = marker.position + new Vector3(0, 1, 0);
                    if (commands.Contains(positionAbove, DungeonRoomDesignerConstants.WallHalf))
                    {
                        // Remove the half wall above this
                        commands.Remove(positionAbove);

                        // Convert this WallHalf to a full Wall
                        commands.Map[marker.position].markerName = DungeonRoomDesignerConstants.Wall;
                    }
                }
            }
        }

        void Pass_AddDoorMarkers(MarkerEmitCommandList commands, DungeonRoomDoorDesigner[] doors)
        {
            foreach (var door in doors)
            {
                // Remove the existing markers at this position
                commands.Remove(door.logicalPosition);
                commands.Remove(door.logicalPosition + new Vector3(0, 1, 0));

                var command = new MarkerEmitCommand();
                command.markerName = DungeonRoomDesignerConstants.Door;
                command.position = door.logicalPosition;
                command.rotation = door.rotation;
                commands.Add(command);
            }
        }

        void EmitMarker(string markerName, float x, float y, float z, float angleY, MarkerEmitCommandList commands)
        {
            var rotation = Quaternion.Euler(0, angleY, 0);
            EmitMarker(markerName, x, y, z, rotation, commands);
        }

        void EmitMarker(string markerName, float x, float y, float z, Quaternion rotation, MarkerEmitCommandList commands)
        {
            var command = new MarkerEmitCommand();
            command.markerName = markerName;
            command.position = new Vector3(x, y, z);
            command.rotation = rotation;
            commands.Add(command);
        }


        public void IterateCells(System.Action<DungeonRoomVoxelCell, IntVector> callback)
        {
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    for (int z = 0; z < cells.GetLength(2); z++)
                    {
                        callback(cells[x, y, z], new IntVector(x, y, z));
                    }
                }
            }
        }

    }
}
