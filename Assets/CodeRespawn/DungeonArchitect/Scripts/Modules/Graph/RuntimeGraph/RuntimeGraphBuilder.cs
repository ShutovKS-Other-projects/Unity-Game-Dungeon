//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonArchitect.RuntimeGraphs
{
    public class RuntimeGraphBuilderHandlers<T>
    {
        public System.Func<GraphNode, bool> CanCreateNode;
        public System.Action<GraphNode, RuntimeGraphNode<T>> NodeCreated;
        public System.Func<GraphNode, T> GetPayload;
    }

    public class RuntimeGraphBuilder
    {
        public static RuntimeGraphNode<T> AddNode<T>(GraphNode graphNode, RuntimeGraph<T> runtimeGraph, RuntimeGraphBuilderHandlers<T> handlers)
        {
            if (handlers.CanCreateNode(graphNode))
            {
                var runtimeNode = new RuntimeGraphNode<T>(runtimeGraph);
                runtimeNode.Payload = handlers.GetPayload(graphNode);
                runtimeNode.Position = graphNode.Position;
                runtimeGraph.Nodes.Add(runtimeNode);
                handlers.NodeCreated(graphNode, runtimeNode);
                return runtimeNode;
            }
            return null;
        }

        public static RuntimeGraphNode<T> AddNode<T>(T payload, RuntimeGraph<T> runtimeGraph)
        {
            var runtimeNode = new RuntimeGraphNode<T>(runtimeGraph);
            runtimeNode.Payload = payload;
            runtimeNode.Position = Vector2.zero;
            runtimeGraph.Nodes.Add(runtimeNode);
            return runtimeNode;
        }

        public static void Build<T>(Graph graph, RuntimeGraph<T> runtimeGraph, RuntimeGraphBuilderHandlers<T> handlers)
        {
            runtimeGraph.Nodes.Clear();

            // Create the nodes
            var mapping = new Dictionary<GraphNode, RuntimeGraphNode<T>>();
            foreach (var graphNode in graph.Nodes)
            {
                var runtimeNode = AddNode(graphNode, runtimeGraph, handlers);
                mapping.Add(graphNode, runtimeNode);
            }

            // Connect the links
            foreach (var link in graph.Links)
            {
                if (link == null) continue;
                var snode = link.Output ? link.Output.Node : null;
                var dnode = link.Input ? link.Input.Node : null;
                if (snode == null || dnode == null) continue;
                if (!mapping.ContainsKey(snode) || !mapping.ContainsKey(dnode)) continue;

                var sourceNode = mapping[snode];
                var destNode = mapping[dnode];

                sourceNode.MakeLinkTo(destNode);
            }
        }
    }
}
