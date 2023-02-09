//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System;
using System.Collections.Generic;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.UI.Widgets.GraphEditors
{
    public class GraphOperations
    {
        /// <summary>
        /// Creates a new graph node of the specified type
        /// </summary>
        /// <typeparam name="T">The type of node to create. Should be a subclass of GraphNode</typeparam>
        /// <returns>The created graph node</returns>
        public static T CreateNode<T>(Graph graph, UIUndoSystem undo) where T : GraphNode
        {
            T node = ScriptableObject.CreateInstance<T>();
            InitializeCreatedNode(graph, node, undo);
            return node;
        }

        /// <summary>
        /// Creates a graph node of the specified type
        /// </summary>
        /// <param name="t">The type of node to create. Should be a subclass of GraphNode</param>
        /// <returns>The created graph node</returns>
        public static GraphNode CreateNode(Graph graph, Type t, UIUndoSystem undo)
        {
            GraphNode node = ScriptableObject.CreateInstance(t) as GraphNode;
            InitializeCreatedNode(graph, node, undo);
            return node;
        }

        private static void InitializeCreatedNode(Graph graph, GraphNode node, UIUndoSystem undo)
        {
			var id = System.Guid.NewGuid().ToString(); // graph.IndexCounter.GetNext();
            if (undo != null)
            {
                undo.RecordObject(graph, "Create Node");
            }

            node.Initialize(id, graph);

            if (undo != null)
            {
                undo.RegisterCreatedObjectUndo(node, "Create Node");
            }

            var pins = new List<GraphPin>();
            pins.AddRange(node.InputPins);
            pins.AddRange(node.OutputPins);

            foreach (var pin in pins)
            {
                if (undo != null)
                {
                    undo.RegisterCompleteObjectUndo(pin, "Create Node");
                }
            }

            graph.Nodes.Add(node);
        }

        /// <summary>
        /// Makes a deep copy of a node.  Called when a node is copy pasted
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="originalNode"></param>
        /// <returns></returns>
        public static T DuplicateNode<T>(Graph graph, T originalNode, UIUndoSystem undo) where T : GraphNode
        {
            var node = CreateNode(graph, originalNode.GetType(), undo);
            node.CopyFrom(originalNode);
            return node as T;
        }
        
        /// <summary>
        /// Destroys a node and removes all references of it from the graph model. Called when the node is deleted from the editor
        /// </summary>
        /// <param name="node"></param>
        public static void DestroyNode(GraphNode node, UIUndoSystem undo)
        {
            if (!node.CanBeDeleted)
            {
                // Cannot be deleted
                return;
            }

            var graph = node.Graph;
            if (undo != null)
            {
                undo.RegisterCompleteObjectUndo(graph, "Delete Node");
                undo.RegisterCompleteObjectUndo(node, "Delete node");
            }
            
            // Break link connections
            BreakInputLinks(node, undo);
            BreakOutputLinks(node, undo);

            // De-register from the graph
            graph.Nodes.Remove(node);

            // Destroy the pins
            var pins = new List<GraphPin>();
            pins.AddRange(node.InputPins);
            pins.AddRange(node.OutputPins);
            foreach (var pin in pins)
            {
                if (undo != null)
                {
                    undo.DestroyObjectImmediate(pin);
                }
            }

            graph.Nodes.Remove(node);

            if (undo != null)
            {
                undo.DestroyObjectImmediate(node);
            }
        }

        
        /// <summary>
        /// Destroys a node and removes all references of it from the graph model. Called when the node is deleted from the editor
        /// </summary>
        /// <param name="node"></param>
        public static void DestroyLink(GraphLink link, UIUndoSystem undo)
        {
            var graph = link.Graph;
            if (undo != null)
            {
                undo.RecordObject(graph, "Destroy Link");
            }
            graph.Links.Remove(link);

            if (undo != null)
            {
                undo.DestroyObjectImmediate(link);
            }
        }

        /// <summary>
        /// Breaks all links connected to the input pins
        /// </summary>
        public static void BreakInputLinks(GraphNode node, UIUndoSystem undo)
        {
            BreakLinks(node.InputPins, undo);
        }

        /// <summary>
        /// Breaks all links connected to the output pins
        /// </summary>
        public static void BreakOutputLinks(GraphNode node, UIUndoSystem undo)
        {
            BreakLinks(node.OutputPins, undo);
        }

        private static void BreakLinks(GraphPin[] pins, UIUndoSystem undo)
        {
            foreach (var pin in pins)
            {
                BreakLinks(pin, undo);
            }
        }

        // Breaks all the links attached to the pin
        private static void BreakLinks(GraphPin pin, UIUndoSystem undo)
        {
            GraphLink[] links = pin.GetConntectedLinks();
            foreach (var link in links)
            {
                DestroyLink(link, undo);
            }
        }

        
        /// <summary>
        /// Creates a link of the specified type
        /// </summary>
        /// <typeparam name="T">The type of the link. Should be GraphLink or one of its subclass</typeparam>
        /// <returns></returns>
        public static T CreateLink<T>(Graph graph) where T : GraphLink
        {
            T link = ScriptableObject.CreateInstance<T>();
            link.Id = graph.IndexCounter.GetNext();
            link.Graph = graph;
            graph.Links.Add(link);
            return link;
        }


    }


    class GraphInputHandler
    {
        /// <summary>
        /// Handles user input (keyboard and mouse)
        /// </summary>
        /// <param name="e">Input event</param>
        /// <param name="camera">Graph camera to convert to / from screen to world coordinates</param>
        /// <returns>true if the input was processed, false otherwise.</returns>
        public static bool HandleNodeInput(GraphNode node, Event e, GraphEditor graphEditor, UISystem uiSystem)
        {
            bool inputProcessed = false;
            if (!node.Dragging)
            {
                // let the pins handle the input first
                foreach (var pin in node.InputPins)
                {
                    if (inputProcessed) break;
                    inputProcessed |= HandlePinInput(pin, e, graphEditor, uiSystem);
                }
                foreach (var pin in node.OutputPins)
                {
                    if (inputProcessed) break;
                    inputProcessed |= HandlePinInput(pin, e, graphEditor, uiSystem);
                }
            }

            var mousePosition = e.mousePosition;
            var mousePositionWorld = graphEditor.Camera.ScreenToWorld(mousePosition);
            int dragButton = 0;
            // If the pins didn't already handle the input, then let the node handle it
            if (!inputProcessed)
            {
                bool insideRect = node.Bounds.Contains(mousePositionWorld);
                if (e.type == EventType.MouseDown && insideRect && e.button == dragButton)
                {
                    node.Dragging = true;
                    inputProcessed = true;
                }
                else if (e.type == EventType.MouseUp && insideRect && e.button == dragButton)
                {
                    node.Dragging = false;
                }
            }

            if (node.Dragging && !node.Selected)
            {
                node.Dragging = false;
            }

            if (node.Dragging && e.type == EventType.MouseDrag)
            {
                inputProcessed = true;
            }

            return inputProcessed;
        }


        /// <summary>
        /// Handles the mouse input and returns true if handled
        /// </summary>
        public static bool HandlePinInput(GraphPin pin, Event e, GraphEditor graphEditor, UISystem uiSystem)
        {
            var camera = graphEditor.Camera;
            var mousePosition = e.mousePosition;
            var mousePositionWorld = camera.ScreenToWorld(mousePosition);
            int buttonIdDrag = 0;
            int buttonIdDestroyLinks = 1;
            if (pin.ContainsPoint(mousePositionWorld))
            {
                if (e.type == EventType.MouseDown && e.button == buttonIdDrag)
                {
                    pin.ClickState = GraphPinMouseState.Clicked;
                    return true;
                }

                if (e.button == buttonIdDestroyLinks)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        pin.RequestLinkDeletionInitiated = true;
                    }
                    else if (e.type == EventType.MouseDrag)
                    {
                        pin.RequestLinkDeletionInitiated = false;
                    }
                    else if (e.type == EventType.MouseUp)
                    {
                        if (pin.RequestLinkDeletionInitiated)
                        {
                            DestroyPinLinks(pin, uiSystem.Undo);
                            if (pin.Node != null && pin.Node.Graph != null)
                            {
                                graphEditor.HandleGraphStateChanged(uiSystem);
                            }
                        }
                    }
                    return true;
                }

                if (pin.ClickState != GraphPinMouseState.Clicked)
                {
                    pin.ClickState = GraphPinMouseState.Hover;
                }
            }
            else
            {
                pin.ClickState = GraphPinMouseState.None;
            }

            return false;
        }

        /// <summary>
        /// Destroys all links connected to this pin
        /// </summary>
        private static void DestroyPinLinks(GraphPin pin, UIUndoSystem undo)
        {
            var pinLinks = pin.GetConntectedLinks();
            foreach (var link in pinLinks)
            {
                GraphOperations.DestroyLink(link, undo);
            }

            pin.NotifyPinLinksDestroyed();
        }

    }
}