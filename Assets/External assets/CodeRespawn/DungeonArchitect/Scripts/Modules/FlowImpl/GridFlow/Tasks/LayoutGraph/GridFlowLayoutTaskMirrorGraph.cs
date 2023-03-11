//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Tasks;
using DungeonArchitect.Flow.Exec;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.GridFlow.Tasks
{
    [FlowExecNodeInfo("Mirror Graph", "Layout Graph/", 1050)]
    public class GridFlowLayoutTaskMirrorGraph  : LayoutBaseFlowTaskMirrorGraph
    {
        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            return base.Execute(context, input);
        }

        protected override Vector3 GetNodePosition(Vector3 coord, Vector3 coordMin, Vector3 coordMax)
        {
            var height = coordMax.y - coordMin.y;
            var position = new Vector2(coord.x, height - coord.y) * GridFlowConstants.LayoutNodeEditorSpacing;
            return position;
        }
    }
}