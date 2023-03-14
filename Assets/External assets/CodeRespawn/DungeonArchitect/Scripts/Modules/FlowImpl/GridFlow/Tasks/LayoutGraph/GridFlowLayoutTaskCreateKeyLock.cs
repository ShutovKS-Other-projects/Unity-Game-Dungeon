//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Tasks;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Items;

namespace DungeonArchitect.Flow.Impl.GridFlow.Tasks
{
    [FlowExecNodeInfo("Create Key Lock", "Layout Graph/", 1040)]
    public class GridFlowLayoutTaskCreateKeyLock : LayoutBaseFlowTaskCreateKeyLock
    {
        public TilemapItemPlacementSettings placementSettings = new TilemapItemPlacementSettings();

        protected override bool Validate(FlowTaskExecContext context, FlowTaskExecInput input, ref string errorMessage, ref FlowTaskExecutionResult executionResult)
        {
            if (!TilemapItemPlacementStrategyUtils.Validate(placementSettings, ref errorMessage))
            {
                executionResult = FlowTaskExecutionResult.FailHalt;
                return false;
            }

            return true;
        }

        protected override void ProcessKeyItem(FlowItem keyItem, FlowLayoutGraphNode keyNode, FlowLayoutGraphLink lockLink)
        {
            keyItem.SetDomainData(placementSettings.Clone() as TilemapItemPlacementSettings);
        }
    }
}