//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using UnityEngine;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.Graphs
{
    public class EditorGraphBuilder : GraphBuilder
    {
        private Object assetObject;
        private UIPlatform platform;
        private UIUndoSystem undo;

        public EditorGraphBuilder(Graph graph, Object assetObject, UIPlatform platform, UIUndoSystem undo) 
            : base(graph)
        {
            this.assetObject = assetObject;
            this.platform = platform;
            this.undo = undo;
        }

        public override GraphNode CreateNode(System.Type nodeType)
        {
            var node = GraphOperations.CreateNode(graph, nodeType, undo);
            node.Position = Vector2.zero;
            if (assetObject != null)
            {
                GraphEditorUtils.AddToAsset(platform, assetObject, node);
            }
            return node;
        }

        public override void DestroyNode(GraphNode node)
        {
            GraphOperations.DestroyNode(node, undo);
        }

        public override TLink LinkNodes<TLink>(GraphPin outputPin, GraphPin inputPin)
        {
            // Make sure a link doesn't already exists
            foreach (var existingLink in graph.Links)
            {
                if (existingLink.Input == inputPin && existingLink.Output == outputPin)
                {
                    return null;
                }
            }

            TLink link = GraphOperations.CreateLink<TLink>(graph);
            link.Input = inputPin;
            link.Output = outputPin;

            if (assetObject != null)
            {
                GraphEditorUtils.AddToAsset(platform, assetObject, link);
            }
            return link;
        }

    }
}
