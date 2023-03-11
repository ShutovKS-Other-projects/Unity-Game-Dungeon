//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Impl.GridFlow;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Builders.GridFlow
{
    public class GridFlowDungeonQuery : DungeonEventListener
    {
        private FlowTilemap tilemap;
        private FlowLayoutGraph graph;
        private Vector3 gridSize = Vector3.zero;

        Dictionary<IntVector2, FlowLayoutGraphNode> nodesByCoord = new Dictionary<IntVector2, FlowLayoutGraphNode>();
        Dictionary<IntVector2, List<FlowTilemapCell>> tilesByNode = new Dictionary<IntVector2, List<FlowTilemapCell>>();
        Dictionary<IntVector2, List<FlowTilemapCell>> freeTilesByNode = new Dictionary<IntVector2, List<FlowTilemapCell>>();

        public override void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model)
        {
            base.OnPostDungeonLayoutBuild(dungeon, model);

            var gridFlowModel = model as GridFlowDungeonModel;
            if (gridFlowModel == null)
            {
                return;
            }

            var gridFlowConfig = dungeon.GetComponent<GridFlowDungeonConfig>();
            if (gridFlowConfig == null)
            {
                return;
            }

            tilemap = gridFlowModel.Tilemap;
            graph = gridFlowModel.LayoutGraph;
            gridSize = gridFlowConfig.gridSize;

            GenerateTileLookup();
        }

        public bool IsMainPath(Vector3 worldPosition)
        {
            var layoutNode = WorldCoordToLayoutNode(worldPosition);
            return layoutNode != null && layoutNode.mainPath;
        }
        
        public GridFlowLayoutNodeRoomType GetRoomType(Vector3 worldPosition)
        {
            var layoutNode = WorldCoordToLayoutNode(worldPosition);
            GridFlowLayoutNodeRoomType roomType = GridFlowLayoutNodeRoomType.Unknown;

            if (layoutNode != null)
            {
                var domainData = layoutNode.GetDomainData<GridFlowTilemapDomainData>();
                if (domainData != null)
                {
                    roomType = domainData.RoomType;
                }
            }

            return roomType;
        }

        public string GetPathName(Vector3 worldPosition)
        {
            var layoutNode = WorldCoordToLayoutNode(worldPosition);
            return layoutNode != null ? layoutNode.pathName : "";
        }
        
        public bool GetPathInfo(Vector3 worldPosition, out string pathName, out int pathIndex, out int pathLength)
        {
            var layoutNode = WorldCoordToLayoutNode(worldPosition);
            if (layoutNode == null)
            {
                pathName = "";
                pathIndex = -1;
                pathLength = -1;
                return false;
            }

            pathName = layoutNode.pathName;
            pathIndex = layoutNode.pathIndex;
            pathLength = layoutNode.pathLength;
            return true;
        }
        
        public FlowLayoutGraphNode GetLayoutNode(IntVector2 layoutNodeCoord)
        {
            if (!nodesByCoord.ContainsKey(layoutNodeCoord)) return null;
            return nodesByCoord[layoutNodeCoord];
        }

        /// <summary>
        /// Get all the tiles that belong to the layout node
        /// </summary>
        /// <param name="layoutNodeCoord"></param>
        /// <param name="onlyFreeTiles">will return only walkable tiles that are free. Use false if you want to decorate everything, true for gameplay logic</param>
        /// <returns></returns>
        public FlowTilemapCell[] GetLayoutNodeTile(IntVector2 layoutNodeCoord, bool onlyFreeTiles)
        {
            Dictionary<IntVector2, List<FlowTilemapCell>> lookup = onlyFreeTiles ? freeTilesByNode : tilesByNode;
            return lookup.ContainsKey(layoutNodeCoord) ? lookup[layoutNodeCoord].ToArray() : new FlowTilemapCell[0];
        }

        public Vector3 TileCoordToWorldCoord(IntVector2 tileCoord)
        {
            var basePosition = transform.position;
            return basePosition + Vector3.Scale(new Vector3(tileCoord.x + 0.5f, 0, tileCoord.y + 0.5f), gridSize);
        }


        public FlowLayoutGraphNode WorldCoordToLayoutNode(Vector3 worldPosition)
        {
            var tile = WorldCoordToTile(worldPosition);
            if (tile == null) return null;

            return GetLayoutNode(tile.NodeCoord);
        }

        
        public FlowTilemapCell WorldCoordToTile(Vector3 worldPosition)
        {
            if (tilemap == null) return null;

            var basePosition = transform.position;
            var tileCoordF = (worldPosition - basePosition);

            var tileX = Mathf.FloorToInt(tileCoordF.x / gridSize.x);
            var tileY = Mathf.FloorToInt(tileCoordF.z / gridSize.z);

            return tilemap.Cells.GetCell(tileX, tileY);
        }

        void GenerateTileLookup()
        {
            foreach (var node in graph.Nodes)
            {
                var coord = GetNodeCoord(node);
                nodesByCoord[coord] = node;
            }

            foreach (var cell in tilemap.Cells)
            {
                if (cell.CellType == FlowTilemapCellType.Floor)
                {
                    var nodeCoord = cell.NodeCoord;
                    if (!freeTilesByNode.ContainsKey(nodeCoord))
                    {
                        freeTilesByNode.Add(nodeCoord, new List<FlowTilemapCell>());
                    }

                    if (cell.Item == DungeonUID.Empty)
                    {
                        freeTilesByNode[nodeCoord].Add(cell);
                    }

                    if (!tilesByNode.ContainsKey(nodeCoord))
                    {
                        tilesByNode.Add(nodeCoord, new List<FlowTilemapCell>());
                    }

                    tilesByNode[nodeCoord].Add(cell);
                }
            }

            // Filter walkable paths on the free tiles (some free tile patches may be blocked by overlays like tree lines)
            var nodeKeys = new List<IntVector2>(freeTilesByNode.Keys);
            foreach (var nodeCoord in nodeKeys)
            {
                freeTilesByNode[nodeCoord] = FilterWalkablePath(freeTilesByNode[nodeCoord]);
            }
        }

        List<FlowTilemapCell> FilterWalkablePath(List<FlowTilemapCell> cells)
        {
            var unreachable = new HashSet<IntVector2>();
            var cellsByCoord = new Dictionary<IntVector2, FlowTilemapCell>();

            foreach (var cell in cells)
            {
                unreachable.Add(cell.TileCoord);
                cellsByCoord[cell.TileCoord] = cell;
            }

            var queue = new Queue<FlowTilemapCell>();
            foreach (var cell in cells)
            {
                if (cell.MainPath)
                {
                    unreachable.Remove(cell.TileCoord);
                    queue.Enqueue(cell);
                }
            }

            var childOffsets = new int[]
            {
                -1, 0,
                1, 0,
                0, -1,
                0, 1
            };

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                var coord = cell.TileCoord;
                for (int i = 0; i < 4; i++)
                {
                    var cx = coord.x + childOffsets[i * 2 + 0];
                    var cy = coord.y + childOffsets[i * 2 + 1];
                    var childCoord = new IntVector2(cx, cy);
                    if (unreachable.Contains(childCoord))
                    {
                        var canTraverse = true;
                        var childCell = cellsByCoord[childCoord];
                        if (childCell.Overlay != null && childCell.Overlay.tileBlockingOverlay)
                        {
                            canTraverse = false;
                        }

                        if (canTraverse)
                        {
                            unreachable.Remove(childCoord);
                            queue.Enqueue(cellsByCoord[childCoord]);
                        }
                    }
                }
            }


            // Grab all the cells that are not in the unreachable list
            var result = new List<FlowTilemapCell>();

            foreach (var cell in cells)
            {
                if (!unreachable.Contains(cell.TileCoord))
                {
                    result.Add(cell);
                }
            }

            return result;
        }

        IntVector2 GetNodeCoord(FlowLayoutGraphNode node)
        {
            var coordF = node.coord;
            return new IntVector2(Mathf.RoundToInt(coordF.x), Mathf.RoundToInt(coordF.y));
        }
    }
}