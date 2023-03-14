//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Utils.Noise;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Tilemap.Tasks
{
    public enum TilemapFlowNodeHandler_CreateTilemapOverlayGenMethod
    {
        Noise,
        Script
    }

    [System.Serializable]
    public class TilemapFlowNodeOverlayNoiseSettings
    {
        public int noiseOctaves = 4;
        public float noiseFrequency = 0.15f;
        public float noiseValuePower = 0;
        public float noiseMinValue = 0;
        public float noiseMaxValue = 1.0f;
        public float noiseThreshold = 0.5f;
        public int minDistFromMainPath = 2;
    }

    public interface ITilemapFlowOverlayGenerator
    {
        void Init(System.Random random);
        bool Generate(FlowTilemapCell cell, FlowTilemapCell incomingCell, System.Random random, ref float overlayValue);
    }

    public class NoiseTilemapFlowOverlayGenerator : ITilemapFlowOverlayGenerator
    {
        GradientNoiseTable noiseTable;
        TilemapFlowNodeOverlayNoiseSettings noiseSettings;
        public NoiseTilemapFlowOverlayGenerator(TilemapFlowNodeOverlayNoiseSettings noiseSettings)
        {
            this.noiseSettings = noiseSettings;
        }

        public void Init(System.Random random)
        {
            noiseTable = new GradientNoiseTable();
            noiseTable.Init(128, random);

            noiseSettings.minDistFromMainPath = Mathf.Max(1, noiseSettings.minDistFromMainPath);
        }

        public bool Generate(FlowTilemapCell cell, FlowTilemapCell incomingCell, System.Random random, ref float overlayValue)
        {
            var cellCoord = incomingCell.TileCoord;
            var position = cellCoord.ToVector2() * noiseSettings.noiseFrequency;
            var n = noiseTable.GetNoiseFBM(position, noiseSettings.noiseOctaves);
            if (noiseSettings.noiseValuePower > 0.0f)
            {
                n = Mathf.Pow(n, noiseSettings.noiseValuePower);
            }

            n = noiseSettings.noiseMinValue + (noiseSettings.noiseMaxValue - noiseSettings.noiseMinValue) * n;

            if (n > noiseSettings.noiseThreshold)
            {
                var distanceFromMainPath = incomingCell.DistanceFromMainPath;
                float noiseFactor = (n - noiseSettings.noiseThreshold) / (1.0f - noiseSettings.noiseThreshold);
                bool insertOverlay = (noiseFactor * distanceFromMainPath > noiseSettings.minDistFromMainPath);

                if (insertOverlay)
                {
                    overlayValue = n;
                    return true;
                }
            }
            return false;
        }
    }
    
    public class TilemapBaseFlowTaskCreateOverlay : FlowExecTask
    {
        public string markerName = "Tree";
        public Color color = Color.green;
        public bool overlayBlocksTile = true;
        public TilemapFlowNodeHandler_CreateTilemapOverlayGenMethod generationMethod = TilemapFlowNodeHandler_CreateTilemapOverlayGenMethod.Noise;
        public TilemapFlowNodeOverlayNoiseSettings noiseSettings = new TilemapFlowNodeOverlayNoiseSettings();
        public FlowTilemapCellOverlayMergeConfig mergeConfig = new FlowTilemapCellOverlayMergeConfig();
        public string generatorScriptClass;

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
            var generator = createGeneratorInstance();
            if (generator == null)
            {
                output.ErrorMessage = "Invalid script reference";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            generator.Init(random);

            // Create overlays
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    var incomingCell = incomingTilemap.Cells[x, y];
                    var cell = tilemap.Cells[x, y];

                    float overlayValue = 0;
                    if (generator.Generate(cell, incomingCell, random, ref overlayValue))
                    {
                        var overlay = new FlowTilemapCellOverlay();
                        overlay.markerName = markerName;
                        overlay.color = color;
                        overlay.noiseValue = overlayValue;
                        overlay.mergeConfig = mergeConfig;
                        overlay.tileBlockingOverlay = overlayBlocksTile;

                        cell.Overlay = overlay;
                    }
                }
            }

            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }

        ITilemapFlowOverlayGenerator createGeneratorInstance()
        {
            ITilemapFlowOverlayGenerator generator = null;
            if (generationMethod == TilemapFlowNodeHandler_CreateTilemapOverlayGenMethod.Noise)
            {
                generator = new NoiseTilemapFlowOverlayGenerator(noiseSettings);
            }
            else if (generationMethod == TilemapFlowNodeHandler_CreateTilemapOverlayGenMethod.Script)
            {
                if (generatorScriptClass != null)
                {
                    var type = System.Type.GetType(generatorScriptClass);
                    if (type != null)
                    {
                        generator = ScriptableObject.CreateInstance(type) as ITilemapFlowOverlayGenerator;
                    }
                }
            }
            return generator;
        }
    }
}