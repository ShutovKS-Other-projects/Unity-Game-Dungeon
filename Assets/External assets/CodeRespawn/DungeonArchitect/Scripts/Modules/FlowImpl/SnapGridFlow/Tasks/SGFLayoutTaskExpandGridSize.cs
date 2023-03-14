//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;
using UnityEditor;
using UnityEngine;
using MathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow.Tasks
{
    [FlowExecNodeInfo("Expand Grid", "Layout Graph/", 2000)]
    public class SGFLayoutTaskExpandGridSize : FlowExecTask
    {
        public int expandAlongX = 1;
        public int expandAlongY = 0;
        public int expandAlongZ = 1;
        

        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            var output = new FlowTaskExecOutput();
            if (input.IncomingTaskOutputs.Length == 0)
            {
                output.ErrorMessage = "Missing Input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            if (input.IncomingTaskOutputs.Length > 1)
            {
                output.ErrorMessage = "Only one input allowed";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            if (expandAlongX <= 0 && expandAlongY <= 0 && expandAlongZ <= 0)
            {
                output.ErrorMessage = "Invalid expansion size";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            output.State = input.CloneInputState();
            var graph = output.State.GetState<FlowLayoutGraph>();
            if (graph == null || graph.Nodes.Count == 0)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            // Find the min-max coords of the existing graph
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = -min;

            var nodes = new Dictionary<Vector3Int, FlowLayoutGraphNode>();
            foreach (var node in graph.Nodes)
            {
                if (node.MergedCompositeNodes.Count > 0)
                {
                    foreach (var subNode in node.MergedCompositeNodes)
                    {
                        nodes[MathUtils.RoundToVector3Int(subNode.coord)] = subNode;
                        
                        min = MathUtils.ComponentMin(min, subNode.coord);
                        max = MathUtils.ComponentMax(max, subNode.coord);

                    }
                }
                else
                {
                    nodes[MathUtils.RoundToVector3Int(node.coord)] = node;
                    
                    min = MathUtils.ComponentMin(min, node.coord);
                    max = MathUtils.ComponentMax(max, node.coord);
                }
            }

            var oldMin = min;
            var oldMax = max;
            min -= new Vector3(expandAlongX, expandAlongY, expandAlongZ);
            max += new Vector3(expandAlongX, expandAlongY, expandAlongZ);

            var coordMin = MathUtils.RoundToVector3Int(min);
            var coordMax = MathUtils.RoundToVector3Int(max);

            var coordOldMin = MathUtils.RoundToVector3Int(oldMin);
            var coordOldMax = MathUtils.RoundToVector3Int(oldMax);
            
            for (int z = coordMin.z; z <= coordMax.z; z++)
            {
                for (int y = coordMin.y; y <= coordMax.y; y++)
                {
                    for (int x = coordMin.x; x <= coordMax.x; x++)
                    {
                        FlowLayoutGraphNode node;
                        var coord = new Vector3Int(x, y, z);
                        if (nodes.ContainsKey(coord))
                        {
                            node = nodes[coord];
                        }
                        else
                        {
                            node = new FlowLayoutGraphNode();
                            node.position = new Vector3(x, y, z) * 4;
                            node.coord = new Vector3(x, y, z);
                            node.active = false;
                            nodes[coord] = node;
                            graph.AddNode(node);
                        }

                        if (x > coordMin.x)
                        {
                            if (x <= coordOldMin.x || x > coordOldMax.x)
                            {
                                var srcNode = nodes[new Vector3Int(x - 1, y, z)];
                                var dstNode = nodes[new Vector3Int(x, y, z)];
                                graph.MakeLink(srcNode, dstNode);
                            }
                        }

                        if (y > coordMin.y)
                        {
                            if (y <= coordOldMin.y || y > coordOldMax.y)
                            {
                                var srcNode = nodes[new Vector3Int(x, y - 1, z)];
                                var dstNode = nodes[new Vector3Int(x, y, z)];
                                graph.MakeLink(srcNode, dstNode);
                            }
                        }
                        
                        if (z > coordMin.z)
                        {
                            if (z <= coordOldMin.z || z > coordOldMax.z)
                            {
                                var srcNode = nodes[new Vector3Int(x, y, z - 1)];
                                var dstNode = nodes[new Vector3Int(x, y, z)];
                                graph.MakeLink(srcNode, dstNode);
                            }
                        }
                    }
                }
            }
            
            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }
    }
}