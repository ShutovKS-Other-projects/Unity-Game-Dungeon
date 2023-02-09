//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph2D
{
    public class FlowLayoutToolGraph2DNode : GraphNode
    {
        public FlowLayoutGraphNode LayoutNode { get; set; }

        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);
            Size = new Vector2(54, 54);

            var pinPosition = Size * 0.5f;
            // Create an input pin on the top
            CreatePinOfType<FlowLayoutToolGraph2DNodePin>(GraphPinType.Input,
                        pinPosition,
                        Rect.zero,
                        new Vector2(0, -1));

            // Create an output pin at the bottom
            CreatePinOfType<FlowLayoutToolGraph2DNodePin>(GraphPinType.Output,
                        pinPosition,
                        Rect.zero,
                        new Vector2(0, -1));

        }
    }
}
