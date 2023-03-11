//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tasks
{
    public class LayoutBaseFlowTaskCreatePath : LayoutBaseFlowTaskPathBuilderBase
    {
        public int minPathSize = 3;
        public int maxPathSize = 3;
        public string pathName = "branch";
        public Color nodeColor = new Color(1, 0.5f, 0);

        public string startFromPath = "main";
        public string endOnPath = "";

        // Override the path name of the first node in the path.  Useful for connecting other paths to it
        public string startNodePathNameOverride = "";
    
        // Override the path name of the first node in the path.  Useful for connecting other paths to it
        public string endNodePathNameOverride = "";

        // Number of searches to perform at once.  This helps converge to a solution faster if we are stuck on a single search path.
        // However, this might increase the overall search time by a little bit. 
        public int numParallelSearches = 1;
        public long maxFramesToProcess = 2000;
        
        public bool drawDebug = false;
        
        
        class StartNodeCandidate
        {
            public DungeonUID StartNodeId;
            public FlowLayoutGraphNode OriginatingHeadNode;
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

            if (minPathSize <= 0)
            {
                output.ErrorMessage = "Invalid path size";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            
            output.State = input.CloneInputState();
            var graph = output.State.GetState<FlowLayoutGraph>();
            maxPathSize = Mathf.Max(maxPathSize, minPathSize);

            if (graph == null || graph.Nodes.Count == 0)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            var graphQuery = new FlowLayoutGraphQuery(graph);
            var possibleStartNodes = new List<StartNodeCandidate>();

            // Find the start node candidates
            {
                var sourceNodes = FlowLayoutGraphUtils.FindNodesOnPath(graph, startFromPath);
                if (sourceNodes.Length == 0)
                {
                    output.ErrorMessage = string.Format("Start path '{0}' not found", startFromPath);
                    output.ExecutionResult = FlowTaskExecutionResult.FailRetry;
                    return output;
                }
                

                foreach (var headNode in sourceNodes)
                {
                    if (headNode == null) continue;

                    var startNodeIds = graphQuery.GetConnectedNodes(headNode.nodeId);
                    foreach (var startNodeId in startNodeIds)
                    {
                        var startNode = graphQuery.GetNode(startNodeId);
                        if (startNode == null || startNode.active)
                        {
                            continue;
                        }
                        var startNodeInfo = new StartNodeCandidate();
                        startNodeInfo.StartNodeId = startNodeId;
                        startNodeInfo.OriginatingHeadNode = headNode;
                        possibleStartNodes.Add(startNodeInfo);
                    }
                }
                
                if (possibleStartNodes.Count == 0)
                {
                    output.ErrorMessage = string.Format("Not enough space to grow out of '{0}'", startFromPath);
                    output.ExecutionResult = FlowTaskExecutionResult.FailRetry;
                    return output;
                }
            }

            
            
            // Find the sink node candidates
            var sinkNodes = FlowLayoutGraphUtils.FindNodesOnPath(graph, endOnPath);
            if (endOnPath.Length > 0 && sinkNodes.Length == 0)
            {
                output.ErrorMessage = string.Format("End path '{0}' not found", endOnPath);
                output.ExecutionResult = FlowTaskExecutionResult.FailRetry;
                return output;
            }

            var pathingSystem = new FFlowAgPathingSystem(maxFramesToProcess);
            {
                var visitedStartNodes = new HashSet<DungeonUID>();
                var startNodeIndices = MathUtils.GetShuffledIndices(possibleStartNodes.Count, context.Random);
                foreach (var startNodeIdx in startNodeIndices)
                {
                    var startNodeInfo = possibleStartNodes[startNodeIdx];
                    var startNodeId = startNodeInfo.StartNodeId;
                    if (visitedStartNodes.Contains(startNodeId)) continue;
                    visitedStartNodes.Add(startNodeId);

                    var startNode = graphQuery.GetNode(startNodeId);
                    if (startNode == null || startNode.active) continue;

                    var staticState = new FlowLayoutStaticGrowthState();
                    staticState.Graph = graph;
                    staticState.GraphQuery = graphQuery;
                    staticState.HeadNode = startNodeInfo.OriginatingHeadNode;
                    staticState.SinkNodes = new List<FlowLayoutGraphNode>(sinkNodes);
                    staticState.Random = context.Random;
                    staticState.MinPathSize = minPathSize;
                    staticState.MaxPathSize = maxPathSize;
                    staticState.NodeColor = nodeColor;
                    staticState.PathName = pathName;
                    staticState.StartNodePathNameOverride = startNodePathNameOverride;
                    staticState.EndNodePathNameOverride = endNodePathNameOverride;
                    staticState.NodeGroupGenerator = CreateNodeGroupGenerator(context.DomainExtensions, graph);
                    staticState.GraphConstraint = CreateGraphConstraint(context.DomainExtensions, graph);
                    staticState.NodeCreationConstraint = CreateNodeCreationConstraint(context.DomainExtensions, graph);
                    
                    pathingSystem.RegisterGrowthSystem(startNode, staticState);
                }
            }

            pathingSystem.Execute(numParallelSearches);
            if (pathingSystem.FoundResult)
            {
                var result = pathingSystem.Result;
                FinalizePath(result.StaticState, result.SharedState, result.State);

                output.ExecutionResult = FlowTaskExecutionResult.Success;
                return output;
            }

            EFlowLayoutGrowthErrorType pathingError = pathingSystem.GetLastError();
            if (pathingError == EFlowLayoutGrowthErrorType.CannotMerge)
            {
                output.ErrorMessage = "Cannot Merge back";
            }
            else if (pathingError == EFlowLayoutGrowthErrorType.NodeConstraint)
            {
                output.ErrorMessage = "Error: Check Constraints";
            }
            else if (pathingError == EFlowLayoutGrowthErrorType.GraphConstraint)
            {
                output.ErrorMessage = "Error: Check Module Constraints";
            }
            else
            {
                output.ErrorMessage = "Cannot find path";
            }
            output.ExecutionResult = FlowTaskExecutionResult.FailRetry;
            return output;
        }
    }
}