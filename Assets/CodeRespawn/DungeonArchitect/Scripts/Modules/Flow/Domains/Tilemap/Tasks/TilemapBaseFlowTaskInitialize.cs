//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;

namespace DungeonArchitect.Flow.Domains.Tilemap.Tasks
{
    [System.Serializable]
    public enum TilemapFlowNodeWallGenerationMethod
    {
        WallAsTiles,
        WallAsEdges,
    }

    public abstract class TilemapBaseFlowTaskInitialize : FlowExecTask
    {
        protected virtual bool Validate(FlowTaskExecContext context, FlowTaskExecInput input, FlowTaskExecOutput output)
        {
            return true;
        }

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
            if (!Validate(context, input, output))
            {
                // Failed
                return output;
            }
            
            var graph = output.State.GetState<FlowLayoutGraph>();  // TODO: Remove the need to cast
            if (graph == null)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            var tilemap = BuildTilemap(graph, context.Random);
            if (tilemap == null)
            {
                output.ErrorMessage = "Failed to generate tilemap";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            output.State.SetState(typeof(FlowTilemap), tilemap);
            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }

        protected abstract FlowTilemap BuildTilemap(FlowLayoutGraph graph, System.Random random);

    }
    
}