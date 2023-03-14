//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect.Builders.Grid.SpatialConstraints
{
    public class SpatialConstraintProcessorGrid3D : SpatialConstraintProcessor
    {
        public override SpatialConstraintRuleDomain GetDomain(SpatialConstraintProcessorContext context)
        {
            var gridConfig = context.config as GridDungeonConfig;

            var domain = base.GetDomain(context);
            domain.gridSize = gridConfig.GridCellSize;
            return domain;
        }
    }
}
