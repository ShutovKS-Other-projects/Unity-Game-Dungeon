//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.RuntimeGraphs
{
    public class RuntimeGraph<T>
    {
        public List<RuntimeGraphNode<T>> Nodes = new List<RuntimeGraphNode<T>>();

        public void RemoveNode(RuntimeGraphNode<T> node)
        {
            if (node.Graph == this)
            {
                node.BreakAllLinks();
                Nodes.Remove(node);
            }
            else
            {
                Debug.LogWarning("Remove node from an invalid graph");
            }
        }
    }

    public class RuntimeGraphNode<T>
    {
        public T Payload = default(T);
        public RuntimeGraph<T> Graph;
        public Vector2 Position = Vector2.zero;

        public List<RuntimeGraphNode<T>> Outgoing = new List<RuntimeGraphNode<T>>();
        public List<RuntimeGraphNode<T>> Incoming = new List<RuntimeGraphNode<T>>();

        public RuntimeGraphNode(RuntimeGraph<T> graph)
        {
            this.Graph = graph;
        }


        public void MakeLinkTo(RuntimeGraphNode<T> destNode)
        {
            Outgoing.Add(destNode);
            destNode.Incoming.Add(this);
        }

        public void BreakLinkTo(RuntimeGraphNode<T> destNode)
        {
            Outgoing.Remove(destNode);
            destNode.Incoming.Remove(this);
        }

        public void BreakAllOutgoingLinks()
        {
            var outgoingNodes = new List<RuntimeGraphNode<T>>(Outgoing);
            foreach (var outgoingNode in outgoingNodes)
            {
                BreakLinkTo(outgoingNode);
            }
        }

        public void BreakAllIncomingLinks()
        {
            var incomingNodes = new List<RuntimeGraphNode<T>>(Incoming);
            foreach (var incomingNode in incomingNodes)
            {
                incomingNode.BreakLinkTo(this);
            }
        }

        public void BreakAllLinks()
        {
            BreakAllOutgoingLinks();
            BreakAllIncomingLinks();
        }

        public override string ToString()
        {
            if (Payload != null)
            {
                return Payload.ToString();
            }
            return base.ToString();
        }
    }
}
