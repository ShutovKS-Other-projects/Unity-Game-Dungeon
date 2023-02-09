//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Domains.Tilemap.Tasks;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.GridFlow.Tasks
{

    [FlowExecNodeInfo("Finalize Tilemap", "Tilemap/", 2500)]
    public class GridFlowTilemapTaskFinalize : TilemapBaseFlowTaskFinalize
    {
        public bool debugUnwalkableCells = false;
        
        protected override bool AssignItems(FlowTilemap tilemap, FlowLayoutGraph graph, System.Random random, ref string errorMessage)
        {
            var nodesByCoord = new Dictionary<IntVector2, FlowLayoutGraphNode>();
            var freeTilesByNode = new Dictionary<IntVector2, List<FlowTilemapCell>>();

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
                }
            }

            // Filter walkable paths on the free tiles (some free tile patches may be blocked by overlays like tree lines)
            var nodeKeys = new List<IntVector2>(freeTilesByNode.Keys);
            foreach (var nodeCoord in nodeKeys)
            {
                freeTilesByNode[nodeCoord] = FilterWalkablePath(freeTilesByNode[nodeCoord]);
            }

            var distanceField = new FlowTilemapDistanceField(tilemap);
            // Add node items
            foreach (var node in graph.Nodes)
            {
                var coord = GetNodeCoord(node);
                if (freeTilesByNode.ContainsKey(coord))
                {
                    var freeTiles = freeTilesByNode[coord];
                    foreach (var item in node.items)
                    {
                        if (freeTiles.Count == 0)
                        {
                            errorMessage = "Item Placement failed. Insufficient free tiles";
                            return false;
                        }

                        var freeTileIndex = -1;
                        var context = new TilemapItemPlacementStrategyContext();
                        context.tilemap = tilemap;
                        context.distanceField = distanceField;
                        context.random = random;

                        string placementErrorMessage = "";
                        var placementSettings = item.GetDomainData<TilemapItemPlacementSettings>();
                        if (placementSettings != null)
                        {
                            var placementStrategy = TilemapItemPlacementStrategyFactory.Create(placementSettings.placementMethod);
                            var placementSuccess = false;
                            if (placementStrategy != null)
                            {
                                placementSuccess = placementStrategy.PlaceItems(item, freeTiles.ToArray(), placementSettings, context, ref freeTileIndex, ref placementErrorMessage);

                                // If we failed, try to fall back to random tile placement, if specified
                                if (!placementSuccess && placementSettings.fallbackToRandomPlacement)
                                {
                                    var randomPlacement = TilemapItemPlacementStrategyFactory.Create(TilemapItemPlacementMethod.RandomTile);
                                    placementSuccess = randomPlacement.PlaceItems(item, freeTiles.ToArray(), placementSettings, context, ref freeTileIndex, ref placementErrorMessage);
                                }
                            }

                            if (!placementSuccess)
                            {
                                errorMessage = "Item Placement failed. " + placementErrorMessage;
                                return false;
                            }
                            if (freeTileIndex < 0 || freeTileIndex >= freeTiles.Count)
                            {
                                errorMessage = "Item Placement failed. Invalid tile index";
                                return false;
                            }
                        }
                        else
                        {
                            freeTileIndex = random.Next(freeTiles.Count - 1);
                        }


                        var freeTile = freeTiles[freeTileIndex];
                        freeTile.Item = item.itemId;
                        freeTiles.Remove(freeTile);
                    }
                }
            }

            return true;
        }
        
        
        List<FlowTilemapCell> FilterWalkablePath(List<FlowTilemapCell> cells)
        {
            var unreachable = new HashSet<IntVector2>();
            var cellsByCoord = new Dictionary<IntVector2, FlowTilemapCell>();

            foreach(var cell in cells)
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

            if (debugUnwalkableCells)
            {
                foreach (var unreachableCoord in unreachable)
                {
                    var invalidCell = cellsByCoord[unreachableCoord];
                    invalidCell.CustomColor = Color.red;
                    invalidCell.UseCustomColor = true;
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
