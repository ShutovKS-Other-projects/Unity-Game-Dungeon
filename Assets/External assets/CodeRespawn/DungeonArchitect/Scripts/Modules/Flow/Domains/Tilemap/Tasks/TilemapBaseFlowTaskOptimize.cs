//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Exec;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Tilemap.Tasks
{
    public class TilemapBaseFlowTaskOptimize : FlowExecTask
    {
        public int discardDistanceFromLayout= 3;
        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            var output = new FlowTaskExecOutput();
            if (input.IncomingTaskOutputs.Length == 0)
            {
                output.ErrorMessage = "Missing Input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            if (input.IncomingTaskOutputs.Length > 1)
            {
                output.ErrorMessage = "Only one input allowed";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            
            output.State = input.CloneInputState();
            var tilemap = output.State.GetState<FlowTilemap>();
            if (tilemap == null)
            {
                output.ErrorMessage = "Missing tilemap input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            
            DiscardDistantTiles(tilemap);

            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }

        void DiscardDistantTiles(FlowTilemap tilemap)
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

            var distanceFromLayout = new Dictionary<FlowTilemapCell, int>();
            foreach (var cell in tilemap.Cells)
            {
                if (cell.LayoutCell)
                {
                    queue.Enqueue(cell);
                    distanceFromLayout[cell] = 0;
                }
            }
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();

                // Traverse the children
                var childDistance = distanceFromLayout[cell] + 1;
                for (int i = 0; i < 4; i++)
                {
                    int nx = cell.TileCoord.x + childOffsets[i * 2 + 0];
                    int ny = cell.TileCoord.y + childOffsets[i * 2 + 1];
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        var ncell = tilemap.Cells[nx, ny];
                        if (ncell.LayoutCell) continue;
                        if (!distanceFromLayout.ContainsKey(ncell) || childDistance < distanceFromLayout[ncell])
                        {
                            distanceFromLayout[ncell] = childDistance;
                            queue.Enqueue(ncell);
                        }
                    }
                }
            }
            discardDistanceFromLayout = Mathf.Max(0, discardDistanceFromLayout);
            foreach (var cell in tilemap.Cells)
            {
                if (!distanceFromLayout.ContainsKey(cell)) continue;
                if (cell.LayoutCell) continue;
                if (distanceFromLayout[cell] > discardDistanceFromLayout)
                {
                    cell.Clear();
                }
            }
        }
    }
}