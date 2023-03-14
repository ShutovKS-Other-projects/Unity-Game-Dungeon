//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Utils.Noise;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Tilemap.Tasks
{
    public class TilemapBaseFlowTaskCreateElevations : FlowExecTask
    {
        public string markerName = "Rock";

        public int noiseOctaves = 4;
        public float noiseFrequency = 0.01f;
        public float noiseValuePower = 0;
        public int numSteps = 4;

        public float minHeight = -20;
        public float maxHeight = -5;
        public float seaLevel = -10;

        public Color landColor = new Color(0.4f, 0.2f, 0);
        public Color seaColor = new Color(0, 0, 0.4f);
        public float minColorMultiplier = 0.1f;

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
            
            var incomingTilemap = input.IncomingTaskOutputs[0].State.GetState<FlowTilemap>();
            if (incomingTilemap == null)
            {
                output.ErrorMessage = "Missing tilemap input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            
            var incomingGraph = input.IncomingTaskOutputs[0].State.GetState<FlowLayoutGraph>();
            if (incomingGraph == null)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            var tilemap = new FlowTilemap(incomingTilemap.Width, incomingTilemap.Height);
            var graph = incomingGraph.Clone() as FlowLayoutGraph;

            output.State.SetState(typeof(FlowTilemap), tilemap);
            output.State.SetState(typeof(FlowLayoutGraph), graph);
            
            var random = context.Random;
            var noiseTable = new GradientNoiseTable();
            noiseTable.Init(128, random);

            // Assign the valid traversable paths 
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    var incomingCell = incomingTilemap.Cells[x, y];
                    var cell = tilemap.Cells[x, y];
                    float cellHeight = 0;
                    if (incomingCell.CellType == FlowTilemapCellType.Empty)
                    {
                        var position = new Vector2(x, y) * noiseFrequency;
                        var n = noiseTable.GetNoiseFBM(position, noiseOctaves);
                        if (noiseValuePower > 1e-6f)
                        {
                            n = Mathf.Pow(n, noiseValuePower);
                        }
                        n = Mathf.Floor(n * numSteps) / numSteps;
                        cellHeight = minHeight + n * (maxHeight - minHeight);
                    }

                    cell.CellType = FlowTilemapCellType.Custom;
                    cell.CustomCellInfo = new FlowTilemapCustomCellInfo();
                    cell.CustomCellInfo.name = markerName;
                    cell.Height = cellHeight;
                    var color = (cell.Height <= seaLevel) ? seaColor : landColor;
                    var minColor = color * minColorMultiplier;
                    var colorBrightness = 1.0f;
                    if (Mathf.Abs(maxHeight - minHeight) > 1e-6f)
                    {
                        colorBrightness = (cell.Height - minHeight) / (maxHeight - minHeight);
                    }
                    cell.CustomCellInfo.defaultColor = Color.Lerp(minColor, color, colorBrightness);
                }
            }

            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }
    }
}