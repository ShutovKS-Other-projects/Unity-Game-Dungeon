//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Flow.Domains.Layout.Tasks;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Impl.GridFlow.Constraints;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.GridFlow.Tasks
{
    [FlowExecNodeInfo("Create Main Path", "Layout Graph/", 1010)]
    public class GridFlowLayoutTaskCreateMainPath : LayoutBaseFlowTaskCreateMainPath
    {
        [System.Serializable]
        public enum NodeConstraintType
        {
            None,
            StartEndNode,
            Script
        }
        
        public TilemapItemPlacementSettings startPlacementSettings = new TilemapItemPlacementSettings();        // TODO: Move this to grid flow impl
        public TilemapItemPlacementSettings goalPlacementSettings = new TilemapItemPlacementSettings();         // TODO: Move this to grid flow impl

        // Node position constraints
        public NodeConstraintType positionConstraintMode;
        public Vector2Int[] startNodePositionConstraints;
        public Vector2Int[] endNodePositionConstraints;
        // A ScriptableObject that implements IGridFlowLayoutNodePositionConstraint
        public string nodePositionConstraintScriptClassName;

        public bool fixedStartRoomSize = false;
        public bool fixedEndRoomSize = false;
        
        private readonly InstanceCache instanceCache = new InstanceCache();
        
        protected override bool Validate(FlowTaskExecContext context, FlowTaskExecInput input, ref string errorMessage, ref FlowTaskExecutionResult executionResult)
        {
            string placementErrorMessage = "";
            if (!TilemapItemPlacementStrategyUtils.Validate(startPlacementSettings, ref placementErrorMessage))
            {
                errorMessage = "Start Item: " + placementErrorMessage;
                executionResult = FlowTaskExecutionResult.FailHalt;
                return false;
            }

            if (!TilemapItemPlacementStrategyUtils.Validate(goalPlacementSettings, ref placementErrorMessage))
            {
                errorMessage = "Goal Item: " + placementErrorMessage;
                executionResult = FlowTaskExecutionResult.FailHalt;
                return false;
            }

            return true;
        }

        protected override void ProcessEntranceItem(FlowItem entranceItem, FlowLayoutGraphNode entranceNode)
        {
            if (startPlacementSettings != null)
            {
                entranceItem.SetDomainData(startPlacementSettings.Clone() as TilemapItemPlacementSettings);
            }
        }

        public override void ProcessGoalItem(FlowItem goalItem, FlowLayoutGraphNode goalNode)
        {
            if (goalPlacementSettings != null)
            {
                goalItem.SetDomainData(goalPlacementSettings.Clone() as TilemapItemPlacementSettings);
            }
        }

        protected override void FinalizePath(FlowLayoutStaticGrowthState staticState, FlowLayoutSharedGrowthState sharedState, FlowLayoutGrowthState state)
        {
            base.FinalizePath(staticState, sharedState, state);
            
            // Tag the nodes as main path
            int size = state.Path.Count;
            for (var i = 0; i < size; i++)
            {
                var pathItem = state.Path[i];
                var pathNode = staticState.GraphQuery.GetNode(pathItem.NodeId);
                if (pathNode != null)
                {
                    pathNode.mainPath = true;

                    var nodeState = pathNode.GetDomainData<GridFlowLayoutNodeState>();
                    nodeState.CanPerturb = true;

                    if (i == 0 && fixedStartRoomSize)
                    {
                        nodeState.CanPerturb = false;
                    }
                    
                    if (i == size - 1 && fixedEndRoomSize)
                    {
                        nodeState.CanPerturb = false;
                    }
                }
            }
        }
        
        private Vector2Int FindGridSize(FlowLayoutGraph graph)
        {
            var gridSize = Vector2Int.zero;
            foreach (var node in graph.Nodes)
            {
                if (node != null)
                {
                    var coord = MathUtils.RoundToVector3Int(node.coord);
                    gridSize.x = Mathf.Max(gridSize.x, coord.x + 1);
                    gridSize.y = Mathf.Max(gridSize.y, coord.y + 1);
                }
            }
            return gridSize;
        }
        
        protected override IFlowLayoutNodeCreationConstraint CreateNodeCreationConstraint(FlowDomainExtensions domainExtensions, FlowLayoutGraph graph)
        {
            if (graph != null)
            {
                if (positionConstraintMode == NodeConstraintType.Script)
                {
                    // Try to instantiate the script
                    var scriptInstance = instanceCache.GetInstance(nodePositionConstraintScriptClassName) as IGridFlowLayoutNodePositionConstraint;
                    if (scriptInstance != null)
                    {
                        var gridSize = FindGridSize(graph);
                        return new GridFlowLayoutNodeConstraintProcessorScript(scriptInstance, gridSize);
                    }
                }
                else if (positionConstraintMode == NodeConstraintType.StartEndNode)
                {
                    return new GridFlowLayoutNodeConstraintProcessorStartEnd(startNodePositionConstraints, endNodePositionConstraints);
                }
            }
            return new NullFlowLayoutNodeCreationConstraint();
        }
    }
}

