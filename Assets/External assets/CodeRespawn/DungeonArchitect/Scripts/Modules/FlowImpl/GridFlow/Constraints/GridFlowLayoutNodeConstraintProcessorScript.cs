//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.GridFlow.Constraints
{
    public class GridFlowLayoutNodeConstraintProcessorScript : IFlowLayoutNodeCreationConstraint
    {
        private readonly IGridFlowLayoutNodePositionConstraint scriptConstraint;
        private readonly Vector2Int gridSize;

        public GridFlowLayoutNodeConstraintProcessorScript(IGridFlowLayoutNodePositionConstraint scriptConstraint, Vector2Int gridSize)
        {
            this.scriptConstraint = scriptConstraint;
            this.gridSize = gridSize;
        }
        
        public bool CanCreateNodeAt(FlowLayoutGraphNode node, int totalPathLength, int currentPathPosition)
        {
            if (scriptConstraint == null || node == null)
            {
                // Ignore
                return true;
            }

            var nodeCoord = MathUtils.RoundToVector2Int(node.coord);
            return scriptConstraint.CanCreateNodeAt(currentPathPosition, totalPathLength, nodeCoord, gridSize);
        }
    }
}