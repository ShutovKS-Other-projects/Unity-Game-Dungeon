//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tasks
{
    public abstract class LayoutBaseFlowTaskMirrorGraph : FlowExecTask
    {
        public LayoutBaseFlowTaskMirrorDirectionX mirrorX = LayoutBaseFlowTaskMirrorDirectionX.None;
        public LayoutBaseFlowTaskMirrorDirectionY mirrorY = LayoutBaseFlowTaskMirrorDirectionY.None;
        public LayoutBaseFlowTaskMirrorDirectionZ mirrorZ = LayoutBaseFlowTaskMirrorDirectionZ.None;
        
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
            
            if (graph == null)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            if (mirrorX == LayoutBaseFlowTaskMirrorDirectionX.Left)
            {
                Vector3 graphCoordMin, graphCoordMax;
                GetGraphBounds(graph, out graphCoordMin, out graphCoordMax);
                
                System.Func<Vector3, bool> funcShouldMirror = (coord) => !Mathf.Approximately(coord.x, graphCoordMin.x);
                System.Func<Vector3, Vector3> funcGetMirrorCoord = (coord) => new Vector3(graphCoordMin.x - (coord.x - graphCoordMin.x), coord.y, coord.z);
                MirrorGraph(graph, funcGetMirrorCoord, funcShouldMirror, graphCoordMin, graphCoordMax);
            }
            else if (mirrorX == LayoutBaseFlowTaskMirrorDirectionX.Right)
            {
                Vector3 graphCoordMin, graphCoordMax;
                GetGraphBounds(graph, out graphCoordMin, out graphCoordMax);
                
                System.Func<Vector3, bool> funcShouldMirror = (coord) => !Mathf.Approximately(coord.x, graphCoordMax.x);
                System.Func<Vector3, Vector3> funcGetMirrorCoord = (coord) => new Vector3(graphCoordMax.x + (graphCoordMax.x - coord.x), coord.y, coord.z);
                MirrorGraph(graph, funcGetMirrorCoord, funcShouldMirror, graphCoordMin, graphCoordMax);
            }
            
            if (mirrorY == LayoutBaseFlowTaskMirrorDirectionY.Down)
            {
                Vector3 graphCoordMin, graphCoordMax;
                GetGraphBounds(graph, out graphCoordMin, out graphCoordMax);

                System.Func<Vector3, bool> funcShouldMirror = (coord) => !Mathf.Approximately(coord.y, graphCoordMin.y);
                System.Func<Vector3, Vector3> funcGetMirrorCoord = (coord) => new Vector3(coord.x, graphCoordMin.y - (coord.y - graphCoordMin.y), coord.z);
                MirrorGraph(graph, funcGetMirrorCoord, funcShouldMirror, graphCoordMin, graphCoordMax);
            }
            else if (mirrorY == LayoutBaseFlowTaskMirrorDirectionY.Up)
            {
                Vector3 graphCoordMin, graphCoordMax;
                GetGraphBounds(graph, out graphCoordMin, out graphCoordMax);

                System.Func<Vector3, bool> funcShouldMirror = (coord) => !Mathf.Approximately(coord.y, graphCoordMax.y);
                System.Func<Vector3, Vector3> funcGetMirrorCoord = (coord) => new Vector3(coord.x, graphCoordMax.y + (graphCoordMax.y - coord.y), coord.z);
                MirrorGraph(graph, funcGetMirrorCoord, funcShouldMirror, graphCoordMin, graphCoordMax);
            }

            if (mirrorZ == LayoutBaseFlowTaskMirrorDirectionZ.Back)
            {
                Vector3 graphCoordMin, graphCoordMax;
                GetGraphBounds(graph, out graphCoordMin, out graphCoordMax);

                System.Func<Vector3, bool> funcShouldMirror = (coord) => !Mathf.Approximately(coord.z, graphCoordMin.z);
                System.Func<Vector3, Vector3> funcGetMirrorCoord = (coord) => new Vector3(coord.x, coord.y, graphCoordMin.z - (coord.z - graphCoordMin.z));
                MirrorGraph(graph, funcGetMirrorCoord, funcShouldMirror, graphCoordMin, graphCoordMax);
            }
            else if (mirrorZ == LayoutBaseFlowTaskMirrorDirectionZ.Front)
            {
                Vector3 graphCoordMin, graphCoordMax;
                GetGraphBounds(graph, out graphCoordMin, out graphCoordMax);

                System.Func<Vector3, bool> funcShouldMirror = (coord) => !Mathf.Approximately(coord.z, graphCoordMax.z);
                System.Func<Vector3, Vector3> funcGetMirrorCoord = (coord) => new Vector3(coord.x, coord.y, graphCoordMax.z + (graphCoordMax.z - coord.z));
                MirrorGraph(graph, funcGetMirrorCoord, funcShouldMirror, graphCoordMin, graphCoordMax);
            }

            // Fix the coords so they start with 0,0,0
            FixNodeCoords(graph);
            
            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }

        void FixNodeCoords(FlowLayoutGraph graph)
        {
            Vector3 graphCoordMin, graphCoordMax;
            GetGraphBounds(graph, out graphCoordMin, out graphCoordMax);
            var offset = -graphCoordMin;
            foreach (var node in graph.Nodes)
            {
                node.coord += offset;
                node.position = GetNodePosition(node.coord, graphCoordMin, graphCoordMax);
            }
        }
        

        protected abstract Vector3 GetNodePosition(Vector3 coord, Vector3 coordMin, Vector3 coordMax);
        
        void MirrorGraph(FlowLayoutGraph graph, System.Func<Vector3, Vector3> funcGetMirrorCoord, System.Func<Vector3, bool> funcShouldMirror,
            Vector3 graphCoordMin, Vector3 graphCoordMax)
        {
            var sourceToMirrorNodes = new Dictionary<FlowLayoutGraphNode, FlowLayoutGraphNode>();
            var sourceToMirrorSubNodes = new Dictionary<DungeonUID, DungeonUID>();
            var nodesCopy = graph.Nodes.ToArray();
            foreach (var sourceNode in nodesCopy)
            {
                FlowLayoutGraphNode mirroredNode;
                if (funcShouldMirror(sourceNode.coord))
                {
                    mirroredNode = sourceNode.Clone();
                    mirroredNode.nodeId = DungeonUID.NewUID();
                    mirroredNode.coord = funcGetMirrorCoord(sourceNode.coord);
                    mirroredNode.position = GetNodePosition(mirroredNode.coord, graphCoordMin, graphCoordMax);
                    graph.AddNode(mirroredNode);
                }
                else
                {
                    mirroredNode = sourceNode;
                }

                for (int i = 0; i < sourceNode.MergedCompositeNodes.Count; i++)
                {
                    if (mirroredNode != sourceNode)
                    {
                        mirroredNode.MergedCompositeNodes[i].nodeId = DungeonUID.NewUID();
                    }
                    
                    var sourceSubNodeID = sourceNode.MergedCompositeNodes[i].nodeId;
                    var mirrorSubNodeID = mirroredNode.MergedCompositeNodes[i].nodeId;
                    sourceToMirrorSubNodes[sourceSubNodeID] = mirrorSubNodeID;
                }
                
                sourceToMirrorNodes[sourceNode] = mirroredNode;
            }
            
            var graphQuery = new FlowLayoutGraphQuery(graph);   // Note: Will query only with the source links since we haven't added the mirrored links yet
            var linksCopy = graph.Links.ToArray();
            foreach (var sourceLink in linksCopy)
            {
                // clone only if the start or end nodes have been mirrored
                var sourceLinkSrc = graphQuery.GetNode(sourceLink.source);
                var sourceLinkDst = graphQuery.GetNode(sourceLink.destination);

                if (funcShouldMirror(sourceLinkSrc.coord) || funcShouldMirror(sourceLinkDst.coord))
                {
                    if (sourceToMirrorNodes.ContainsKey(sourceLinkSrc) && sourceToMirrorNodes.ContainsKey(sourceLinkDst))
                    {
                        var mirrorLinkSrc = sourceToMirrorNodes[sourceLinkSrc];
                        var mirrorLinkDst = sourceToMirrorNodes[sourceLinkDst];
                        var mirrorLink = graph.MakeLink(mirrorLinkSrc, mirrorLinkDst);
                        mirrorLink.state = sourceLink.state.Clone();
                        if (sourceToMirrorSubNodes.ContainsKey(sourceLink.sourceSubNode) && sourceToMirrorSubNodes.ContainsKey(sourceLink.destinationSubNode))
                        {
                            mirrorLink.sourceSubNode = sourceToMirrorSubNodes[sourceLink.sourceSubNode];
                            mirrorLink.destinationSubNode = sourceToMirrorSubNodes[sourceLink.destinationSubNode];
                        }
                    }
                }
            }
        }
        
        
        bool GetGraphBounds(FlowLayoutGraph graph, out Vector3 graphCoordMin, out Vector3 graphCoordMax)
        {
            if (graph == null || graph.Nodes.Count == 0)
            {
                graphCoordMin = Vector3.zero;
                graphCoordMax = Vector3.zero;
                return false;
            }
            
            graphCoordMin = graphCoordMax = graph.Nodes[0].coord;

            foreach (var node in graph.Nodes)
            {
                graphCoordMin = MathUtils.ComponentMin(graphCoordMin, node.coord);
                graphCoordMax = MathUtils.ComponentMax(graphCoordMax, node.coord);
            }
            
            return true;
        }

    }
    
    
    public enum LayoutBaseFlowTaskMirrorDirectionX
    { 
        None,
        Left, 
        Right
    }
    
    public enum LayoutBaseFlowTaskMirrorDirectionY
    {
        None, 
        Up, 
        Down
    }
    
    public enum LayoutBaseFlowTaskMirrorDirectionZ
    { 
        None,
        Front, 
        Back
    }


}