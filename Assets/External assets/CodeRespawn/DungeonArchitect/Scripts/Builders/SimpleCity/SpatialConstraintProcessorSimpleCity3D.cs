//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect.Builders.SimpleCity.SpatialConstraints
{
    public class SpatialConstraintProcessorSimpleCity3D : SpatialConstraintProcessor
    {
        public override SpatialConstraintRuleDomain GetDomain(SpatialConstraintProcessorContext context)
        {
            var cityConfig = context.config as SimpleCityDungeonConfig;
            var cellSize = cityConfig.CellSize;

            var domain = base.GetDomain(context);
            domain.gridSize = new Vector3(cellSize.x, 0, cellSize.y);
            return domain;
        }
    }
}
