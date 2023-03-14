//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;


namespace DungeonArchitect.Graphs.Layouts
{
    public interface IGraphLayout<T>
    {
        void Layout(T[] nodes, IGraphLayoutNodeActions<T> nodeActions);
    }

    public interface IGraphLayoutNodeActions<T>
    {
        void SetNodePosition(T node, Vector2 position);
        Vector2 GetNodePosition(T node);
        T[] GetOutgoingNodes(T node);
    }

    public enum GraphLayoutType
    {
        Layered,
        Spring
    }

    public class GraphLayoutNode<T>
    {
        public T Payload { get; set; }
        public Vector2 Position { get; set; }
        public List<GraphLayoutNode<T>> Outgoing { get; private set; }
        public List<GraphLayoutNode<T>> Incoming { get; private set; }

        public GraphLayoutNode(T payload, Vector2 position)
        {
            Outgoing = new List<GraphLayoutNode<T>>();
            Incoming = new List<GraphLayoutNode<T>>();

            this.Payload = payload;
            this.Position = position;
        }
    }

    public abstract class GraphLayoutBase<T> : IGraphLayout<T>
    {
        public void Layout(T[] nodes, IGraphLayoutNodeActions<T> nodeActions)
        {
            if (nodeActions == null || nodes == null) { return; }

            var nodeToLayoutMap = new Dictionary<T, GraphLayoutNode<T>>();
            var layoutNodes = new GraphLayoutNode<T>[nodes.Length];

            // Create the nodes
            for (int i = 0; i < nodes.Length; i++) 
            {
                var node = nodes[i];
                var position = nodeActions.GetNodePosition(node);
                var layoutNode = new GraphLayoutNode<T>(node, position);
                layoutNodes[i] = layoutNode;

                nodeToLayoutMap.Add(node, layoutNode);
            }

            // Link the nodes
            foreach (var node in nodes)
            {
                var layoutNode = nodeToLayoutMap[node];
                T[] outgoingNodes = nodeActions.GetOutgoingNodes(node);
                foreach (var outgoingNode in outgoingNodes)
                {
                    if (!nodeToLayoutMap.ContainsKey(outgoingNode)) continue;
                    var layoutOutgoingNode = nodeToLayoutMap[outgoingNode];
                    layoutNode.Outgoing.Add(layoutOutgoingNode);
                    layoutOutgoingNode.Incoming.Add(layoutNode);
                }
            }

            LayoutImpl(layoutNodes);

            foreach (var layoutNode in layoutNodes)
            {
                nodeActions.SetNodePosition(layoutNode.Payload, layoutNode.Position);
            }
        }

        protected abstract void LayoutImpl(GraphLayoutNode<T>[] nodes);
    }

}
