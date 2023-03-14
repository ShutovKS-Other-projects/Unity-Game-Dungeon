//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Utils;
using System.Linq;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.GridFlow.Constraints
{
    public class GridFlowLayoutNodeConstraintProcessorStartEnd : IFlowLayoutNodeCreationConstraint
    {
        readonly Vector2Int[] startPositions;
        readonly Vector2Int[] endPositions;

        public GridFlowLayoutNodeConstraintProcessorStartEnd(Vector2Int[] startPositions, Vector2Int[] endPositions)
        {
            this.startPositions = startPositions;
            this.endPositions = endPositions;
        }

        public bool CanCreateNodeAt(FlowLayoutGraphNode node, int totalPathLength, int currentPathPosition)
        {
            if (currentPathPosition == 0)
            {
                // Start Node
                if (startPositions != null && startPositions.Length > 0)
                {
                    var coord = MathUtils.RoundToVector2Int(node.coord);
                    return startPositions.Contains(coord);
                }
            }
            else if (currentPathPosition == totalPathLength - 1)
            {
                // End Node
                if (endPositions != null && endPositions.Length > 0)
                {
                    var coord = MathUtils.RoundToVector2Int(node.coord);
                    return endPositions.Contains(coord);
                }
            }

            return true;
        }
    }
}