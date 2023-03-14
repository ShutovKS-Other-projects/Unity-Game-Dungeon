//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.SpatialConstraints;
using DungeonArchitect.Builders.GridFlow;

namespace DungeonArchitect.Builders.Grid.SpatialConstraints
{
    public class SpatialConstraintProcessorGridFlow3D : SpatialConstraintProcessor
    {
        public override SpatialConstraintRuleDomain GetDomain(SpatialConstraintProcessorContext context)
        {
            var gridConfig = context.config as GridFlowDungeonConfig;

            var domain = base.GetDomain(context);
            domain.gridSize = gridConfig.gridSize;
            return domain;
        }
    }
}
