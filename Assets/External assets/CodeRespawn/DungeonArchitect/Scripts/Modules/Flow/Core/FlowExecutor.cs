//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains;
using UnityEngine;

namespace DungeonArchitect.Flow.Exec
{
    public class FlowExecutionContext
    {
        public System.Random Random { get; set; }
        public FlowExecGraph ExecGraph { get; set; }
        public FlowExecNodeOutputRegistry NodeOutputRegistry { get; set; }
        public FlowDomainExtensions DomainExtensions { get; set; }
        public HashSet<FlowExecRuleGraphNode> Visited { get; private set; }

        public FlowExecutionContext()
        {
            Visited = new HashSet<FlowExecRuleGraphNode>();
        }
    }

    public class FlowExecutor
    {
        public int RetriesUsed { get; set; } = 0;
        
        public bool Execute(FlowExecGraph execGraph, System.Random random, FlowDomainExtensions domainExtensions, int numTries, out FlowExecNodeOutputRegistry nodeOutputRegistry)
        {
            if (execGraph == null || random == null)
            {
                Debug.LogError("Invalid asset state");
                nodeOutputRegistry = null;
                return false;
            }

            if (execGraph.resultNode == null)
            {
                Debug.LogError("Cannot find result node in Execution Graph");
                nodeOutputRegistry = null;
                return false;
            }

            RetriesUsed = 0;
            FlowTaskExecutionResult lastRunStatus = FlowTaskExecutionResult.FailHalt;
            while (RetriesUsed < numTries) {
                RetriesUsed++;

                var context = new FlowExecutionContext();
                context.ExecGraph = execGraph;
                context.Random = random;
                context.DomainExtensions = domainExtensions;
                context.NodeOutputRegistry = new FlowExecNodeOutputRegistry(); 

                var taskOutput = ExecuteGraph(context);
                lastRunStatus = (taskOutput != null) ? taskOutput.ExecutionResult : FlowTaskExecutionResult.FailHalt; 
                if (lastRunStatus == FlowTaskExecutionResult.Success)
                {
                    //if (tries > 1) {
                    //    Debug.Log("Num Tries: " + tries);
                    //}
                    nodeOutputRegistry = context.NodeOutputRegistry;
                    return true;
                }
                else if (lastRunStatus == FlowTaskExecutionResult.FailHalt)
                {
                    break;
                }
            }

            nodeOutputRegistry = null;
            return false;
        }

        private FlowTaskExecOutput ExecuteGraph(FlowExecutionContext context)
        {
            foreach (var node in context.ExecGraph.Nodes)
            {
                var execNode = node as FlowExecRuleGraphNode;
                if (execNode != null)
                {
                    execNode.executionStatus = new GridFlowGraphNodeExecutionStatus();
                }
            }
            return ExecuteNode(context, context.ExecGraph.resultNode);
        }

        private FlowTaskExecOutput ExecuteNode(FlowExecutionContext context, FlowExecRuleGraphNode execNode)
        {
            context.Visited.Add(execNode);

            execNode.executionStatus.ExecutionStage = GridFlowGraphNodeExecutionStage.WaitingToExecute;

            var incomingNodes = FlowExecGraphUtils.GetIncomingNodes(execNode);
            var incomingTaskOutputs = new List<FlowTaskExecOutput>();
            foreach (var incomingNode in incomingNodes)
            {
                FlowTaskExecOutput incomingTaskOutput = null;
                if (!context.Visited.Contains(incomingNode))
                {
                    incomingTaskOutput = ExecuteNode(context, incomingNode);
                    if (incomingTaskOutput.ExecutionResult != FlowTaskExecutionResult.Success)
                    {
                        return incomingTaskOutput;
                    }
                }
                else
                {
                    incomingTaskOutput = context.NodeOutputRegistry.Get(incomingNode.Id);
                }
                
                incomingTaskOutputs.Add(incomingTaskOutput);
                Debug.Assert(incomingTaskOutput != null);
            }

            var taskContext = new FlowTaskExecContext();
            taskContext.Random = context.Random;
            taskContext.DomainExtensions = context.DomainExtensions;
            
            var taskInput = new FlowTaskExecInput();
            taskInput.IncomingTaskOutputs = incomingTaskOutputs.ToArray();

            var taskOutput = execNode.task.Execute(taskContext, taskInput);
            {
                execNode.executionStatus.ErrorMessage = taskOutput.ErrorMessage;
                execNode.executionStatus.Success = taskOutput.ExecutionResult;
                execNode.executionStatus.ExecutionStage = GridFlowGraphNodeExecutionStage.Executed;
                context.NodeOutputRegistry.Register(execNode.Id, taskOutput);
            }

            return taskOutput;
        }
    }
}
