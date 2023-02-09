//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Tilemap
{
    [System.Serializable]
    public enum FlowTilemapCellType
    {
        Empty,
        Floor,
        Wall,
        Door,
        Custom
    }

    [System.Serializable]
    public enum FlowTilemapEdgeType
    {
        Empty,
        Wall,
        Fence,
        Door
    }

    [System.Serializable]
    public class FlowTilemapCustomCellInfo
    {
        public string name;
        public Color defaultColor = Color.white;

        public override string ToString()
        {
            if (name.Length == 0)
            {
                return base.ToString();
            }
            return name;
        }
    }

    [System.Serializable]
    public enum FlowTilemapCellCategory
    {
        Layout,
        Biome,
        Elevation
    }

    [System.Serializable]
    public class FlowTilemapCellOverlay
    {
        public string markerName;
        public Color color;
        public float noiseValue { get; set; }

        /// <summary>
        /// Specifies if the overlay blocks the tile (like a rock overlay) or doesn't block a tile (like grass overlay), so items can be placed on top of it
        /// </summary>
        public bool tileBlockingOverlay = true;

        public FlowTilemapCellOverlayMergeConfig mergeConfig;

        public FlowTilemapCellOverlay Clone()
        {
            var newOverlay = new FlowTilemapCellOverlay();
            newOverlay.markerName = markerName;
            newOverlay.color = color;
            newOverlay.noiseValue = noiseValue;
            newOverlay.tileBlockingOverlay = tileBlockingOverlay;
            newOverlay.mergeConfig = mergeConfig.Clone();

            return newOverlay;
        }
    }

    [System.Serializable]
    public enum FlowTilemapCellOverlayMergeWallOverlayRule
    {
        KeepWallAndOverlay,
        KeepWallRemoveOverlay,
        KeepOverlayRemoveWall
    }

    [System.Serializable]
    public class FlowTilemapCellOverlayMergeConfig
    {
        public float minHeight = 0;
        public float maxHeight = 0;
        public FlowTilemapCellOverlayMergeWallOverlayRule wallOverlayRule = FlowTilemapCellOverlayMergeWallOverlayRule.KeepWallAndOverlay;
        public float markerHeightOffsetForLayoutTiles = 0;
        public float markerHeightOffsetForNonLayoutTiles = 0;
        public bool removeElevationMarker = false;

        public FlowTilemapCellOverlayMergeConfig Clone()
        {
            var newConfig = new FlowTilemapCellOverlayMergeConfig();
            newConfig.minHeight = minHeight;
            newConfig.maxHeight = maxHeight;
            newConfig.wallOverlayRule = wallOverlayRule;
            newConfig.markerHeightOffsetForLayoutTiles = markerHeightOffsetForLayoutTiles;
            newConfig.markerHeightOffsetForNonLayoutTiles = markerHeightOffsetForNonLayoutTiles;
            newConfig.removeElevationMarker = removeElevationMarker;
            return newConfig;
        }
    }


    [System.Serializable]
    public class FlowTilemapEdge
    {
        public FlowTilemapEdgeType EdgeType = FlowTilemapEdgeType.Empty;
        public DungeonUID Item = DungeonUID.Empty;
        public IntVector2 EdgeCoord;
        public bool HorizontalEdge = true;
        public object Userdata = null;

        public FlowTilemapEdge Clone()
        {
            var clone = new FlowTilemapEdge();
            clone.EdgeType = EdgeType;
            clone.Item = Item;
            clone.EdgeCoord = EdgeCoord;
            clone.HorizontalEdge = HorizontalEdge;

            if (Userdata != null && Userdata is System.ICloneable)
            {
                clone.Userdata = (Userdata as System.ICloneable).Clone();
            }
            return clone;
        }
    }

    [System.Serializable]
    public class FlowTilemapCell
    {
        public FlowTilemapCellType CellType = FlowTilemapCellType.Empty;
        public FlowTilemapCustomCellInfo CustomCellInfo = null;
        public DungeonUID Item = DungeonUID.Empty;
        public string[] Tags = new string[0];
        public FlowTilemapCellOverlay Overlay;
        public IntVector2 NodeCoord;
        public IntVector2 TileCoord;
        public bool UseCustomColor = false;
        public Color CustomColor = Color.white;
        public bool MainPath = false;
        public bool LayoutCell = false;
        public int DistanceFromMainPath = int.MaxValue;
        public float Height = 0;
        public object Userdata = null;

        public FlowTilemapCell Clone()
        {
            var newCell = new FlowTilemapCell();
            newCell.CellType = CellType;
            newCell.CustomCellInfo = CustomCellInfo;
            newCell.Item = Item;
            newCell.Tags = new List<string>(Tags).ToArray();
            newCell.Overlay = (Overlay != null) ? Overlay.Clone() : null;
            newCell.NodeCoord = NodeCoord;
            newCell.TileCoord = TileCoord;
            newCell.UseCustomColor = UseCustomColor;
            newCell.CustomColor = CustomColor;
            newCell.MainPath = MainPath;
            newCell.LayoutCell = LayoutCell;
            newCell.DistanceFromMainPath = DistanceFromMainPath;
            newCell.Height = Height;

            if (Userdata != null && Userdata is System.ICloneable)
            {
                newCell.Userdata = (Userdata as System.ICloneable).Clone();
            }
            return newCell;
        }

        public void Clear()
        {
            CellType = FlowTilemapCellType.Empty;
            CustomCellInfo = null;
            Item = DungeonUID.Empty;
            Tags = new string[0];
            Overlay = null;
            UseCustomColor = false;
            MainPath = false;
            LayoutCell = false;
            DistanceFromMainPath = int.MaxValue;
            Height = 0;
            Userdata = null;
        }
    }

    [System.Serializable]
    public class FlowTilemapCellDoorInfo : System.ICloneable
    {
        public bool locked = false;
        public bool oneWay = false;
        public IntVector2 nodeA;
        public IntVector2 nodeB;

        public object Clone()
        {
            var newObj = new FlowTilemapCellDoorInfo();
            newObj.locked = locked;
            newObj.oneWay = oneWay;
            newObj.nodeA = nodeA;
            newObj.nodeB = nodeB;
            return newObj;
        }
    }

    [System.Serializable]
    public class FlowTilemapCellWallInfo : System.ICloneable
    {
        public List<IntVector2> owningNodes = new List<IntVector2>();

        public object Clone()
        {
            var newObj = new FlowTilemapCellWallInfo();
            newObj.owningNodes = new List<IntVector2>(owningNodes);
            return newObj;
        }
    }

    // Disabling serialization as this is making things slow
    // TODO: Fix serialization issue
    //[System.Serializable]
    public class FlowTilemap : ICloneable
    {
        public int Width;
        public int Height;

        [SerializeField]
        [HideInInspector]
        public FlowTilemapCellDatabase Cells;

        [SerializeField]
        [HideInInspector]
        public FlowTilemapEdgeDatabase Edges;

        public FlowTilemap(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            Cells = new FlowTilemapCellDatabase(Width, Height);
            Edges = new FlowTilemapEdgeDatabase(Width, Height);
        }

        public object Clone()
        {
            var newTilemap = new FlowTilemap(Width, Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    newTilemap.Cells[x, y] = Cells[x, y].Clone();
                }
            }

            for (int x = 0; x <= Width; x++)
            {
                for (int y = 0; y <= Height; y++)
                {
                    newTilemap.Edges.SetHorizontal(x, y, Edges.GetHorizontal(x, y).Clone());
                    newTilemap.Edges.SetVertical(x, y, Edges.GetVertical(x, y).Clone());
                }
            }

            return newTilemap;
        }

    }

    [System.Serializable]
    public class FlowTilemapEdgeDatabase : IEnumerable<FlowTilemapEdge>
    {
        [SerializeField]
        private FlowTilemapEdge[] edgesHorizontal;

        [SerializeField]
        private FlowTilemapEdge[] edgesVertical;

        [SerializeField]
        private int width;

        [SerializeField]
        private int height;


        public FlowTilemapEdgeDatabase(int tilemapWidth, int tilemapHeight)
        {
            width = tilemapWidth + 1;
            height = tilemapHeight + 1;
            var numElements = width * height;
            edgesHorizontal = new FlowTilemapEdge[numElements];
            edgesVertical = new FlowTilemapEdge[numElements];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var index = Index(x, y);

                    // Create Horizontal edge
                    {
                        var edgeH = new FlowTilemapEdge();
                        edgeH.EdgeCoord = new IntVector2(x, y);
                        edgeH.HorizontalEdge = true;
                        edgesHorizontal[index] = edgeH;
                    }

                    // Create Vertical edge
                    {
                        var edgeV = new FlowTilemapEdge();
                        edgeV.EdgeCoord = new IntVector2(x, y);
                        edgeV.HorizontalEdge = false;
                        edgesVertical[index] = edgeV;
                    }
                }
            }

        }

        public FlowTilemapEdge GetHorizontal(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return null;
            return edgesHorizontal[Index(x, y)];
        }

        public FlowTilemapEdge GetVertical(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return null;
            return edgesVertical[Index(x, y)];
        }

        public void SetHorizontal(int x, int y, FlowTilemapEdge edge)
        {
            edgesHorizontal[Index(x, y)] = edge;
        }

        public void SetVertical(int x, int y, FlowTilemapEdge edge)
        {
            edgesVertical[Index(x, y)] = edge;
        }


        private int Index(int x, int y)
        {
            return y * width + x;
        }

        IEnumerator<FlowTilemapEdge> IEnumerable<FlowTilemapEdge>.GetEnumerator()
        {
            return new FlowTilemapEdgeDatabaseEnumerator(edgesHorizontal, edgesVertical);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FlowTilemapEdgeDatabaseEnumerator(edgesHorizontal, edgesVertical);
        }
    }

    /// <summary>
    /// This class gives a 2D grid view for an underlying 1D array
    /// Unity serialization requires a 1-dimensional array, hence the need for this class
    /// </summary>
    [System.Serializable]
    public class FlowTilemapCellDatabase : IEnumerable<FlowTilemapCell>
    {
        [SerializeField]
        private FlowTilemapCell[] cells;

        [SerializeField]
        private int width;

        [SerializeField]
        private int height;

        public FlowTilemapCellDatabase(int width, int height)
        {
            this.width = width;
            this.height = height;
            cells = new FlowTilemapCell[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = new FlowTilemapCell();
                    cell.TileCoord = new IntVector2(x, y);
                    this[x, y] = cell;
                }
            }
        }

        public FlowTilemapCell this[int x, int y]
        {
            get
            {
                return cells[Index(x, y)];
            }
            set
            {
                cells[Index(x, y)] = value;
            }
        }

        public FlowTilemapCell GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return null;
            return this[x, y];
        }

        private int Index(int x, int y)
        {
            return y * width + x;
        }

        IEnumerator<FlowTilemapCell> IEnumerable<FlowTilemapCell>.GetEnumerator()
        {
            return new FlowTilemapCellDatabaseEnumerator(cells);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FlowTilemapCellDatabaseEnumerator(cells);
        }
    }


    public class FlowTilemapCellDatabaseEnumerator : IEnumerator<FlowTilemapCell>
    {
        int position = -1;
        FlowTilemapCell[] cells = null;
        FlowTilemapCell current;

        public FlowTilemapCellDatabaseEnumerator(FlowTilemapCell[] cells)
        {
            this.cells = cells;
        }

        public void Dispose()
        {
            cells = null;
            current = null;
        }

        public bool MoveNext()
        {
            if (++position >= cells.Length)
            {
                return false;
            }

            // Set current box to next item in collection.
            current = cells[position];
            return true;
        }

        public void Reset()
        {
            position = -1;
            current = null;
        }

        public FlowTilemapCell Current
        {
            get { return current; }
        }


        object IEnumerator.Current
        {
            get { return Current; }
        }
    }

    public class FlowTilemapEdgeDatabaseEnumerator : IEnumerator<FlowTilemapEdge>
    {
        int position = -1;
        FlowTilemapEdge[] edgesH = null;
        FlowTilemapEdge[] edgesV = null;
        FlowTilemapEdge current;

        public FlowTilemapEdgeDatabaseEnumerator(FlowTilemapEdge[] edgesH, FlowTilemapEdge[] edgesV)
        {
            this.edgesH = edgesH;
            this.edgesV = edgesV;
        }

        public void Dispose()
        {
            edgesH = null;
            edgesV = null;
            current = null;
        }

        public bool MoveNext()
        {
            ++position;

            if (position >= edgesH.Length + edgesV.Length)
            {
                return false;
            }

            int index = position;
            if (index < edgesH.Length)
            {
                current = edgesH[index];
            }
            else
            {
                index -= edgesH.Length;
                current = edgesV[index];
            }

            return true;
        }

        public void Reset()
        {
            position = -1;
            current = null;
        }

        public FlowTilemapEdge Current
        {
            get { return current; }
        }


        object IEnumerator.Current
        {
            get { return Current; }
        }
    }


    public class FlowTilemapDistanceFieldCell
    {
        public int DistanceFromEdge = int.MaxValue;
        public int DistanceFromDoor = int.MaxValue;
    }

    public class FlowTilemapDistanceField
    {
        FlowTilemap tilemap;
        public FlowTilemapDistanceFieldCell[,] distanceCells;

        public FlowTilemapDistanceField(FlowTilemap tilemap)
        {
            this.tilemap = tilemap;
            distanceCells = new FlowTilemapDistanceFieldCell[tilemap.Width, tilemap.Height];
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    distanceCells[x, y] = new FlowTilemapDistanceFieldCell();
                }
            }


            Build();
        }

        private static int[] childOffsets = new int[]
        {
                -1, 0,
                1, 0,
                0, -1,
                0, 1
        };

        void Build()
        {
            FindDistanceFromEdge();
            FindDistanceFromDoor();
        }

        struct NeighborData
        {
            public FlowTilemapCell cell;
            public FlowTilemapEdge edge;
        }

        NeighborData[] GetNeighbourData(FlowTilemapCell cell)
        {
            var coord = cell.TileCoord;
            var left = new NeighborData
            {
                cell = tilemap.Cells.GetCell(coord.x - 1, coord.y),
                edge = tilemap.Edges.GetVertical(coord.x, coord.y)
            };
            
            var right = new NeighborData
            {
                cell = tilemap.Cells.GetCell(coord.x + 1, coord.y),
                edge = tilemap.Edges.GetVertical(coord.x + 1, coord.y)
            };
            
            var down = new NeighborData
            {
                cell = tilemap.Cells.GetCell(coord.x, coord.y - 1),
                edge = tilemap.Edges.GetHorizontal(coord.x, coord.y)
            };

            var up = new NeighborData
            {
                cell = tilemap.Cells.GetCell(coord.x, coord.y + 1),
                edge = tilemap.Edges.GetHorizontal(coord.x, coord.y + 1)
            };

            return new[] { left, up, right, down };
        }
        
        void FindDistanceFromEdge()
        {
            var queue = new Queue<FlowTilemapCell>();
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    var cell = tilemap.Cells[x, y];
                    if (cell.CellType == FlowTilemapCellType.Floor)
                    {
                        bool allNeighborsWalkable = true;
                        var ndata = GetNeighbourData(cell);
                        foreach (var neighbour in ndata)
                        {
                            var ncell = neighbour.cell;
                            if (ncell != null)
                            {

                                if (ncell.CellType != FlowTilemapCellType.Floor)
                                {
                                    allNeighborsWalkable = false;
                                    break;
                                }

                                // Check if there's a blocking overlay
                                if (cell.Overlay != null && cell.Overlay.tileBlockingOverlay)
                                {
                                    allNeighborsWalkable = false;
                                    break;
                                }
                            }

                            var nedge = neighbour.edge;
                            if (nedge != null)
                            {
                                if (nedge.EdgeType != FlowTilemapEdgeType.Empty)
                                {
                                    allNeighborsWalkable = false;
                                    break;
                                }
                            }
                        }
                        
                        if (!allNeighborsWalkable)
                        {
                            queue.Enqueue(cell);
                            distanceCells[x, y].DistanceFromEdge = 0;
                        }
                    }
                }
            }

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();

                var x = cell.TileCoord.x;
                var y = cell.TileCoord.y;
                var ndist = distanceCells[x, y].DistanceFromEdge + 1;

                var ndata = GetNeighbourData(cell);
                foreach (var neighbour in ndata)
                {
                    var ncell = neighbour.cell;
                    if (ncell != null)
                    {
                        var ncoord = ncell.TileCoord;
                        var walkableTile = (ncell.CellType == FlowTilemapCellType.Floor);
                        if (walkableTile && cell.Overlay != null && cell.Overlay.tileBlockingOverlay)
                        {
                            walkableTile = false;
                        }

                        var nedge = neighbour.edge;
                        if (nedge != null)
                        {
                            walkableTile &= (nedge.EdgeType == FlowTilemapEdgeType.Empty);
                        }
                        
                        if (walkableTile && ndist < distanceCells[ncoord.x, ncoord.y].DistanceFromEdge)
                        {
                            distanceCells[ncoord.x, ncoord.y].DistanceFromEdge = ndist;
                            queue.Enqueue(ncell);
                        }
                    }
                }
            }
        }

        void FindDistanceFromDoor()
        {
            var queue = new Queue<FlowTilemapCell>();
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    var cell = tilemap.Cells[x, y];
                    if (cell.CellType == FlowTilemapCellType.Door)
                    {
                        queue.Enqueue(cell);
                        distanceCells[x, y].DistanceFromDoor = 0;
                    }
                    else
                    {
                        var ndata = GetNeighbourData(cell);
                        foreach (var neighbor in ndata)
                        {
                            var nedge = neighbor.edge;
                            if (nedge != null && nedge.EdgeType == FlowTilemapEdgeType.Door)
                            {
                                queue.Enqueue(cell);
                                distanceCells[x, y].DistanceFromDoor = 1;   // 1, Since we are not on the door cell
                                break;
                            }
                        }
                    }
                }
            }

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();

                var x = cell.TileCoord.x;
                var y = cell.TileCoord.y;
                var ndist = distanceCells[x, y].DistanceFromDoor + 1;

                
                var ndata = GetNeighbourData(cell);
                foreach (var neighbour in ndata)
                {
                    var ncell = neighbour.cell;
                    var nedge = neighbour.edge;
                    if (ncell == null || nedge == null) continue;
                    
                    var ncoord = ncell.TileCoord;
                    
                    var walkableTile = (ncell.CellType == FlowTilemapCellType.Floor && nedge.EdgeType == FlowTilemapEdgeType.Empty);
                    if (walkableTile && cell.Overlay != null && cell.Overlay.tileBlockingOverlay)
                    {
                        walkableTile = false;
                    }

                    if (walkableTile && ndist < distanceCells[ncoord.x, ncoord.y].DistanceFromDoor)
                    {
                        distanceCells[ncoord.x, ncoord.y].DistanceFromDoor = ndist;
                        queue.Enqueue(ncell);
                    }
                }
                
                for (int i = 0; i < 4; i++)
                {
                    var nx = x + childOffsets[i * 2 + 0];
                    var ny = y + childOffsets[i * 2 + 1];
                    var ncell = tilemap.Cells.GetCell(nx, ny);
                }
            }
        }
    }
}
