//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Linq;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow.Constraints
{
    public class SGFLayoutNodeConstraintProcessorStartEnd : IFlowLayoutNodeCreationConstraint
    {
        readonly Vector3Int[] startPositions;
        readonly Vector3Int[] endPositions;

        public SGFLayoutNodeConstraintProcessorStartEnd(Vector3Int[] startPositions, Vector3Int[] endPositions)
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
                    var coord = MathUtils.RoundToVector3Int(node.coord);
                    return startPositions.Contains(coord);
                }
            }
            else if (currentPathPosition == totalPathLength - 1)
            {
                // End Node
                if (endPositions != null && endPositions.Length > 0)
                {
                    var coord = MathUtils.RoundToVector3Int(node.coord);
                    return endPositions.Contains(coord);
                }
            }

            return true;
        }
    }
}