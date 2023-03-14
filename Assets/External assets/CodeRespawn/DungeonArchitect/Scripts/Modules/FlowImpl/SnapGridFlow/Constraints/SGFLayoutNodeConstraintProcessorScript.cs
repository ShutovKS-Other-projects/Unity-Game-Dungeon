//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow.Constraints
{
    public class SGFLayoutNodeConstraintProcessorScript : IFlowLayoutNodeCreationConstraint
    {
        private readonly ISGFLayoutNodePositionConstraint scriptConstraint;
        private readonly Vector3Int gridSize;

        public SGFLayoutNodeConstraintProcessorScript(ISGFLayoutNodePositionConstraint scriptConstraint, Vector3Int gridSize)
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

            var nodeCoord = MathUtils.RoundToVector3Int(node.coord);
            return scriptConstraint.CanCreateNodeAt(currentPathPosition, totalPathLength, nodeCoord, gridSize);
        }
    }
    
}