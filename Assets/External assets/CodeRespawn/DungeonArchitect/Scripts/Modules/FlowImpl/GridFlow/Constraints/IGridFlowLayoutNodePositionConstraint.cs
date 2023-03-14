//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    public interface IGridFlowLayoutNodePositionConstraint
    {
        bool CanCreateNodeAt(int currentPathPosition, int totalPathLength, Vector2Int nodeCoord, Vector2Int gridSize);
    }
}