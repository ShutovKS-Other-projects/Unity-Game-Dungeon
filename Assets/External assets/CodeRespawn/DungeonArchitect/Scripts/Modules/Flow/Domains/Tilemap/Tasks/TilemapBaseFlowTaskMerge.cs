//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;

namespace DungeonArchitect.Flow.Domains.Tilemap.Tasks
{
    public class TilemapBaseFlowTaskMerge : FlowExecTask
    {   
        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            var output = new FlowTaskExecOutput();
            if (input.IncomingTaskOutputs.Length == 0)
            {
                output.ErrorMessage = "Missing Input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            FlowLayoutGraph incomingGraph = null;
            var incomingTilemaps = new List<FlowTilemap>();
            foreach (var incomingOutput in input.IncomingTaskOutputs)
            {
                var incomingTilemap = incomingOutput.State.GetState<FlowTilemap>();
                if (incomingTilemap != null)
                {
                    incomingTilemaps.Add(incomingTilemap);
                }

                if (incomingGraph == null)
                {
                    incomingGraph = incomingOutput.State.GetState<FlowLayoutGraph>();
                }
            }
            
            if (incomingTilemaps.Count == 0)
            {
                output.ErrorMessage = "Missing tilemap input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            
            if (incomingGraph == null)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            var tilemap = new FlowTilemap(incomingTilemaps[0].Width, incomingTilemaps[0].Height);
            var graph = incomingGraph.Clone() as FlowLayoutGraph;

            output.State.SetState(typeof(FlowTilemap), tilemap);
            output.State.SetState(typeof(FlowLayoutGraph), graph);

            var registeredStateTypes = new HashSet<System.Type>();
            registeredStateTypes.Add(typeof(FlowTilemap));
            registeredStateTypes.Add(typeof(FlowLayoutGraph));
            
            // Clone all the missing states from the incoming nodes
            foreach (var incomingOutput in input.IncomingTaskOutputs)
            {
                foreach (var incomingOutputStateType in incomingOutput.State.GetRegisteredStateTypes())
                {
                    if (!registeredStateTypes.Contains(incomingOutputStateType))
                    {
                        var incomingState = incomingOutput.State.GetState(incomingOutputStateType);
                        if (incomingState != null)
                        {
                            var clonedState = incomingState.Clone() as ICloneable;
                            if (clonedState != null)
                            {
                                output.State.SetState(incomingOutputStateType, clonedState);
                                registeredStateTypes.Add(incomingOutputStateType);
                            }
                        }
                    }
                }
            }

            // merge the cells
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    int bestWeight = 0;
                    FlowTilemapCell bestCell = null;
                    var incomingOverlays = new List<FlowTilemapCellOverlay>();
                    foreach (var incomingTilemap in incomingTilemaps)
                    {
                        var weight = 0;
                        var incomingCell = incomingTilemap.Cells[x, y];
                        if (incomingCell.CellType == FlowTilemapCellType.Empty)
                        {
                            weight = 1;
                        }
                        else if (incomingCell.CellType == FlowTilemapCellType.Custom)
                        {
                            weight = 2;
                        }
                        else
                        {
                            weight = 3;
                        }

                        if (incomingCell.Overlay != null)
                        {
                            incomingOverlays.Add(incomingCell.Overlay);
                        }

                        bool useResult = false;
                        if (weight > bestWeight)
                        {
                            useResult = true;
                        }
                        else if (weight == bestWeight)
                        {
                            if (bestCell != null && incomingCell.Height > bestCell.Height)
                            {
                                useResult = true;
                            }
                        }

                        if (useResult)
                        {
                            bestCell = incomingCell;
                            bestWeight = weight;
                        }
                    }
                    
                    tilemap.Cells[x, y] = bestCell.Clone();
                    var resultCell = tilemap.Cells[x, y];
                    FlowTilemapCellOverlay bestOverlay = null;
                    float bestOverlayWeight = 0;
                    foreach (var incomingOverlay in incomingOverlays)
                    {
                        var valid = resultCell.Height >= incomingOverlay.mergeConfig.minHeight
                                   && resultCell.Height <= incomingOverlay.mergeConfig.maxHeight;

                        if (valid)
                        {
                            if (bestOverlay == null || incomingOverlay.noiseValue > bestOverlayWeight)
                            {
                                bestOverlay = incomingOverlay;
                                bestOverlayWeight = incomingOverlay.noiseValue;
                            }
                        }
                    }
                    if (bestOverlay != null)
                    {
                        resultCell.Overlay = bestOverlay.Clone();
                    }
                }
            }

            // Merge the edges
            for (int y = 0; y <= tilemap.Height; y++)
            {
                for (int x = 0; x <= tilemap.Width; x++)
                {
                    FlowTilemapEdge bestEdgeH = null;
                    FlowTilemapEdge bestEdgeV = null;
                    foreach (var incomingTilemap in incomingTilemaps)
                    {
                        var incomingEdgeH = incomingTilemap.Edges.GetHorizontal(x, y);
                        var incomingEdgeV = incomingTilemap.Edges.GetVertical(x, y);
                        if (incomingEdgeH.EdgeType != FlowTilemapEdgeType.Empty)
                        {
                            bestEdgeH = incomingEdgeH;
                        }
                        if (incomingEdgeV.EdgeType != FlowTilemapEdgeType.Empty)
                        {
                            bestEdgeV = incomingEdgeV;
                        }
                    }

                    if (bestEdgeH != null)
                    {
                        tilemap.Edges.SetHorizontal(x, y, bestEdgeH.Clone());
                    }
                    if (bestEdgeV != null)
                    {
                        tilemap.Edges.SetVertical(x, y, bestEdgeV.Clone());
                    }
                }
            }


            foreach (var cell in tilemap.Cells)
            {
                if (cell.CellType == FlowTilemapCellType.Wall && cell.Overlay != null)
                {
                    if (cell.Overlay.mergeConfig != null)
                    {
                        var wallOverlayRule = cell.Overlay.mergeConfig.wallOverlayRule;
                        if (wallOverlayRule == FlowTilemapCellOverlayMergeWallOverlayRule.KeepOverlayRemoveWall)
                        {
                            cell.CellType = FlowTilemapCellType.Floor;
                            cell.UseCustomColor = true;
                        }
                        else if (wallOverlayRule == FlowTilemapCellOverlayMergeWallOverlayRule.KeepWallRemoveOverlay)
                        {
                            cell.Overlay = null;
                        }
                    }
                }
            }

            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }
    }
}