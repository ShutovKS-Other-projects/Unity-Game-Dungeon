//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Domains.Tilemap.Tasks;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.GridFlow.Tasks
{
    [FlowExecNodeInfo("Initialize Tilemap", "Tilemap/", 2000)]
    public class GridFlowTilemapTaskInitialize : TilemapBaseFlowTaskInitialize
    {
        public Vector2Int tilemapSizePerNode = new Vector2Int(10, 10);
        public Vector2 perturbAmount = new Vector2(3, 3);
        public float corridorLaneWidth = 2;
        public int layoutPadding = 0;
        public bool cropTilemap = true;
        public TilemapFlowNodeWallGenerationMethod wallGenerationMethod = TilemapFlowNodeWallGenerationMethod.WallAsTiles;

        public int caveAutomataNeighbors = 5;
        public int caveAutomataIterations = 4;
        public float caveThickness = 2.25f;

        public float roomColorSaturation = 0.3f;
        public float roomColorBrightness = 1.5f;

        private int nodeWidth;
        private int nodeHeight;
        
        protected override bool Validate(FlowTaskExecContext context, FlowTaskExecInput input, FlowTaskExecOutput output)
        {
            var graph = output.State.GetState<FlowLayoutGraph>();
            if (graph == null)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return false;
            }

            if (graph.Nodes.Count == 0)
            {
                output.ErrorMessage = "Invalid graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return false;
            }

            return true;
        }

        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            var output = base.Execute(context, input);

            var tilemapState = new GridFlowTilemapState();
            tilemapState.WallGenerationMethod = wallGenerationMethod;
            output.State.SetState(typeof(GridFlowTilemapState), tilemapState);
            
            return output;
        }
        protected override FlowTilemap BuildTilemap(FlowLayoutGraph graph, System.Random random)
        {
            IntVector2 abstractGridSize;
            if (!GetGraphSize(graph, out abstractGridSize))
            {
                return null;
            }

            nodeWidth = abstractGridSize.x;
            nodeHeight = abstractGridSize.y;
            var tilemapWidth = nodeWidth * tilemapSizePerNode.x;
            var tilemapHeight = nodeHeight * tilemapSizePerNode.y;
            var tilemap = new FlowTilemap(tilemapWidth, tilemapHeight);
            
            var tileNodes = new TilemapFlowNodeInfo[nodeWidth, nodeHeight];
            for (int ny = 0; ny < nodeHeight; ny++)
            {
                for (int nx = 0; nx <nodeWidth; nx++)
                {
                    var x0 = nx * tilemapSizePerNode.x;
                    var y0 = ny * tilemapSizePerNode.y;
                    var x1 = x0 + tilemapSizePerNode.x;
                    var y1 = y0 + tilemapSizePerNode.y;
                    var node = new TilemapFlowNodeInfo(x0, y0, x1, y1);
                    tileNodes[nx, ny] = node;
                }
            }
            foreach (var node in graph.Nodes)
            {
                var coord = GetNodeCoord(node);
                var tileNode = tileNodes[coord.x, coord.y];
                tileNode.node = node;
            }

            PerturbRoomSizes(graph, tileNodes, random);
            FixCorridorSizes(tileNodes, graph);

            RasterizeRoomCorridors(tileNodes, tilemap);
            RasterizeBaseCaveBlocks(tileNodes, tilemap, graph);

            GenerateMainPath(tileNodes, tilemap, graph);
            BuildCaves(tileNodes, tilemap, graph, random);
            BuildDoors(tileNodes, tilemap, graph);

            tilemap = CropTilemap(tilemap);

            CalculateDistanceFromMainPath(tileNodes, tilemap, new GridFlowLayoutNodeRoomType[] { GridFlowLayoutNodeRoomType.Cave });
            CalculateDistanceFromMainPath(tileNodes, tilemap, new GridFlowLayoutNodeRoomType[] {
                GridFlowLayoutNodeRoomType.Room,
                GridFlowLayoutNodeRoomType.Corridor
            });
            CalculateDistanceFromMainPathOnEmptyArea(tilemap);

            DebugPostProcess(tileNodes, tilemap, graph);

            return tilemap;
        }


        FlowTilemap CropTilemap(FlowTilemap oldTilemap)
        {
            if (oldTilemap.Width == 0 || oldTilemap.Height == 0)
            {
                return oldTilemap;
            }

            int x0 = 0;
            int x1 = 0;
            int y0 = 0;
            int y1 = 0;
            if (cropTilemap)
            {
                bool foundFirstCell = false;
                foreach (var cell in oldTilemap.Cells)
                {
                    bool layoutTile = cell.CellType == FlowTilemapCellType.Floor
                                      || cell.CellType == FlowTilemapCellType.Wall
                                      || cell.CellType == FlowTilemapCellType.Door;

                    if (layoutTile)
                    {
                        var x = cell.TileCoord.x;
                        var y = cell.TileCoord.y;
                        if (!foundFirstCell)
                        {
                            foundFirstCell = true;
                            x0 = x1 = x;
                            y0 = y1 = y;
                        }
                        else
                        {
                            x0 = Mathf.Min(x0, x);
                            x1 = Mathf.Max(x1, x);
                            y0 = Mathf.Min(y0, y);
                            y1 = Mathf.Max(y1, y);
                        }
                    }
                }
            }
            else
            {
                x1 = oldTilemap.Width - 1;
                y1 = oldTilemap.Height - 1;
            }

            var p = layoutPadding;
            var layoutWidth = x1 - x0 + 1;
            var layoutHeight = y1 - y0 + 1;
            var tilemap = new FlowTilemap(
                    layoutWidth + p * 2,
                    layoutHeight + p * 2);

            // clone the cells
            for (int y = 0; y < layoutHeight; y++)
            {
                for (int x = 0; x < layoutWidth; x++)
                {
                    var ix = x + p;
                    var iy = y + p;

                    tilemap.Cells[ix, iy] = oldTilemap.Cells[x + x0, y + y0].Clone();
                    tilemap.Cells[ix, iy].TileCoord = new IntVector2(ix, iy);
                }
            }
            // clone the edges
            for (int y = 0; y <= layoutHeight; y++)
            {
                for (int x = 0; x <= layoutWidth; x++)
                {
                    var ix = x + p;
                    var iy = y + p;

                    tilemap.Edges.SetHorizontal(ix, iy, oldTilemap.Edges.GetHorizontal(x + x0, y + y0).Clone());
                    tilemap.Edges.SetVertical(ix, iy, oldTilemap.Edges.GetVertical(x + x0, y + y0).Clone());

                    var edgeH = tilemap.Edges.GetHorizontal(ix, iy);
                    edgeH.EdgeCoord = new IntVector2(ix, iy);

                    var edgeV = tilemap.Edges.GetVertical(ix, iy);
                    edgeV.EdgeCoord = new IntVector2(ix, iy);
                }
            }

            return tilemap;
        }

        #region Room / Corridor Generation Functions
        void PerturbRoomSizes(FlowLayoutGraph graph, TilemapFlowNodeInfo[,] tileNodes, System.Random random)
        {
            // Perturb horizontally
            {
                //perturbAmount = Mathf.Min(perturbAmount, tilemapSizePerNode * 0.5f - corridorLaneWidth);
                //perturbAmount = Mathf.Max(0, perturbAmount);

                var maxPerturbAmount = Mathf.FloorToInt((tilemapSizePerNode.x - 2 - corridorLaneWidth) * 0.5f);
                maxPerturbAmount = Mathf.Max(0, maxPerturbAmount);

                var perturbX = Mathf.Min(perturbAmount.x, maxPerturbAmount);
                perturbX = Mathf.Max(0, perturbX);

                for (int ny = 0; ny < nodeHeight; ny++)
                {
                    for (int nx = -1; nx < nodeWidth; nx++)
                    {
                        var nodeA = (nx >= 0) ? tileNodes[nx, ny] : null;
                        var nodeB = (nx + 1 < nodeWidth) ? tileNodes[nx + 1, ny] : null;
                        
                        bool perturbA = true;
                        bool perturbB = true;

                        if (nodeA != null && nodeA.node != null)
                        {
                            var nodeStateA = nodeA.node.GetDomainData<GridFlowLayoutNodeState>();
                            perturbA = nodeStateA.CanPerturb;
                        }
                        
                        if (nodeB != null && nodeB.node != null)
                        {
                            var nodeStateB = nodeB.node.GetDomainData<GridFlowLayoutNodeState>();
                            perturbB = nodeStateB.CanPerturb;
                        }
                        
                        bool connected = false;
                        if (nodeA != null && nodeB != null)
                        {
                            var link = graph.GetLink(nodeA.node, nodeB.node, true);
                            connected = (link != null && link.state.type != FlowLayoutGraphLinkType.Unconnected);
                        }

                        if (connected)
                        {
                            if (perturbA && perturbB)
                            {
                                float amount = random.Range(-perturbX, perturbX);
                                nodeA.x1 += amount;
                                nodeB.x0 += amount;
                            }
                        }
                        else
                        {
                            
                            if (nodeA != null && perturbA)
                            {
                                var amount = perturbX * random.NextFloat();
                                nodeA.x1 -= amount;
                            }

                            if (nodeB != null && perturbB)
                            {
                                var amount = perturbX * random.NextFloat();
                                nodeB.x0 += amount;
                            }
                        }
                    }
                }
            }

            // Perturb vertically
            {
                var maxPerturbAmount = Mathf.FloorToInt((tilemapSizePerNode.y - 2 - corridorLaneWidth) * 0.5f);
                maxPerturbAmount = Mathf.Max(0, maxPerturbAmount);

                var perturbY = Mathf.Min(perturbAmount.y, maxPerturbAmount);
                perturbY = Mathf.Max(0, perturbY);

                for (int nx = 0; nx < nodeWidth; nx++)
                {
                    for (int ny = -1; ny < nodeHeight; ny++)
                    {
                        var nodeA = (ny >= 0) ? tileNodes[nx, ny] : null;
                        var nodeB = (ny + 1 < nodeHeight) ? tileNodes[nx, ny + 1] : null;

                        bool perturbA = true;
                        bool perturbB = true;

                        if (nodeA != null && nodeA.node != null)
                        {
                            var nodeStateA = nodeA.node.GetDomainData<GridFlowLayoutNodeState>();
                            perturbA = nodeStateA.CanPerturb;
                        }
                        
                        if (nodeB != null && nodeB.node != null)
                        {
                            var nodeStateB = nodeB.node.GetDomainData<GridFlowLayoutNodeState>();
                            perturbB = nodeStateB.CanPerturb;
                        }
                        
                        bool connected = false;
                        if (nodeA != null && nodeB != null)
                        {
                            var link = graph.GetLink(nodeA.node, nodeB.node, true);
                            connected = (link != null && link.state.type != FlowLayoutGraphLinkType.Unconnected);
                        }

                        if (connected)
                        {
                            if (perturbA && perturbB)
                            {
                                bool canMoveDown = (nodeA.x0 >= nodeB.x0 && nodeA.x1 <= nodeB.x1);
                                bool canMoveUp = (nodeB.x0 >= nodeA.x0 && nodeB.x1 <= nodeA.x1);

                                if (!canMoveUp && !canMoveDown) continue;

                                if (canMoveUp && canMoveDown)
                                {
                                    // Move randomly on either one direction
                                    if (random.NextFloat() < 0.5f)
                                    {
                                        canMoveUp = false;
                                    }
                                    else
                                    {
                                        canMoveDown = false;
                                    }
                                }

                                float perturbDirection = (canMoveUp ? -1 : 1);
                                float perturb = random.NextFloat() * perturbY * perturbDirection;
                                nodeA.y1 += perturb;
                                nodeB.y0 += perturb;
                            }
                        }
                        else
                        {
                            if (nodeA != null && perturbA)
                            {
                                float amountA = perturbY * random.NextFloat();
                                nodeA.y1 -= amountA;
                            }

                            if (nodeB != null && perturbB)
                            {
                                float amountB = perturbY * random.NextFloat();
                                nodeB.y0 += amountB;
                            }
                        }
                    }
                }
            }
        }

        GridFlowLayoutNodeRoomType GetRoomType(FlowLayoutGraphNode node)
        {
            var domainData = node.GetDomainData<GridFlowTilemapDomainData>();
            return domainData.RoomType;
        }
        
        void FixCorridorSizes(TilemapFlowNodeInfo[,] tileNodes, FlowLayoutGraph graph)
        {
            foreach (var tileNode in tileNodes)
            {
                var node = tileNode.node;
                if (GetRoomType(node) == GridFlowLayoutNodeRoomType.Corridor)
                {
                    var incomingNodes = graph.GetIncomingNodes(node);
                    var outgoingNodes = graph.GetOutgoingNodes(node);
                    if (incomingNodes.Length == 0 || outgoingNodes.Length == 0) continue;

                    var incomingNode = incomingNodes[0];
                    var outgoingNode = outgoingNodes[0];
                    var inCoord = GetNodeCoord(incomingNode);
                    var outCoord = GetNodeCoord(outgoingNode);
                    var vertical = inCoord.x == outCoord.x;
                    if (vertical)
                    {
                        tileNode.x0 = Mathf.Max(tileNode.x0, tileNode.midX - corridorLaneWidth);
                        tileNode.x1 = Mathf.Min(tileNode.x1, tileNode.midX + corridorLaneWidth);
                    }
                    else
                    {
                        tileNode.y0 = Mathf.Max(tileNode.y0, tileNode.midY - corridorLaneWidth);
                        tileNode.y1 = Mathf.Min(tileNode.y1, tileNode.midY + corridorLaneWidth);
                    }
                }
            }
        }
        void RasterizeRoomCorridors(TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap)
        {
            foreach (var tileNode in tileNodes)
            {
                if (!tileNode.node.active) continue;
                bool wallsAsTiles = wallGenerationMethod == TilemapFlowNodeWallGenerationMethod.WallAsTiles;
                var b = NodeTilemapBounds.Build(tileNode, tilemap.Width, tilemap.Height, wallsAsTiles);

                if (GetRoomType(tileNode.node) == GridFlowLayoutNodeRoomType.Cave)
                {
                    // Render the caves in another pass
                    continue;
                }

                var nodeCoord = GetNodeCoord(tileNode.node);
                // Rasterize the tiles
                for (int y = b.y0; y <= b.y1; y++)
                {
                    for (int x = b.x0; x <= b.x1; x++)
                    {
                        var cell = tilemap.Cells.GetCell(x, y);
                        if (cell == null) continue;
                        cell.LayoutCell = true;

                        cell.NodeCoord = nodeCoord;
                        if (wallsAsTiles)
                        {
                            cell.CellType = (x == b.x0 || x == b.x1 || y == b.y0 || y == b.y1)
                            ? FlowTilemapCellType.Wall
                            : FlowTilemapCellType.Floor;
                        }
                        else
                        {
                            if (x < b.x1 && y < b.y1)
                            {
                                cell.CellType = FlowTilemapCellType.Floor;
                            }
                        }

                        var nodeColor = tileNode.node.color;
                        cell.CustomColor = ColorUtils.BrightenColor(nodeColor, roomColorSaturation, roomColorBrightness);

                        if (cell.CellType == FlowTilemapCellType.Floor)
                        {
                            cell.UseCustomColor = true;
                        }

                        else if (cell.CellType == FlowTilemapCellType.Wall)
                        {
                            FlowTilemapCellWallInfo wallInfo = cell.Userdata as FlowTilemapCellWallInfo;
                            if (wallInfo == null)
                            {
                                wallInfo = new FlowTilemapCellWallInfo();
                                cell.Userdata = wallInfo;
                            }

                            wallInfo.owningNodes.Add(nodeCoord);
                        }
                    }
                }

                // Rasterize the edges
                if (!wallsAsTiles)
                {
                    for (int y = b.y0; y < b.y1; y++)
                    {
                        var edge1 = tilemap.Edges.GetVertical(b.x0, y);
                        var edge2 = tilemap.Edges.GetVertical(b.x1, y);
                        edge1.EdgeType = FlowTilemapEdgeType.Wall;
                        edge2.EdgeType = FlowTilemapEdgeType.Wall;
                    }
                    for (int x = b.x0; x < b.x1; x++)
                    {
                        var edge1 = tilemap.Edges.GetHorizontal(x, b.y0);
                        var edge2 = tilemap.Edges.GetHorizontal(x, b.y1);
                        edge1.EdgeType = FlowTilemapEdgeType.Wall;
                        edge2.EdgeType = FlowTilemapEdgeType.Wall;
                    }
                }
            }
        }
        #endregion

        #region Cave Generation Functions
        private void RasterizeBaseCaveBlocks(TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap, FlowLayoutGraph graph)
        {
            foreach (var tileNode in tileNodes)
            {
                if (!tileNode.node.active) continue;
                bool wallsAsTiles = wallGenerationMethod == TilemapFlowNodeWallGenerationMethod.WallAsTiles;
                var b = NodeTilemapBounds.Build(tileNode, tilemap.Width, tilemap.Height, wallsAsTiles);

                if (GetRoomType(tileNode.node) != GridFlowLayoutNodeRoomType.Cave)
                {
                    // Only build the caves in this pass
                    continue;
                }

                var nodeCoord = GetNodeCoord(tileNode.node);
                var caveNode = tileNode.node;
                var blockLeft = ShouldBlockCaveBoundary(graph, caveNode, -1, 0);
                var blockRight = ShouldBlockCaveBoundary(graph, caveNode, 1, 0);
                var blockTop = ShouldBlockCaveBoundary(graph, caveNode, 0, -1);
                var blockBottom = ShouldBlockCaveBoundary(graph, caveNode, 0, 1);
                for (int y = b.y0; y <= b.y1; y++)
                {
                    for (int x = b.x0; x <= b.x1; x++)
                    {
                        var cell = tilemap.Cells.GetCell(x, y);
                        if (cell == null) continue;

                        if (cell.CellType == FlowTilemapCellType.Empty)
                        {
                            var makeFloor = true;
                            if (wallsAsTiles)
                            {
                                if (blockLeft && x == b.x0) makeFloor = false;
                                if (blockRight && x == b.x1) makeFloor = false;
                                if (blockTop && y == b.y0) makeFloor = false;
                                if (blockBottom && y == b.y1) makeFloor = false;
                            }
                            else
                            {
                                if (blockRight && x == b.x1) makeFloor = false;
                                if (blockBottom && y == b.y1) makeFloor = false;

                                if (blockLeft && x == b.x0 && y == b.y0) makeFloor = false;
                                if (blockLeft && x == b.x0 && y == b.y1) makeFloor = false;
                                
                                if (blockTop && x == b.x0 && y == b.y1) makeFloor = false;
                                if (blockTop && x == b.x1 && y == b.y1) makeFloor = false;
                            }

                            if (makeFloor)
                            {
                                cell.NodeCoord = nodeCoord;
                                cell.CellType = FlowTilemapCellType.Floor;
                                cell.UseCustomColor = true;
                                var nodeColor = tileNode.node.color;
                                cell.CustomColor = ColorUtils.BrightenColor(nodeColor, roomColorSaturation, roomColorBrightness);
                            }
                        }
                    }
                }
            }
        }
        private void BuildCaves(TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap, FlowLayoutGraph graph, System.Random random)
        {
            CalculateDistanceFromMainPath(tileNodes, tilemap, new GridFlowLayoutNodeRoomType[] { GridFlowLayoutNodeRoomType.Cave });
            var caveMap = GenerateCaveBuildMap(tileNodes, tilemap, graph);
            BuildCaveStep_BuildRocks(caveMap, tilemap, random);
            BuildCaveStep_SimulateGrowth(caveMap, tilemap, random);
            BuildCaveStep_Cleanup(caveMap, tileNodes, tilemap);
            BuildCaveStep_UpdateEdges(caveMap, tileNodes, tilemap);
        }
        private CaveCellBuildTile[,] GenerateCaveBuildMap(TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap, FlowLayoutGraph graph)
        {
            var caveMap = new CaveCellBuildTile[tilemap.Width, tilemap.Height];

            foreach (var cell in tilemap.Cells)
            {
                var caveTile = new CaveCellBuildTile();
                caveTile.tileCoord = cell.TileCoord;
                var tileNode = tileNodes[cell.NodeCoord.x, cell.NodeCoord.y];
                caveTile.valid = (GetRoomType(tileNode.node) == GridFlowLayoutNodeRoomType.Cave && tileNode.node.active && cell.CellType == FlowTilemapCellType.Floor);
                caveMap[cell.TileCoord.x, cell.TileCoord.y] = caveTile;
            }

            return caveMap;
        }
        private void BuildCaveStep_BuildRocks(CaveCellBuildTile[,] caveMap, FlowTilemap tilemap, System.Random random)
        {
            foreach (var caveCell in caveMap)
            {
                var tileCell = tilemap.Cells[caveCell.tileCoord.x, caveCell.tileCoord.y];
                if (caveThickness > 0)
                {
                    var rockProbability = Mathf.Exp(-tileCell.DistanceFromMainPath / caveThickness);
                    caveCell.rockTile = random.NextFloat() < rockProbability;
                }
                else
                {
                    caveCell.rockTile = (tileCell.DistanceFromMainPath == 0);
                }
            }
        }
        private void BuildCaveStep_SimulateGrowth(CaveCellBuildTile[,] caveMap, FlowTilemap tilemap, System.Random random)
        {
            var width = caveMap.GetLength(0);
            var height = caveMap.GetLength(1);

            for (int i = 0; i < caveAutomataIterations; i++)
            {
                CaveCellBuildTile[,] oldMap = new CaveCellBuildTile[width, height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        oldMap[x, y] = caveMap[x, y].Clone();
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int nrocks = 0;
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                if (dx == 0 && dy == 0) continue;
                                int nx = x + dx;
                                int ny = y + dy;
                                if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                                if (oldMap[nx, ny].rockTile)
                                {
                                    nrocks++;
                                }
                            }
                        }


                        if (nrocks >= caveAutomataNeighbors)
                        {
                            caveMap[x, y].rockTile = true;
                        }
                    }
                }

            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (caveMap[x, y].valid && !caveMap[x, y].rockTile)
                    {
                        tilemap.Cells[x, y].CellType = FlowTilemapCellType.Empty;
                        tilemap.Cells[x, y].UseCustomColor = false;
                        caveMap[x, y].valid = false;
                    }
                }
            }
        }
        private void BuildCaveStep_Cleanup(CaveCellBuildTile[,] caveMap, TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap)
        {
            var width = tilemap.Width;
            var height = tilemap.Height;
            var traversibleCaveTiles = new bool[width, height];

            var childOffsets = new int[]
            {
                -1, 0,
                1, 0,
                0, -1,
                0, 1
            };

            foreach (var tileNode in tileNodes)
            {
                if (GetRoomType(tileNode.node) != GridFlowLayoutNodeRoomType.Cave)
                {
                    // Only process the caves
                    continue;
                }

                if (!tileNode.node.active)
                {
                    // Do not process inactive nodes
                    continue;
                }

                var nodeCoord = GetNodeCoord(tileNode.node);
                var tileCenter = NodeCoordToTileCoord(nodeCoord);

                if (traversibleCaveTiles[tileCenter.x, tileCenter.y])
                {
                    // Already processed from another adjacent node
                    continue;
                }

                // Flood fill from the center of this node
                var queue = new Queue<IntVector2>();
                queue.Enqueue(tileCenter);
                while (queue.Count > 0)
                {
                    var front = queue.Dequeue();
                    if (traversibleCaveTiles[front.x, front.y])
                    {
                        // Already processed
                        continue;
                    }

                    traversibleCaveTiles[front.x, front.y] = true;

                    // Traverse the children
                    for (int i = 0; i < 4; i++)
                    {
                        var childCoord = new IntVector2(
                            front.x + childOffsets[i * 2 + 0],
                            front.y + childOffsets[i * 2 + 1]);

                        if (childCoord.x >= 0 && childCoord.y >= 0 && childCoord.x < width && childCoord.y < height)
                        {
                            if (caveMap[childCoord.x, childCoord.y].valid)
                            {
                                var visited = traversibleCaveTiles[childCoord.x, childCoord.y];
                                if (!visited)
                                {
                                    queue.Enqueue(childCoord);
                                }
                            }
                        }
                    }
                }
            }

            // Assign the valid traversable paths 
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var cell = tilemap.Cells[x, y];
                    var nodeCoord = cell.NodeCoord;
                    var tileNode = tileNodes[nodeCoord.x, nodeCoord.y];
                    if (GetRoomType(tileNode.node) == GridFlowLayoutNodeRoomType.Cave)
                    {
                        var valid = traversibleCaveTiles[x, y];
                        caveMap[x, y].valid = valid;
                        if (!valid)
                        {
                            tilemap.Cells[x, y].CellType = FlowTilemapCellType.Empty;
                            tilemap.Cells[x, y].UseCustomColor = false;
                        }
                        else
                        {
                            tilemap.Cells[x, y].LayoutCell = true;
                        }
                    }
                }
            }
        }

        private bool CanCreateEdgeToAdjacentCaveTile(FlowTilemap tilemap, CaveCellBuildTile[,] caveMap, int x, int y)
        {
            if (x < 0 || x >= tilemap.Width || y < 0 || y >= tilemap.Height) return true;   // this is an empty tile

            if (tilemap.Cells[x, y].CellType == FlowTilemapCellType.Empty
                || tilemap.Cells[x, y].CellType == FlowTilemapCellType.Custom)
            {
                return true;
            }

            bool caveTile = caveMap[x, y].valid && caveMap[x, y].rockTile;
            if (caveTile)
            {
                // no need for an edge between two cave tiles
                return false;
            }

            // This tile is not empty and is not a cave tile
            return false;
        }

        private void BuildCaveStep_UpdateEdges(CaveCellBuildTile[,] caveMap, TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap)
        {
            var width = tilemap.Width;
            var height = tilemap.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool caveTile = x < width && y < height && caveMap[x, y].valid && caveMap[x, y].rockTile;
                    if (!caveTile) continue;

                    bool createEdgeLeft = CanCreateEdgeToAdjacentCaveTile(tilemap, caveMap, x - 1, y);
                    bool createEdgeRight = CanCreateEdgeToAdjacentCaveTile(tilemap, caveMap, x + 1, y);

                    bool createEdgeBottom = CanCreateEdgeToAdjacentCaveTile(tilemap, caveMap, x, y - 1);
                    bool createEdgeTop = CanCreateEdgeToAdjacentCaveTile(tilemap, caveMap, x, y + 1);

                    if (createEdgeLeft)
                    {
                        var edgeV = tilemap.Edges.GetVertical(x, y);
                        if (edgeV.EdgeType == FlowTilemapEdgeType.Empty)
                        {
                            edgeV.EdgeType = FlowTilemapEdgeType.Fence;
                        }
                    }

                    if (createEdgeRight)
                    {
                        var edgeV = tilemap.Edges.GetVertical(x + 1, y);
                        if (edgeV.EdgeType == FlowTilemapEdgeType.Empty)
                        {
                            edgeV.EdgeType = FlowTilemapEdgeType.Fence;
                        }
                    }

                    if (createEdgeBottom)
                    {
                        var edgeH = tilemap.Edges.GetHorizontal(x, y);
                        if (edgeH.EdgeType == FlowTilemapEdgeType.Empty)
                        {
                            edgeH.EdgeType = FlowTilemapEdgeType.Fence;
                        }
                    }

                    if (createEdgeTop)
                    {
                        var edgeH = tilemap.Edges.GetHorizontal(x, y + 1);
                        if (edgeH.EdgeType == FlowTilemapEdgeType.Empty)
                        {
                            edgeH.EdgeType = FlowTilemapEdgeType.Fence;
                        }
                    }
                }
            }

        }
        
        private void DebugPostProcess(TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap, FlowLayoutGraph graph)
        {
            bool debugMainPathDistance = false;
            var width = tilemap.Width;
            var height = tilemap.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tileCell = tilemap.Cells[x, y];
                    var tileNode = tileNodes[tileCell.NodeCoord.x, tileCell.NodeCoord.y];
                    var roomType = GetRoomType(tileNode.node);

                    if (debugMainPathDistance)
                    {
                        var startColor = new Color(1.0f, 0.4f, 0.4f);
                        var endColor = new Color(0.25f, 0.1f, 0.1f);
                        if (tileCell.CellType != FlowTilemapCellType.Empty)
                        {
                            if (roomType == GridFlowLayoutNodeRoomType.Cave)
                            {
                                startColor = new Color(0.4f, 0.4f, 1.0f);
                                endColor = new Color(0.1f, 0.1f, 0.25f);
                            }
                            else if (roomType == GridFlowLayoutNodeRoomType.Room || roomType == GridFlowLayoutNodeRoomType.Corridor)
                            {
                                startColor = new Color(0.4f, 1.0f, 0.4f);
                                endColor = new Color(0.1f, 0.25f, 0.1f);
                            }
                        }

                        var distanceFactor = Mathf.Exp(-tileCell.DistanceFromMainPath / 5.0f);
                        var debugColor = Color.Lerp(endColor, startColor, distanceFactor);
                        tileCell.CustomColor = debugColor;
                        tileCell.UseCustomColor = true;
                    }
                }
            }
        }
        bool ShouldBlockCaveBoundary(FlowLayoutGraph graph, FlowLayoutGraphNode caveNode, int dx, int dy)
        {
            var coord = GetNodeCoord(caveNode);
            var otherCoord = coord + new IntVector2(dx, dy);
            FlowLayoutGraphNode otherNode = null;
            foreach (var node in graph.Nodes)
            {
                var nodeCoord = GetNodeCoord(node);
                if (nodeCoord.Equals(otherCoord))
                {
                    otherNode = node;
                    break;
                }
            }
            if (otherNode == null || !otherNode.active)
            {
                // a node in this location doesn't exist
                return false;
            }

            // Check if we have a link between these nodes. If we don't, then block it
            var link = graph.GetLink(caveNode, otherNode);
            if (link == null)
            {
                // No link exists. we should block this
                return true;
            }

            // We have a link to the other node.   block only if they it is a non-cave nodes
            return GetRoomType(otherNode) != GridFlowLayoutNodeRoomType.Cave;
        }
        #endregion

        #region Common Functions
        private void GenerateMainPath(TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap, FlowLayoutGraph graph)
        {
            foreach (var link in graph.Links)
            {
                var nodeA = graph.GetNode(link.source);
                var nodeB = graph.GetNode(link.destination);

                var tileCenterA = NodeCoordToTileCoord(GetNodeCoord(nodeA));
                var tileCenterB = NodeCoordToTileCoord(GetNodeCoord(nodeB));
                if (tileCenterA.x == tileCenterB.x)
                {
                    var x = tileCenterA.x;
                    int y0 = Mathf.Min(tileCenterA.y, tileCenterB.y);
                    int y1 = Mathf.Max(tileCenterA.y, tileCenterB.y);
                    for (int y = y0; y <= y1; y++)
                    {
                        var cell = tilemap.Cells[x, y];
                        cell.MainPath = true;
                        cell.DistanceFromMainPath = 0;
                    }
                }
                else if (tileCenterA.y == tileCenterB.y)
                {
                    var y = tileCenterA.y;
                    int x0 = Mathf.Min(tileCenterA.x, tileCenterB.x);
                    int x1 = Mathf.Max(tileCenterA.x, tileCenterB.x);
                    for (int x = x0; x <= x1; x++)
                    {
                        var cell = tilemap.Cells[x, y];
                        cell.MainPath = true;
                        cell.DistanceFromMainPath = 0;
                    }
                }
                else
                {
                    Debug.Log("invalid input");
                }
            }

            if (graph.Links.Count == 0 && graph.Nodes.Count == 1)
            {
                var node = graph.Nodes[0];
                var tc = NodeCoordToTileCoord(GetNodeCoord(node));
                tilemap.Cells[tc.x, tc.y].MainPath = true;
                tilemap.Cells[tc.x, tc.y].DistanceFromMainPath = 0;

            }
        }
        private void CalculateDistanceFromMainPathOnEmptyArea(FlowTilemap tilemap)
        {
            var width = tilemap.Width;
            var height = tilemap.Height;
            var queue = new Queue<FlowTilemapCell>();

            var childOffsets = new int[]
            {
                -1, 0,
                1, 0,
                0, -1,
                0, 1
            };

            foreach (var cell in tilemap.Cells)
            {
                if (cell.CellType != FlowTilemapCellType.Empty)
                {
                    continue;
                }

                var validStartNode = false;

                for (int i = 0; i < 4; i++)
                {
                    int nx = cell.TileCoord.x + childOffsets[i * 2 + 0];
                    int ny = cell.TileCoord.y + childOffsets[i * 2 + 1];
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        var ncell = tilemap.Cells[nx, ny];
                        if (ncell.CellType != FlowTilemapCellType.Empty)
                        {
                            validStartNode = true;
                            cell.DistanceFromMainPath = Mathf.Min(cell.DistanceFromMainPath, ncell.DistanceFromMainPath);
                        }
                    }
                }

                if (validStartNode)
                {
                    queue.Enqueue(cell);
                }
            }


            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                var ndist = cell.DistanceFromMainPath + 1;

                for (int i = 0; i < 4; i++)
                {
                    int nx = cell.TileCoord.x + childOffsets[i * 2 + 0];
                    int ny = cell.TileCoord.y + childOffsets[i * 2 + 1];
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        var ncell = tilemap.Cells[nx, ny];

                        if (ncell.CellType == FlowTilemapCellType.Empty)
                        {
                            if (ndist < ncell.DistanceFromMainPath)
                            {
                                ncell.DistanceFromMainPath = ndist;
                                queue.Enqueue(ncell);
                                
                            }
                        }
                    }
                }
            }
        }
        private void CalculateDistanceFromMainPath(TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap, GridFlowLayoutNodeRoomType[] allowedRoomTypes)
        {
            var width = tilemap.Width;
            var height = tilemap.Height;
            var queue = new Queue<FlowTilemapCell>();

            foreach (var cell in tilemap.Cells)
            {
                var tileNode = tileNodes[cell.NodeCoord.x, cell.NodeCoord.y];
                var roomType = GetRoomType(tileNode.node);
                if (!allowedRoomTypes.Contains(roomType))
                {
                    continue;
                }

                if (cell.MainPath)
                {
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
                var tile = queue.Dequeue();

                // Traverse the children
                var childDistance = tile.DistanceFromMainPath + 1;
                for (int i = 0; i < 4; i++)
                {
                    int nx = tile.TileCoord.x + childOffsets[i * 2 + 0];
                    int ny = tile.TileCoord.y + childOffsets[i * 2 + 1];
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        var ncell = tilemap.Cells[nx, ny];
                        var ntileNode = tileNodes[ncell.NodeCoord.x, ncell.NodeCoord.y];
                        var nroomType = GetRoomType(ntileNode.node);
                        if (!allowedRoomTypes.Contains(nroomType))
                        {
                            continue;
                        }

                        if (childDistance < ncell.DistanceFromMainPath)
                        {
                            ncell.DistanceFromMainPath = childDistance;
                            queue.Enqueue(ncell);
                        }
                    }
                }
            }
        }

        struct DoorInfo
        {
            public FlowLayoutGraphLink Link { get; set; }
            public IntVector2 CellCoord { get; set; }
            public bool HorizontalDoorEdge { get; set; }
        }

        void BuildDoors(TilemapFlowNodeInfo[,] tileNodes, FlowTilemap tilemap, FlowLayoutGraph graph)
        {
            bool wallsAsTiles = (wallGenerationMethod == TilemapFlowNodeWallGenerationMethod.WallAsTiles);

            // Build the doors
            var doorList = new List<DoorInfo>();
            foreach (var tileNode in tileNodes)
            {
                if (!tileNode.node.active) continue;
                var b = NodeTilemapBounds.Build(tileNode, tilemap.Width, tilemap.Height, wallsAsTiles);

                var node = tileNode.node;
                var nodeCoord = GetNodeCoord(node);
                foreach (var link in graph.GetOutgoingLinks(tileNode.node))
                {
                    if (link.state.type == FlowLayoutGraphLinkType.Unconnected) continue;

                    var otherNode = graph.GetNode(link.destination);
                    if (GetRoomType(node) == GridFlowLayoutNodeRoomType.Cave && GetRoomType(otherNode) == GridFlowLayoutNodeRoomType.Cave)
                    {
                        // We don't need a door between two cave nodes
                        continue;
                    }

                    bool foundDoor = false;
                    var doorCoord = new IntVector2(0, 0);
                    bool horizontalEdge = true;

                    var otherCoord = GetNodeCoord(otherNode);
                    if (nodeCoord.x == otherCoord.x)
                    {
                        // Vertical link
                        var y = (nodeCoord.y < otherCoord.y) ? b.y1 : b.y0;
                        foundDoor = true;
                        doorCoord = new IntVector2(b.mx, y);
                        horizontalEdge = true;
                    }
                    else if (nodeCoord.y == otherCoord.y)
                    {
                        // Horizontal link
                        var x = (nodeCoord.x < otherCoord.x) ? b.x1 : b.x0;
                        foundDoor = true;
                        doorCoord = new IntVector2(x, b.my);
                        horizontalEdge = false;
                    }

                    if (foundDoor)
                    {
                        var doorMeta = new FlowTilemapCellDoorInfo();
                        {
                            var sourceNode = graph.GetNode(link.source);
                            var destNode = graph.GetNode(link.destination);
                            doorMeta.oneWay = (link.state.type == FlowLayoutGraphLinkType.OneWay);
                            doorMeta.nodeA = GetNodeCoord(sourceNode);
                            doorMeta.nodeB = GetNodeCoord(destNode);
                            int numLockedItems = link.state.items.Count(i => i.type == FlowGraphItemType.Lock);
                            doorMeta.locked = numLockedItems > 0;
                        }

                        if (wallsAsTiles)
                        {
                            FlowTilemapCell doorCell = tilemap.Cells.GetCell(doorCoord.x, doorCoord.y);
                            doorCell.Userdata = doorMeta;
                            doorCell.CellType = FlowTilemapCellType.Door;
                        }
                        else
                        {
                            FlowTilemapEdge doorEdge = horizontalEdge
                                ? tilemap.Edges.GetHorizontal(doorCoord.x, doorCoord.y)
                                : tilemap.Edges.GetVertical(doorCoord.x, doorCoord.y);
                            doorEdge.Userdata = doorMeta;
                            doorEdge.EdgeType = FlowTilemapEdgeType.Door;
                        }

                        var doorInfo = new DoorInfo();
                        doorInfo.Link = link;
                        doorInfo.CellCoord = doorCoord;
                        doorInfo.HorizontalDoorEdge = horizontalEdge;
                        doorList.Add(doorInfo);
                    }
                }
            }

            // Add door items
            foreach (var door in doorList)
            {
                var items = door.Link.state.items;
                var doorItem = items.Count > 0 ? items[0] : null;
                if (doorItem == null) continue;

                if (wallsAsTiles)
                {
                    var cellCoord = door.CellCoord;
                    var cell = tilemap.Cells[cellCoord.x, cellCoord.y];
                    cell.Item = doorItem.itemId;
                }
                else
                {
                    var doorCoord = door.CellCoord;
                    FlowTilemapEdge doorEdge = door.HorizontalDoorEdge
                        ? tilemap.Edges.GetHorizontal(doorCoord.x, doorCoord.y)
                        : tilemap.Edges.GetVertical(doorCoord.x, doorCoord.y);
                    if (doorEdge != null)
                    {
                        doorEdge.Item = doorItem.itemId;
                    }
                }
            }
        }
        #endregion

        #region Utility Functions

        IntVector2 GetNodeCoord(FlowLayoutGraphNode node)
        {
            var coordF = node.coord;
            return new IntVector2(Mathf.RoundToInt(coordF.x), Mathf.RoundToInt(coordF.y));
        }
        
        private IntVector2 NodeCoordToTileCoord(IntVector2 nodeCoord)
        {
            var itilemapSizePerNode = new IntVector2(tilemapSizePerNode.x, tilemapSizePerNode.y);
            var tileCoord = nodeCoord * itilemapSizePerNode;
            tileCoord += itilemapSizePerNode / 2;
            return tileCoord;
        }
        bool GetGraphSize(FlowLayoutGraph graph, out IntVector2 size)
        {
            if (graph.Nodes.Count == 0)
            {
                size = IntVector2.Zero;
                return false;
            }

            int width = -int.MaxValue;
            int height = -int.MaxValue;

            foreach (var node in graph.Nodes)
            {
                var coord = GetNodeCoord(node);
                width = Mathf.Max(width, coord.x);
                height = Mathf.Max(height, coord.y);
            }

            size = new IntVector2(width + 1, height + 1);
            return true;
        }
        FlowTilemapCellType GetCellType(FlowTilemap tilemap, int x, int y)
        {
            if (x < 0 || y < 0 || x >= tilemap.Width || y >= tilemap.Height)
            {
                return FlowTilemapCellType.Empty;
            }
            return tilemap.Cells[x, y].CellType;
        }
        #endregion
    }
    
    
    #region Data structures

    struct NodeTilemapBounds
    {
        public int x0, y0, x1, y1, mx, my;
        public static NodeTilemapBounds Build(TilemapFlowNodeInfo tileNode, int tilemapWidth, int tilemapHeight, bool clampToBoundary)
        {
            var b = new NodeTilemapBounds();
            b.x0 = Mathf.FloorToInt(tileNode.x0);
            b.y0 = Mathf.FloorToInt(tileNode.y0);
            b.x1 = Mathf.FloorToInt(tileNode.x1);
            b.y1 = Mathf.FloorToInt(tileNode.y1);
            b.mx = Mathf.FloorToInt(tileNode.midX);
            b.my = Mathf.FloorToInt(tileNode.midY);

            if (clampToBoundary)
            {
                b.x0 = Mathf.Clamp(b.x0, 0, tilemapWidth - 1);
                b.x1 = Mathf.Clamp(b.x1, 0, tilemapWidth - 1);
                b.y0 = Mathf.Clamp(b.y0, 0, tilemapHeight - 1);
                b.y1 = Mathf.Clamp(b.y1, 0, tilemapHeight - 1);
                b.mx = Mathf.Clamp(b.mx, 0, tilemapWidth - 1);
                b.my = Mathf.Clamp(b.my, 0, tilemapHeight - 1);
            }

            return b;
        }
    }

    class CaveCellBuildTile
    {
        public IntVector2 tileCoord;
        public bool valid = false;
        public bool rockTile = false;

        public CaveCellBuildTile Clone()
        {
            var tile = new CaveCellBuildTile();
            tile.tileCoord = tileCoord;
            tile.valid = valid;
            tile.rockTile = rockTile;
            return tile;
        }
    }

    public class TilemapFlowNodeInfo
    {
        public TilemapFlowNodeInfo(float x0, float y0, float x1, float y1)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;

            midX = (x0 + x1) * 0.5f;
            midY = (y0 + y1) * 0.5f;
        }

        public float x0;
        public float x1;
        public float y0;
        public float y1;

        public float midX;
        public float midY;
        public FlowLayoutGraphNode node;
    }
    #endregion
}
