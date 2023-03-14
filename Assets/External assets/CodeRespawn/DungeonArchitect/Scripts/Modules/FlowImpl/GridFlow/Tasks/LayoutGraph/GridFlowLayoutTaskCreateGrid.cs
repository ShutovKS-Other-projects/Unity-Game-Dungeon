//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.GridFlow.Tasks
{
    [FlowExecNodeInfo("Create Grid", "Layout Graph/", 1000)]
    public class GridFlowLayoutTaskCreateGrid : FlowExecTask
    {
        public Vector2Int resolution = new Vector2Int(6, 5);

        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            var graph = new FlowLayoutGraph();

            int width = resolution.x;
            int height = resolution.y;
            var nodes = new FlowLayoutGraphNode[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var node = new FlowLayoutGraphNode();
                    node.position = new Vector2(x, height - y - 1) * GridFlowConstants.LayoutNodeEditorSpacing;
                    node.coord = new Vector3(x, y, 0);
                    nodes[x, y] = node;

                    if (x > 0)
                    {
                        var srcNode = nodes[x - 1, y];
                        var dstNode = nodes[x, y];
                        graph.MakeLinkFastUnsafe(srcNode, dstNode);
                    }
                    if (y > 0)
                    {
                        var srcNode = nodes[x, y - 1];
                        var dstNode = nodes[x, y];
                        graph.MakeLinkFastUnsafe(srcNode, dstNode);
                    }

                    graph.AddNode(node);
                }
            }
            
            var output = new FlowTaskExecOutput();
            output.State.SetState(typeof(FlowLayoutGraph), graph);
            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }
    }
}
