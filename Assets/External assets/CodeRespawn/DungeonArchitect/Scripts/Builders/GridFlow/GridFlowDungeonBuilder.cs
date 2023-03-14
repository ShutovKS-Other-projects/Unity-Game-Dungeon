//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DungeonArchitect.Utils;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Domains.Tilemap.Tasks;
using DungeonArchitect.Flow.Impl.GridFlow;
using DungeonArchitect.Flow.Items;

namespace DungeonArchitect.Builders.GridFlow
{
    public class GridFlowDungeonBuilder : DungeonBuilder
    {
        private GridFlowDungeonConfig gridFlowConfig;
        private GridFlowDungeonModel gridFlowModel;
        protected FlowExecNodeOutputRegistry execNodeOutputRegistry = null;
        public FlowExecNodeOutputRegistry ExecNodeOutputRegistry
        {
            get
            {
                return execNodeOutputRegistry;
            }
        }

        public override void BuildDungeon(DungeonConfig config, DungeonModel model)
        {
            gridFlowConfig = config as GridFlowDungeonConfig;
            gridFlowModel = model as GridFlowDungeonModel;

            if (gridFlowConfig.flowAsset == null)
            {
                Debug.LogError("Missing grid flow asset");
                return;
            }

            base.BuildDungeon(config, model);

            GenerateLevelLayout();

            var minimap = GetComponent<GridFlowMinimap>();
            if (minimap != null && minimap.initMode == GridFlowMinimapInitMode.OnDungeonRebuild)
            {
                minimap.Initialize();
            }
        }

        public override void EmitMarkers()
        {
            base.EmitMarkers();

            EmitLevelMarkers();

            ProcessMarkerOverrideVolumes();
        }

        void GenerateLevelLayout()
        {
            if (gridFlowConfig == null || gridFlowModel == null || gridFlowConfig.flowAsset == null)
            {
                return;
            }

            gridFlowModel.Reset();

            var execGraph = gridFlowConfig.flowAsset.execGraph;
            var random = new System.Random((int)gridFlowConfig.Seed);
            var domainExtensions = new FlowDomainExtensions();

            FlowExecutor executor = new FlowExecutor();
            if (!executor.Execute(execGraph, random, domainExtensions,gridFlowConfig.numGraphRetries, out execNodeOutputRegistry))
            {
                Debug.LogError("Failed to generate level layout. Please check your grid flow graph. Alternatively, increase the 'Num Graph Retries' parameter in the config");
            }
            else
            {
                var resultNode = execGraph.resultNode;
                var execState = execNodeOutputRegistry.Get(resultNode.Id);

                var layoutGraph = execState.State.GetState<FlowLayoutGraph>();
                var tilemap = execState.State.GetState<FlowTilemap>();
                
                var tilemapState = execState.State.GetState<GridFlowTilemapState>();
                if (tilemapState != null)
                {
                    gridFlowModel.wallsAsEdges = (tilemapState.WallGenerationMethod == TilemapFlowNodeWallGenerationMethod.WallAsEdges);
                }

                if (layoutGraph == null || tilemap == null)
                {
                    Debug.Log("Failed to generate grid flow tilemap");
                    return;
                }
                
                gridFlowModel.Initialize(layoutGraph, tilemap, gridFlowConfig.gridSize);
            }
        }

        bool IsCellOfType(FlowTilemap tilemap, int x, int y, FlowTilemapCellType[] types)
        {
            var cell = tilemap.Cells.GetCell(x, y);
            if (cell == null) return false;
            return types.Contains(cell.CellType);
        }

        class LayoutGraphLookup
        {
            private Dictionary<IntVector2, FlowLayoutGraphNode> nodesByCoord = new Dictionary<IntVector2, FlowLayoutGraphNode>();
            public LayoutGraphLookup(FlowLayoutGraph layoutGraph)
            {
                foreach (var node in layoutGraph.Nodes)
                {
                    if (node.active)
                    {
                        var coord = new IntVector2(
                            Mathf.RoundToInt(node.coord.x),
                            Mathf.RoundToInt(node.coord.y));
                        nodesByCoord[coord] = node;
                    }
                }
            }

            public FlowLayoutGraphNode GetNode(IntVector2 coord)
            {
                return nodesByCoord.ContainsKey(coord) ? nodesByCoord[coord] : null;
            }
        }
        
        GridFlowLayoutNodeRoomType GetCellRoomType(FlowTilemap tilemap, int x, int y, LayoutGraphLookup layoutGraphLookup)
        {
            var cell = tilemap.Cells.GetCell(x, y);
            if (cell == null)
            {
                return GridFlowLayoutNodeRoomType.Unknown;
            }

            var layoutNode = layoutGraphLookup.GetNode(cell.NodeCoord);
            if (layoutNode == null)
            {
                return GridFlowLayoutNodeRoomType.Unknown;
            }
            
            var domainData = layoutNode.GetDomainData<GridFlowTilemapDomainData>();
            return domainData.RoomType;
        }

        Quaternion GetBaseTransform(FlowTilemap tilemap, int x, int y)
        {
            var cellTypesToTransform = new FlowTilemapCellType[]
            {
                FlowTilemapCellType.Wall,
                FlowTilemapCellType.Door
            };

            var cell = tilemap.Cells[x, y];
            if (!cellTypesToTransform.Contains(cell.CellType))
            {
                return Quaternion.identity;
            }

            var validL = IsCellOfType(tilemap, x - 1, y, cellTypesToTransform);
            var validR = IsCellOfType(tilemap, x + 1, y, cellTypesToTransform);
            var validB = IsCellOfType(tilemap, x, y - 1, cellTypesToTransform);
            var validT = IsCellOfType(tilemap, x, y + 1, cellTypesToTransform);

            var angleY = 0;
            if (validL && validR)
            {
                angleY = validT ? 180 : 0;
            }
            else if (validT && validB)
            {
                angleY = validR ? 270 : 90;
            }
            else if (validL && validT) angleY = 180;
            else if (validL && validB) angleY = 90;
            else if (validR && validT) angleY = 270;
            else if (validR && validB) angleY = 0;

            return Quaternion.Euler(0, angleY, 0);
        }

        string GetEdgeMarkerName(FlowTilemapEdgeType edgeType)
        {
            if (edgeType == FlowTilemapEdgeType.Wall) return GridFlowDungeonMarkerNames.Wall;
            else if (edgeType == FlowTilemapEdgeType.Fence) return GridFlowDungeonMarkerNames.Fence;
            else if (edgeType == FlowTilemapEdgeType.Door)
            {
                return GridFlowDungeonMarkerNames.Door;
            }
            else
            {
                return "[Empty]";
            }
        }

        bool CreateLockItemMetadata(FlowItem item, ref string doorMarker, out FlowItemMetadata lockItemData)
        {
            if (item != null && item.type == FlowGraphItemType.Lock)
            {
                // Turn this into a locked door (lock marker will be spawned instead of a door (or a one way door)
                doorMarker = item.markerName;

                lockItemData = new FlowItemMetadata();
                lockItemData.itemId = item.itemId;
                lockItemData.itemType = item.type;
                lockItemData.referencedItems = new List<DungeonUID>(item.referencedItemIds).ToArray();
                return true;
            }
            else
            {
                lockItemData = null;
                return false;
            }
        }

        bool ChunkSupportsWalls(GridFlowLayoutNodeRoomType type)
        {
            return type == GridFlowLayoutNodeRoomType.Room || type == GridFlowLayoutNodeRoomType.Corridor;
        }
        
        
        void EmitLevelMarkers()
        {
            if (gridFlowConfig == null || gridFlowModel == null || gridFlowModel.LayoutGraph == null)
            {
                Debug.LogError("GridFlowBuilder: Invalid state");
                return;
            }

            var items = gridFlowModel.LayoutGraph.GetAllItems();
            var itemMap = new Dictionary<DungeonUID, FlowItem>();
            foreach (var item in items)
            {
                itemMap[item.itemId] = item;
            }

            var tilemap = gridFlowModel.Tilemap;
            if (tilemap == null)
            {
                return;
            }

            var basePosition = transform.position;
            var gridSize = gridFlowConfig.gridSize;
            // Emit the cell markers
            for (int x = 0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    var position = basePosition + Vector3.Scale(new Vector3(x + 0.5f, 0, y + 0.5f), gridSize);
                    var baseRotation = GetBaseTransform(tilemap, x, y);
                    var markerTransform = Matrix4x4.TRS(position, baseRotation, Vector3.one);
                    var cell = tilemap.Cells[x, y];
                    int cellId = tilemap.Width * y + x;

                    if (cell.Item != DungeonUID.Empty && itemMap.ContainsKey(cell.Item))
                    {
                        var item = itemMap[cell.Item];
                        if (item.markerName != null && item.markerName.Length > 0 && item.type != FlowGraphItemType.Lock)
                        {
                            // Emit this item
                            var itemData = new FlowItemMetadata();
                            itemData.itemId = item.itemId;
                            itemData.itemType = item.type;
                            itemData.referencedItems = new List<DungeonUID>(item.referencedItemIds).ToArray();

                            EmitMarker(item.markerName, markerTransform, new IntVector(x, 0, y), cellId, itemData);
                        }
                    }

                    bool removeElevationMarker = false;
                    if (cell.Overlay != null && cell.Overlay.markerName != null)
                    {
                        var heightOffset = 0.0f;
                        if (cell.Overlay.mergeConfig != null)
                        {
                            heightOffset += cell.LayoutCell
                                ? cell.Overlay.mergeConfig.markerHeightOffsetForLayoutTiles
                                : cell.Overlay.mergeConfig.markerHeightOffsetForNonLayoutTiles;
                        }

                        var height = cell.Height;
                        var overlayPosition = basePosition + Vector3.Scale(new Vector3(x + 0.5f, height + heightOffset, y + 0.5f), gridSize);
                        var overlayMarkerTransform = Matrix4x4.TRS(overlayPosition, Quaternion.identity, Vector3.one);
                        EmitMarker(cell.Overlay.markerName, overlayMarkerTransform, new IntVector(x, 0, y), cellId);

                        if (cell.Overlay.mergeConfig != null)
                        {
                            removeElevationMarker = cell.Overlay.mergeConfig.removeElevationMarker;
                        }
                    }

                    switch (cell.CellType)
                    {
                        case FlowTilemapCellType.Floor:
                            EmitMarker(GridFlowDungeonMarkerNames.Ground, markerTransform, new IntVector(x, 0, y), cellId);
                            break;

                        case FlowTilemapCellType.Wall:
                            EmitMarker(GridFlowDungeonMarkerNames.Wall, markerTransform, new IntVector(x, 0, y), cellId);
                            EmitMarker(GridFlowDungeonMarkerNames.Ground, markerTransform, new IntVector(x, 0, y), cellId);
                            break;

                        case FlowTilemapCellType.Door:
                            {
                                var doorMarker = GridFlowDungeonMarkerNames.Door;
                                var doorData = cell.Userdata as FlowTilemapCellDoorInfo;
                                if (doorData != null && doorData.oneWay)
                                {
                                    // One way door
                                    doorMarker = GridFlowDungeonMarkerNames.DoorOneWay;

                                    // Apply the correct one-way direction
                                    var flipDirection = (doorData.nodeA.x > doorData.nodeB.x) || (doorData.nodeA.y > doorData.nodeB.y);
                                    if (!flipDirection)
                                    {
                                        var doorRotation = baseRotation * Quaternion.Euler(0, 180, 0);
                                        markerTransform = Matrix4x4.TRS(position, doorRotation, Vector3.one);
                                    }
                                }

                                FlowItemMetadata lockItemData = null;
                                if (cell.Item != DungeonUID.Empty && itemMap.ContainsKey(cell.Item))
                                {
                                    var item = itemMap[cell.Item];
                                    CreateLockItemMetadata(item, ref doorMarker, out lockItemData);
                                }

                                EmitMarker(doorMarker, markerTransform, new IntVector(x, 0, y), cellId, lockItemData);
                                EmitMarker(GridFlowDungeonMarkerNames.Ground, markerTransform, new IntVector(x, 0, y), cellId);
                            }
                            break;

                        case FlowTilemapCellType.Custom:
                            if (cell.CustomCellInfo != null && !removeElevationMarker)
                            {
                                var markerName = cell.CustomCellInfo.name;
                                var height = cell.Height;
                                var customPosition = basePosition + Vector3.Scale(new Vector3(x + 0.5f, height, y + 0.5f), gridSize);
                                var customMarkerTransform = Matrix4x4.TRS(customPosition, Quaternion.identity, Vector3.one);

                                EmitMarker(markerName, customMarkerTransform, new IntVector(x, 0, y), cellId);
                            }
                            break;

                    }
                }
            }

            // Emit the edge markers
            {

                var walkableCellTypes = new FlowTilemapCellType[]
                {
                    FlowTilemapCellType.Floor,
                    FlowTilemapCellType.Wall,
                    FlowTilemapCellType.Door
                };

                var wallSeparators = new HashSet<IntVector2>();
                var fenceSeparators = new HashSet<IntVector2>();

                var layoutGraphLookup = new LayoutGraphLookup(gridFlowModel.LayoutGraph);
                
                foreach (var edge in tilemap.Edges)
                {
                    var coord = edge.EdgeCoord;
                    var isGroundTile = IsCellOfType(tilemap, coord.x, coord.y, walkableCellTypes);
                    var roomType = GetCellRoomType(tilemap, coord.x, coord.y, layoutGraphLookup);

                    if (edge.EdgeType == FlowTilemapEdgeType.Empty) continue;

                    Vector3 position;
                    float angle;
                    if (edge.HorizontalEdge)
                    {
                        position = basePosition + Vector3.Scale(new Vector3(coord.x + 0.5f, 0, coord.y), gridSize);
                        angle = isGroundTile ? 0 : 180;
                        if (isGroundTile && edge.EdgeType == FlowTilemapEdgeType.Wall)
                        {
                            var roomTypeBelow = GetCellRoomType(tilemap, coord.x, coord.y - 1, layoutGraphLookup);
                            if (!ChunkSupportsWalls(roomType) && ChunkSupportsWalls(roomTypeBelow))
                            {
                                angle += 180;
                            }
                        }
                    }
                    else
                    {
                        position = basePosition + Vector3.Scale(new Vector3(coord.x, 0, coord.y + 0.5f), gridSize);
                        angle = isGroundTile ? 90 : 270;
                        if (isGroundTile && edge.EdgeType == FlowTilemapEdgeType.Wall)
                        {
                            var roomTypeLeft = GetCellRoomType(tilemap, coord.x - 1, coord.y, layoutGraphLookup);
                            if (!ChunkSupportsWalls(roomType) && ChunkSupportsWalls(roomTypeLeft))
                            {
                                angle += 180;
                            }
                        }
                    }

                    if (gridFlowConfig.flipEdgeWalls)
                    {
                        if (edge.EdgeType == FlowTilemapEdgeType.Wall || edge.EdgeType == FlowTilemapEdgeType.Fence)
                        {
                            angle += 180;
                        }
                    }
                    
                    var baseRotation = Quaternion.Euler(0, angle, 0);
                    var markerTransform = Matrix4x4.TRS(position, baseRotation, Vector3.one);

                    bool supportedMarker = true;
                    var markerName = "";
                    FlowItemMetadata itemMetadata = null;
                    if (edge.EdgeType == FlowTilemapEdgeType.Wall)
                    {
                        markerName = GridFlowDungeonMarkerNames.Wall;

                        wallSeparators.Add(coord);
                        if (edge.HorizontalEdge)
                        {
                            wallSeparators.Add(coord + new IntVector2(1, 0));
                        }
                        else
                        {
                            wallSeparators.Add(coord + new IntVector2(0, 1));
                        }
                    }
                    else if (edge.EdgeType == FlowTilemapEdgeType.Fence)
                    {
                        markerName = GridFlowDungeonMarkerNames.Fence;

                        fenceSeparators.Add(coord);
                        if (edge.HorizontalEdge)
                        {
                            fenceSeparators.Add(coord + new IntVector2(1, 0));
                        }
                        else
                        {
                            fenceSeparators.Add(coord + new IntVector2(0, 1));
                        }
                    }
                    else if (edge.EdgeType == FlowTilemapEdgeType.Door)
                    {
                        markerName = GridFlowDungeonMarkerNames.Door;
                        var doorData = edge.Userdata as FlowTilemapCellDoorInfo;
                        if (doorData != null && doorData.oneWay)
                        {
                            // One way door
                            markerName = GridFlowDungeonMarkerNames.DoorOneWay;

                            // Apply the correct one-way direction
                            var flipDirection = (doorData.nodeA.x > doorData.nodeB.x) || (doorData.nodeA.y > doorData.nodeB.y);
                            if (!flipDirection)
                            {
                                var doorRotation = baseRotation * Quaternion.Euler(0, 180, 0);
                                markerTransform = Matrix4x4.TRS(position, doorRotation, Vector3.one);
                            }
                        }

                        if (edge.Item != DungeonUID.Empty && itemMap.ContainsKey(edge.Item))
                        {
                            var item = itemMap[edge.Item];
                            CreateLockItemMetadata(item, ref markerName, out itemMetadata);
                        }
                    }
                    else
                    {
                        supportedMarker = false;
                    }

                    if (supportedMarker)
                    {
                        EmitMarker(markerName, markerTransform, new IntVector(coord.x, 0, coord.y), 0, itemMetadata);
                    }
                }

                // Emit wall separator markers
                foreach (var gridCoord in wallSeparators)
                {
                    var position = basePosition + Vector3.Scale(new Vector3(gridCoord.x, 0, gridCoord.y), gridSize);
                    var markerTransform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                    EmitMarker(GridFlowDungeonMarkerNames.WallSeparator, markerTransform, new IntVector(gridCoord.x, 0, gridCoord.y), 0);
                }

                // Emit fence separator markers
                foreach (var gridCoord in fenceSeparators)
                {
                    if (wallSeparators.Contains(gridCoord)) continue;

                    var position = basePosition + Vector3.Scale(new Vector3(gridCoord.x, 0, gridCoord.y), gridSize);
                    var markerTransform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                    EmitMarker(GridFlowDungeonMarkerNames.FenceSeparator, markerTransform, new IntVector(gridCoord.x, 0, gridCoord.y), 0);
                }
            }
        }

        public override void DebugDraw()
        {
            
        }
    }

    public static class GridFlowDungeonMarkerNames
    {
        public static readonly string Ground = "Ground";
        public static readonly string Wall = "Wall";
        public static readonly string WallSeparator = "WallSeparator";
        public static readonly string Fence = "Fence";
        public static readonly string FenceSeparator = "FenceSeparator";
        public static readonly string Door = "Door";
        public static readonly string DoorOneWay = "DoorOneWay";

    }

}
