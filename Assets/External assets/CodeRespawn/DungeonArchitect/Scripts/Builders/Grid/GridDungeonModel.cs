//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DungeonArchitect.Builders.Grid
{
    /// <summary>
    /// The type of cell used in the grid builder
    /// </summary>
	[System.Serializable]
    public enum CellType
    {
        Room,
        Corridor,
        CorridorPadding,
        Unknown
    }

    /// <summary>
    /// Data-structure to hold the stair information in the grid based builder
    /// </summary>
    [System.Serializable]
    public class StairInfo
    {
        [SerializeField]
        public int OwnerCell;

        [SerializeField]
        public int ConnectedToCell;

        [SerializeField]
        public Vector3 Position;

        [SerializeField]
        public Quaternion Rotation;

        [SerializeField]
        public IntVector IPosition;
    };

    /// <summary>
    /// Data-structure to hold the Cell information. A cell is a piece of the dungeon layout and can be either a room or a corridor
    /// </summary>
    [Serializable]
    public class Cell
    {
        public Cell()
        {
            cellType = CellType.Unknown;
        }
        public Cell(int x, int z, int width, int length)
        {
            bounds = new Rectangle(x, z, width, length);
            cellType = CellType.Unknown;
        }

        [SerializeField]
        int id;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        [SerializeField]
        Rectangle bounds = new Rectangle();
        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }
		
		[SerializeField]
        CellType cellType = CellType.Unknown;
        public CellType CellType
        {
            get { return cellType; }
            set { cellType = value; }
        }

        public Bounds GetWorldBounds(Vector3 gridSize)
        {
            var bounds = new Bounds();
            var width = this.Bounds.Width * gridSize.x;
            var length = this.Bounds.Width * gridSize.z;
            var center = Vector3.Scale(this.Bounds.CenterF(), gridSize);
            bounds.center = center;
            bounds.size = new Vector3(width, 2, length);
            return bounds;
        }

        /// <summary>
        /// Indicates if the cell is user defined and not procedurally generated
        /// </summary>
        [SerializeField]
        bool userDefined;
        public bool UserDefined
        {
            get
            {
                return userDefined;
            }
            set
            {
                userDefined = value;
            }
        }

        [SerializeField]
        HashSet<int> connectedRooms = new HashSet<int>();
        public HashSet<int> ConnectedRooms
        {
            get { return connectedRooms; }
            set { connectedRooms = value; }
        }

        [SerializeField]
        HashSet<int> fixedRoomConnections = new HashSet<int>();
        public HashSet<int> FixedRoomConnections
        {
            get { return fixedRoomConnections; }
            set { fixedRoomConnections = value; }
        }

        [SerializeField]
        HashSet<int> adjacentCells = new HashSet<int>();
        public HashSet<int> AdjacentCells
        {
            get
            {
                return adjacentCells;
            }
            set
            {
                adjacentCells = value;
            }
        }

        public IntVector Center
        {
            get
            {
                return new IntVector(bounds.X + bounds.Width / 2, 0, bounds.Z + bounds.Length / 2);
            }
        }

        public Vector3 CenterF
        {
            get
            {
                return new Vector3(bounds.X + bounds.Width / 2.0f, bounds.Location.y, bounds.Z + bounds.Length / 2.0f);
            }
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null) return false;
            if (obj is Cell)
            {
                var cell = obj as Cell;
                return this.Id == cell.Id;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return id;
        }
    }

    /// <summary>
    /// Data-structure to hold the door information
    /// </summary>
    [Serializable]
    public class CellDoor
    {
        [SerializeField]
        IntVector[] adjacentTiles = new IntVector[2];

        /// <summary>
        /// The adjacent tile positions shared by this door (entry / exit tiles)
        /// </summary>
        public IntVector[] AdjacentTiles
        {
            get
            {
                return adjacentTiles;
            }
        }


        [SerializeField]
        bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Cell Ids of one of the cells that owns this door
        /// </summary>
        [SerializeField]
        int[] adjacentCells = new int[2];
        public int[] AdjacentCells
        {
            get
            {
                return adjacentCells;
            }
            set
            {
                adjacentCells = value;
            }
        }


        public override string ToString()
        {
            return string.Format("[CellDoor: {0} <=> {1}]", AdjacentCells[0], AdjacentCells[1]);
        }
    }

    /// <summary>
    /// Data-structure for IntVector pair. Used for caching
    /// </summary>
    [Serializable]
    public struct IntVector2Key
    {
        [SerializeField]
        public IntVector a;

        [SerializeField]
        public IntVector b;

        public IntVector2Key(IntVector a, IntVector b)
        {
            this.a = a;
            this.b = b;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj != null && obj is IntVector2Key)
            {
                var other = (IntVector2Key)obj;
                return a.Equals(other.a) &&
                    b.Equals(other.b);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (a.GetHashCode() << 16) + b.GetHashCode();
        }
    }

    /// <summary>
    /// Manages the doors in the grid based builder
    /// </summary>
    [Serializable]
    public class DoorManager
    {
        [SerializeField]
        Dictionary<IntVector2Key, CellDoor> doorLookupCache = new Dictionary<IntVector2Key, CellDoor>();

        [SerializeField]
        List<CellDoor> doors = new List<CellDoor>();

        public void Clear()
        {
            doorLookupCache.Clear();
            doors.Clear();
        }

        public void RemoveDoor(CellDoor door)
        {
            var keysToRemove = new List<IntVector2Key>();
            foreach (var key in doorLookupCache.Keys)
            {
                if (doorLookupCache[key] == door)
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                doorLookupCache.Remove(key);
            }
            doors.Remove(door);
        }

        /// <summary>
        /// Creates a door between the two grid points
        /// </summary>
        /// <param name="p1">The grid poition 1</param>
        /// <param name="p2">The grid poition 2</param>
        /// <param name="cellId1">Cell Id of the first adjacent cell</param>
        /// <param name="cellId2">Cell Id of the second adjacent cell</param>
        /// <returns></returns>
        public CellDoor CreateDoor(IntVector p1, IntVector p2, int cellId1, int cellId2)
        {
            var key1 = new IntVector2Key(p1, p2);
            if (doorLookupCache.ContainsKey(key1)) return doorLookupCache[key1];
            var key2 = new IntVector2Key(p2, p1);
            if (doorLookupCache.ContainsKey(key2)) return doorLookupCache[key2];

            // Create a new door
            var door = new CellDoor();
            door.AdjacentTiles[0] = new IntVector(p1.x, p1.y, p1.z);
            door.AdjacentTiles[1] = new IntVector(p2.x, p2.y, p2.z);

            door.AdjacentCells[0] = cellId1;
            door.AdjacentCells[1] = cellId2;

            // Add to the memo lookup
            doorLookupCache.Add(key1, door);
            doorLookupCache.Add(key2, door);

            doors.Add(door);
            return door;
        }

        /// <summary>
        /// Check if a door exists between the two cells 
        /// </summary>
        /// <param name="cellA">Cell Id of the first cell</param>
        /// <param name="cellB">Cell Id of the second cell</param>
        /// <returns></returns>
        public bool ContainsDoorBetweenCells(int cellA, int cellB)
        {
            foreach (var door in doors)
            {
                if (!door.Enabled)
                {
                    continue;
                }
                if ((door.AdjacentCells[0] == cellA && door.AdjacentCells[1] == cellB) ||
                        (door.AdjacentCells[0] == cellB && door.AdjacentCells[1] == cellA))
                {
                    return true;
                }
            }
            return false;
        }

		public bool ContainsDoor(int x1, int z1, int x2, int z2) {
			foreach (var door in doors) {
                if (!door.Enabled) { continue; }
				var containsDoor = 
						door.AdjacentTiles[0].x == x1 && door.AdjacentTiles[0].z == z1 &&
						door.AdjacentTiles[1].x == x2 && door.AdjacentTiles[1].z == z2;
				if (containsDoor) {
					return true;
				}

				containsDoor = 
						door.AdjacentTiles[1].x == x1 && door.AdjacentTiles[1].z == z1 &&
						door.AdjacentTiles[0].x == x2 && door.AdjacentTiles[0].z == z2;
				if (containsDoor) {
					return true;
				}
			}
			return false;
		}

        /// <summary>
        /// List of registered doors
        /// </summary>
        public CellDoor[] Doors
        {
            get
            {
                return doors.ToArray();
            }
        }
    }


    /// <summary>
    /// Data model for the grid based dungeon builder
	/// </summary>
	//[System.Serializable]
    public class GridDungeonModel : DungeonModel
    {
		[HideInInspector]
        [SerializeField]
        public DoorManager DoorManager = new DoorManager();
        
        [HideInInspector]
        public GridDungeonConfig Config;
        
        [SerializeField]
        [HideInInspector]
        public List<Cell> Cells = new List<Cell>();
		
		[HideInInspector]
        [SerializeField]
        public Dictionary<int, List<StairInfo>> CellStairs = new Dictionary<int, List<StairInfo>>();

        [HideInInspector]
        public Dictionary<int, Dictionary<int, GridCellInfo>> GridCellInfoLookup = new Dictionary<int, Dictionary<int, GridCellInfo>>();

        /// <summary>
        /// Get meta-data about the grid in x, z grid coordinate
        /// </summary>
        /// <param name="x">X value in grid coordinate</param>
        /// <param name="z">Z value in grid cooridnate</param>
        /// <returns></returns>
        public GridCellInfo GetGridCellLookup(int x, int z)
        {
            if (!GridCellInfoLookup.ContainsKey(x) || !GridCellInfoLookup[x].ContainsKey(z))
            {
                return new GridCellInfo();
            }
            return GridCellInfoLookup[x][z];
        }

        /// <summary>
        /// Builds a lookup for fast data retrieval
        /// </summary>
        public void BuildSpatialCellLookup()
        {
            // Cache the cell types based on their positions
            GridCellInfoLookup.Clear();
            foreach (var cell in Cells)
            {
                if (cell.CellType == CellType.Unknown) continue;
                IntVector basePosition = cell.Bounds.Location;
                for (int dx = 0; dx < cell.Bounds.Size.x; dx++)
                {
                    for (int dz = 0; dz < cell.Bounds.Size.z; dz++)
                    {
                        int x = basePosition.x + dx;
                        int z = basePosition.z + dz;

                        // register the cell type in the lookup
                        if (!GridCellInfoLookup.ContainsKey(x)) GridCellInfoLookup.Add(x, new Dictionary<int, GridCellInfo>());
                        if (!GridCellInfoLookup[x].ContainsKey(z))
                        {
                            GridCellInfoLookup[x].Add(z, new GridCellInfo(cell.Id, cell.CellType));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The list of registered doors
        /// </summary>
        public CellDoor[] Doors
        {
            get
            {
                return DoorManager.Doors;
            }
        }

        /// <summary>
        /// Builds the cell lookup for faster cell retrieval
        /// </summary>
        public void BuildCellLookup()
        {
            CellLookup.Clear();
            foreach (var cell in Cells)
            {
                CellLookup.Add(cell.Id, cell);
            }
        }

        /// <summary>
        /// Gets the cell information
        /// </summary>
        /// <param name="Id">Id of the cell to lookup</param>
        /// <returns></returns>
        public Cell GetCell(int Id)
        {
            return CellLookup.ContainsKey(Id) ? CellLookup[Id] : null;
        }

        /// <summary>
        /// Cell lookup based on the Cell Id
        /// </summary>
        public Dictionary<int, Cell> CellLookup = new Dictionary<int, Cell>();

        /// <summary>
        /// Finds the cell based on the position in grid coordinates
        /// </summary>
        /// <param name="position">Position to lookup in grid cooridnates</param>
        /// <returns>Cell information at that location.  Returns null if none found</returns>
        public Cell FindCellByPosition(IntVector position)
        {
            foreach (var cell in Cells)
            {
                if (cell.Bounds.Contains(position))
                {
                    return cell;
                }
            }
            return null;
        }

        /// <summary>
        /// Clears the dungeon data model
        /// </summary>
        public override void ResetModel()
        {
            DoorManager = new DoorManager();
            Config = null;
            Cells = new List<Cell>();
            CellStairs.Clear();
        }

        public bool ContainsStairAtLocation(int x, int z)
        {
            foreach (var stairList in CellStairs.Values) {
                foreach (var stair in stairList) {
                    if (stair.IPosition.x == x && stair.IPosition.z == z) {
                        return true;
                    }
                }
            }
            return false;
        }

		public StairInfo GetStairAtLocation(int x, int z) {
			foreach (var stairList in CellStairs.Values) {
				foreach (var stair in stairList) {
					if (stair.IPosition.x == x && stair.IPosition.z == z) {
						return stair;
					}
				}
			}
			return null;
		}

        /// <summary>
        /// Check if a stair exists between the two cells
        /// </summary>
        /// <param name="cellA"></param>
        /// <param name="cellB"></param>
        /// <returns></returns>
        public bool ContainsStair(int cellA, int cellB)
        {
            return CheckContainStair(cellA, cellB) || CheckContainStair(cellB, cellA);
        }
        bool CheckContainStair(int cellA, int cellB)
        {
            if (!CellStairs.ContainsKey(cellA)) return false;

            foreach (var stair in CellStairs[cellA])
            {
                if (stair.ConnectedToCell == cellB) return true;
            }
            return false;
        }
    }

	public class GridDungeonModelUtils {
		struct LongestPathBFSData {
			public Cell cell;
			public int distance;
		}

        public static Cell[] FindFurthestRooms(GridDungeonModel model)
        {
            return FindFurthestRooms(model, CellType.Room, CellType.Room);
        }

        public static Cell[] FindFurthestRooms(GridDungeonModel model, CellType startCellType, CellType endCellType) {
			var result = new Cell[2];
            var bestDistance = 0;
			foreach (var startCell in model.Cells) {
                if (startCell.CellType != startCellType)
                {
                    continue;
                }
				var queue = new Queue<LongestPathBFSData>();
				var startData = new LongestPathBFSData { cell = startCell, distance = 0 };
				queue.Enqueue(startData);
                LongestPathBFSData bestEndCell = startData;
                var cellDistances = new Dictionary<int, int>();    // CellId -> Distance mapping
                
				while (queue.Count > 0) {
					var front = queue.Dequeue();
                    if (front.cell == null) continue;

                    var processCell = false;
                    if (!cellDistances.ContainsKey(front.cell.Id))
                    {
                        // We never processed this cell
                        processCell = true;
                    }
                    else if (cellDistances[front.cell.Id] > front.distance)
                    {
                        // We found a shorter path to this cell
                        processCell = true;
                    }

                    if (processCell)
                    {
                        cellDistances[front.cell.Id] = front.distance;
                        foreach (var childId in front.cell.AdjacentCells) {
                            var child = model.GetCell(childId);
                            var childData = new LongestPathBFSData { cell = child, distance = front.distance + 1 };
                            queue.Enqueue(childData);
                        }
                    }

                    if (front.cell.CellType == endCellType && front.distance > bestEndCell.distance)
                    {
                        bestEndCell = front;
                    }
				}

                if (bestEndCell.distance > bestDistance)
                {
                    result[0] = startData.cell;
                    result[1] = bestEndCell.cell;
                    bestDistance = bestEndCell.distance;
                }
			}

			return result;
		}
	}

}
