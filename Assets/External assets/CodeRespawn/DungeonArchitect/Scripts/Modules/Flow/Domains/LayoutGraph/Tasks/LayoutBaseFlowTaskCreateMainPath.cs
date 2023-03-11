//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tasks
{
    public class LayoutBaseFlowTaskCreateMainPath : LayoutBaseFlowTaskPathBuilderBase
    {
        public int pathSize = 12;
        public string pathName = "main";
        public Color nodeColor = Color.green;
        public string startMarkerName = "SpawnPoint";
        public string goalMarkerName = "LevelGoal";
        public string startNodePathName = "main_start";
        public string goalNodePathName = "main_goal";
        public bool drawDebug = false;

        // Number of searches to perform at once.  This helps converge to a solution faster if we are stuck on a single search path.
        // However, this might increase the overall search time by a little bit. 
        public int numParallelSearches = 1;
        public long maxFramesToProcess = 1000;

        protected virtual bool Validate(FlowTaskExecContext context, FlowTaskExecInput input, ref string errorMessage, ref FlowTaskExecutionResult executionResult)
        {
            return true;
        }

        protected virtual void ProcessEntranceItem(FlowItem entranceItem, FlowLayoutGraphNode entranceNode)
        {
        }

        public virtual void ProcessGoalItem(FlowItem goalItem, FlowLayoutGraphNode goalNode)
        {
            
        }

        protected override void FinalizePath(FlowLayoutStaticGrowthState staticState, FlowLayoutSharedGrowthState sharedState, FlowLayoutGrowthState state)
        {
            base.FinalizePath(staticState, sharedState, state);
            
            var graphQuery = staticState.GraphQuery;
            
            // Add an entry item to the first node
            {
                var entranceNode = graphQuery.GetNode(state.Path[0].NodeId);
                // Override the entrance node path name
                entranceNode.pathName = startNodePathName;
                    
                // Add a spawn point item
                var item = entranceNode.CreateItem<FlowItem>();
                item.type = FlowGraphItemType.Entrance;
                item.markerName = startMarkerName;
                ProcessEntranceItem(item, entranceNode);
            }
                
            // Add a goal item to the last node
            {
                var goalNodeId = state.Path[state.Path.Count - 1].NodeId;
                var goalNode = graphQuery.GetNode(goalNodeId);
                    
                // Override the goal node path name
                goalNode.pathName = goalNodePathName;
                    
                // Add a goal item
                var item = goalNode.CreateItem<FlowItem>();
                item.type = FlowGraphItemType.Exit;
                item.markerName = goalMarkerName;
                ProcessGoalItem(item, goalNode);
            }
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

            if (pathSize <= 0)
            {
                output.ErrorMessage = "Invalid path size";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            output.State = input.CloneInputState();
            var graph = output.State.GetState<FlowLayoutGraph>();

            if (graph == null || graph.Nodes.Count == 0)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            if (!Validate(context, input, ref output.ErrorMessage, ref output.ExecutionResult))
            {
                return output;
            }
            
            var graphQuery = new FlowLayoutGraphQuery(graph);
            var staticState = new FlowLayoutStaticGrowthState();
            staticState.Graph = graph;
            staticState.GraphQuery = graphQuery;
            staticState.Random = context.Random;
            staticState.MinPathSize = pathSize;
            staticState.MaxPathSize = pathSize;
            staticState.NodeColor = nodeColor;
            staticState.PathName = pathName;
            staticState.NodeGroupGenerator = CreateNodeGroupGenerator(context.DomainExtensions, graph);
            staticState.GraphConstraint = CreateGraphConstraint(context.DomainExtensions, graph);
            staticState.NodeCreationConstraint = CreateNodeCreationConstraint(context.DomainExtensions, graph);

            int[] shuffledEntranceIndices = MathUtils.GetShuffledIndices(graph.Nodes.Count, context.Random);
            var pathingSystem = new FFlowAgPathingSystem(maxFramesToProcess);
            foreach (var nodeIndex in shuffledEntranceIndices)
            {
                var startNode = graph.Nodes[nodeIndex];
                if (startNode == null || startNode.active)
                {
                    // Cannot use this node
                    continue;
                }
                
                pathingSystem.RegisterGrowthSystem(startNode, staticState);
            }
            
            pathingSystem.Execute(numParallelSearches);
            if (pathingSystem.FoundResult)
            {
                var pathResult = pathingSystem.Result;
                FinalizePath(pathResult.StaticState, pathResult.SharedState, pathResult.State);

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