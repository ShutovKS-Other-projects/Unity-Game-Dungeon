//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Grammar
{
    public interface IGrammarGraphBuilder 
    {
        int CreateNode(string nodeName, int executionIndex);
        bool LinkNodes(int nodeAId, int nodeBId);
    }

    public class GrammarGraphBuilder : IGrammarGraphBuilder
    {
        GrammarNodeType[] nodeTypes;
        GraphBuilder graphBuilder;
        int nodeIdCounter = 0;
        GrammarGraph grammarGraph;
        Dictionary<int, GraphNode> generatedNodes = new Dictionary<int, GraphNode>();

        public GrammarGraphBuilder(GrammarGraph grammarGraph, GrammarNodeType[] nodeTypes, GraphBuilder graphBuilder)
        {
            this.grammarGraph = grammarGraph;
            this.nodeTypes = nodeTypes;
            this.graphBuilder = graphBuilder;
        }

        public int CreateNode(string nodeName, int executionIndex)
        {
            // Find the node type that has this name
            GrammarNodeType targetNodeType = null;
            foreach (var nodeType in nodeTypes)
            {
                if (nodeType.nodeName == nodeName)
                {
                    targetNodeType = nodeType;
                    break;
                }
            }

            if (targetNodeType == null)
            {
                return -1;
            }

            var node = graphBuilder.CreateNode(typeof(GrammarTaskNode)) as GrammarTaskNode;
            node.NodeType = targetNodeType;
            node.executionIndex = executionIndex;

            int nodeId = ++nodeIdCounter;
            generatedNodes.Add(nodeId, node);
            return nodeId;
        }

        public bool LinkNodes(int nodeAId, int nodeBId)
        {
            if (nodeAId == nodeBId)
            {
                // Cannot link to the same node
                return false;
            }

            if (!generatedNodes.ContainsKey(nodeAId) || !generatedNodes.ContainsKey(nodeBId))
            {
                return false;
            }

            GraphNode nodeA = generatedNodes[nodeAId];
            GraphNode nodeB = generatedNodes[nodeBId];

            // Assumes that we link to the first pin of both the nodes
            if (nodeA.OutputPin == null || nodeB.InputPin == null)
            {
                return false;
            }

            var link = graphBuilder.LinkNodes<GraphLink>(nodeA.OutputPin, nodeB.InputPin);
            return (link != null);
        }

        public void ClearGraph()
        {
            var nodes = grammarGraph.Nodes.ToArray();
            foreach (var node in nodes)
            {
                if (node is CommentNode) continue;

                graphBuilder.DestroyNode(node);
            }
        }
    }

}
