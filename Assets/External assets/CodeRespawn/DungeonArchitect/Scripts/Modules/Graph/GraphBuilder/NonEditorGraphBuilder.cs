//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Graphs
{
    public class NonEditorGraphBuilder : GraphBuilder
    {
        public NonEditorGraphBuilder(Graph graph)
            : base(graph)
        {
        }

        public override GraphNode CreateNode(System.Type nodeType)
        {
            GraphNode node = ScriptableObject.CreateInstance(nodeType) as GraphNode;
            var id = System.Guid.NewGuid().ToString();
            node.Initialize(id, graph);
            node.Position = Vector2.zero;
            graph.Nodes.Add(node);
            return node;
        }

        public override TLink LinkNodes<TLink>(GraphPin outputPin, GraphPin inputPin) 
        {
            // Make sure a link doesn't already exists
            foreach (var existingLink in graph.Links)
            {
                if (existingLink != null && existingLink.Input == inputPin && existingLink.Output == outputPin)
                {
                    return null;
                }
            }

            TLink link = ScriptableObject.CreateInstance<TLink>();
            link.Id = graph.IndexCounter.GetNext();
            link.Graph = graph;
            link.Input = inputPin;
            link.Output = outputPin;
            graph.Links.Add(link);
            return link;
        }

        public override void DestroyNode(GraphNode node)
        {
            if (node == null || !node.CanBeDeleted)
            {
                return;
            }

            var graph = node.Graph;

            // Break link connections
            var linksToBreak = new List<GraphLink>();
            foreach (var link in graph.Links) {
                if (link != null && link.Input != null && link.Output != null)
                {
                    if (link.Input.Node == node || link.Output.Node == node)
                    {
                        linksToBreak.Add(link);
                    }
                }
            }

            foreach(var link in linksToBreak)
            {
                graph.Links.Remove(link);
                DestroyObject(link);
            }

            // Destroy the pins
            var pins = new List<GraphPin>();
            pins.AddRange(node.InputPins);
            pins.AddRange(node.OutputPins);
            foreach (var pin in pins)
            {
                DestroyObject(pin);
            }

            graph.Nodes.Remove(node);
            DestroyObject(node);
        }

        void DestroyObject(Object obj)
        {
            if (Application.isEditor)
            {
                Object.DestroyImmediate(obj);
            }
            else
            {
                Object.Destroy(obj);
            }
        }

    }
}
