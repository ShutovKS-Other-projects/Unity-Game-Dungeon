//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect.Builders.Maze
{
    public class MazeDungeonSpatialConstraints : SpatialConstraintProcessor
    {
        public override SpatialConstraintRuleDomain GetDomain(SpatialConstraintProcessorContext context)
        {
            var gridConfig = context.config as MazeDungeonConfig;

            var gridSize2D = (gridConfig != null) ? gridConfig.gridSize : Vector2.one;
            var gridSize = new Vector3(gridSize2D.x, 0, gridSize2D.y);
            
            var domain = base.GetDomain(context);
            domain.gridSize = gridSize;
            return domain;
        }
    }
}
