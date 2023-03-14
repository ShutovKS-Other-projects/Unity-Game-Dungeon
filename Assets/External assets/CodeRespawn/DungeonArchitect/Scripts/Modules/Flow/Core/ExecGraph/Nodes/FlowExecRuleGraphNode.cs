//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine.Serialization;

namespace DungeonArchitect.Flow.Exec
{
    public enum GridFlowGraphNodeExecutionStage
    {
        NotExecuted,
        WaitingToExecute,       // Dependent nodes are being processed
        Executed                // The node has been executed
    }
    public class GridFlowGraphNodeExecutionStatus
    {
        public GridFlowGraphNodeExecutionStage ExecutionStage { get; set; }
        public FlowTaskExecutionResult Success { get; set; }
        public string ErrorMessage { get; set; }

        public GridFlowGraphNodeExecutionStatus()
        {
            ExecutionStage = GridFlowGraphNodeExecutionStage.NotExecuted;
            Success = FlowTaskExecutionResult.FailHalt;
            ErrorMessage = "";
        }
    }

    public class FlowExecRuleGraphNode : FlowExecGraphNodeBase
    {
        [FormerlySerializedAs("nodeHandler")] 
        public FlowExecTask task;
        
        public GridFlowGraphNodeExecutionStatus executionStatus;
    }
}
