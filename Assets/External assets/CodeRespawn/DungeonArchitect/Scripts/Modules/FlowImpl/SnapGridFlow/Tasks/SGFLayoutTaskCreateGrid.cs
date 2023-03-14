//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow.Tasks
{
    [FlowExecNodeInfo("Create Grid", "Layout Graph/", 1000)]
    public class SGFLayoutTaskCreateGrid : FlowExecTask
    {
        public Vector3Int resolution = new Vector3Int(6, 4, 5);

        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            var graph = new FlowLayoutGraph();

            int sizeX = resolution.x;
            int sizeY = resolution.y;
            int sizeZ = resolution.z;
            
            var nodes = new FlowLayoutGraphNode[sizeX, sizeY, sizeZ];
            for (int z = 0; z < sizeZ; z++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        var node = new FlowLayoutGraphNode();
                        node.position = new Vector3(x, y, z) * 4;
                        node.coord = new Vector3(x, y, z);
                        nodes[x, y, z] = node;

                        if (x > 0)
                        {
                            var srcNode = nodes[x - 1, y, z];
                            var dstNode = nodes[x, y, z];
                            graph.MakeLinkFastUnsafe(srcNode, dstNode);
                        }

                        if (y > 0)
                        {
                            var srcNode = nodes[x, y - 1, z];
                            var dstNode = nodes[x, y, z];
                            graph.MakeLinkFastUnsafe(srcNode, dstNode);
                        }
                        
                        if (z > 0)
                        {
                            var srcNode = nodes[x, y, z - 1];
                            var dstNode = nodes[x, y, z];
                            graph.MakeLinkFastUnsafe(srcNode, dstNode);
                        }
                        
                        graph.AddNode(node);
                    }
                }
            }

            var output = new FlowTaskExecOutput();
            output.State.SetState(typeof(FlowLayoutGraph), graph);
            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }
    }
}