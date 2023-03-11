//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow.Tasks
{
    [FlowExecNodeInfo("Add Padding", "Layout Graph/", 3000)]
    public class SGFLayoutTaskAddPadding : FlowExecTask
    {
        public bool paddingAlongX = true;
        public bool paddingAlongY = false;
        public bool paddingAlongZ = true;
        public Color color = new Color(0.5f, 0.5f, 0.75f);
        public string[] categories = new string[0];
        
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

            output.State = input.CloneInputState();
            var graph = output.State.GetState<FlowLayoutGraph>();
            if (graph == null || graph.Nodes.Count == 0)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            
            
            var activeNodes = new Dictionary<Vector3Int, FlowLayoutGraphNode>();
            foreach (var node in graph.Nodes)
            {
                if (!node.active) continue;
                if (node.MergedCompositeNodes.Count > 0)
                {
                    foreach (var subNode in node.MergedCompositeNodes)
                    {
                        activeNodes[MathUtils.RoundToVector3Int(subNode.coord)] = subNode;
                    }
                }
                else
                {
                    activeNodes[MathUtils.RoundToVector3Int(node.coord)] = node;
                }
            }

            var paddingNodes = new List<FlowLayoutGraphNode>();
            
            foreach (var node in graph.Nodes)
            {
                if (!node.active)
                {
                    // Check if we have a surrounding active node
                    var coord = MathUtils.RoundToVector3Int(node.coord);
                    if (HasNeighbour(coord, activeNodes))
                    {
                        paddingNodes.Add(node);
                    }
                }
            }
            
            foreach (var node in paddingNodes)
            {
                node.active = true;
                node.color = color;
                
                var snapNodeData = node.GetDomainData<FlowLayoutNodeSnapDomainData>();
                snapNodeData.Categories = categories;
            }
            
            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }

        bool HasNeighbour(Vector3Int coord, Dictionary<Vector3Int, FlowLayoutGraphNode> activeNodes)
        {
            if (paddingAlongX)
            {
                if (activeNodes.ContainsKey(coord + new Vector3Int(1, 0, 0)) || activeNodes.ContainsKey(coord + new Vector3Int(-1, 0, 0)))
                {
                    return true;
                }
            }
            
            if (paddingAlongY)
            {
                if (activeNodes.ContainsKey(coord + new Vector3Int(0, 1, 0)) || activeNodes.ContainsKey(coord + new Vector3Int(0, -1, 0)))
                {
                    return true;
                }
            }

            if (paddingAlongZ)
            {
                if (activeNodes.ContainsKey(coord + new Vector3Int(0, 0, 1)) || activeNodes.ContainsKey(coord + new Vector3Int(0, 0, -1)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}