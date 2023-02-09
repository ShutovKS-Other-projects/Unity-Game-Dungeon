//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;

namespace DungeonArchitect.Flow.Domains.Tilemap.Tasks
{
    public class TilemapBaseFlowTaskFinalize : FlowExecTask
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

            var graph = output.State.GetState<FlowLayoutGraph>();

            if (!AssignItems(tilemap, graph, context.Random, ref output.ErrorMessage))
            {
                output.ExecutionResult = FlowTaskExecutionResult.FailRetry;
                return output;
            }
            
            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }

        protected virtual bool AssignItems(FlowTilemap tilemap, FlowLayoutGraph graph, System.Random random, ref string errorMessage)
        {
            return true;
        }
    }
}
