//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Graphs;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph2D
{
    public class FlowLayoutToolGraph2DBuilder
    {
        public static void Build(FlowLayoutGraph layoutGraph, GraphBuilder graphBuilder)
        {
            if (graphBuilder == null)
            {
                return;
            }

            graphBuilder.DestroyAllNodes();

            if (layoutGraph == null)
            {
                return;
            }

            // Create the nodes
            var runtimeToPreviewMap = new Dictionary<DungeonUID, FlowLayoutToolGraph2DNode>();
            foreach (var runtimeNode in layoutGraph.Nodes)
            {
                var previewNode = graphBuilder.CreateNode<FlowLayoutToolGraph2DNode>();
                if (previewNode != null)
                {
                    previewNode.LayoutNode = runtimeNode;
                    previewNode.Position = runtimeNode.position;
                    runtimeToPreviewMap.Add(runtimeNode.nodeId, previewNode);
                }
            }

            foreach (var link in layoutGraph.Links)
            {
                var startNode = runtimeToPreviewMap[link.source];
                var endNode = runtimeToPreviewMap[link.destination];

                if (startNode != null && endNode != null)
                {
                    var outputPin = startNode.OutputPin;
                    var inputPin = endNode.InputPin;

                    if (outputPin != null && inputPin != null)
                    {
                        var previewLink = graphBuilder.LinkNodes<FlowLayoutToolGraph2DLink>(startNode.OutputPin, endNode.OutputPin);
                        previewLink.layoutLinkState = link.state;
                    }
                }

            }
        }
    }


    public class FlowLayoutToolGraph2DUtils
    {
        public static FlowItem[] GetAllItems(FlowLayoutToolGraph2D graph)
        {
            var items = new List<FlowItem>();
            foreach (var node in graph.Nodes)
            {
                var previewNode = node as FlowLayoutToolGraph2DNode;
                if (previewNode != null)
                {
                    foreach (var item in previewNode.LayoutNode.items)
                    {
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                }
            }

            foreach (var link in graph.Links)
            {
                var previewLink = link as FlowLayoutToolGraph2DLink;
                if (previewLink != null)
                {
                    foreach (var item in previewLink.layoutLinkState.items)
                    {
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                }
            }

            return items.ToArray();
        }

    }
}
