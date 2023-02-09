//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Visibility
{
    public class VisibilityGraph
    {
        public int VisibilityDepth { get; set; } = 1;

        private List<VisibilityGraphNode> nodes = new List<VisibilityGraphNode>();

        struct VisibilitySearchState
        {
            public VisibilityGraphNode Node;
            public int Depth;
        }
        
        public void RegisterNode(VisibilityGraphNode node)
        {
            Debug.Assert(!nodes.Contains(node));
            nodes.Add(node);
            node.Initialize();
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public void UpdateVisibility(Vector3[] trackedObjects)
        {
            var visibleNodes = GetVisibleNodes(trackedObjects);
            foreach (var node in nodes)
            {
                bool visible = visibleNodes.Contains(node);
                node.SetVisible(visible);
            }
        }

        
        private HashSet<VisibilityGraphNode> GetVisibleNodes(Vector3[] trackedObjects)
        {
            var visibleNodes = new HashSet<VisibilityGraphNode>();
            if (trackedObjects.Length == 0)
            {
                return visibleNodes;
            }

            var startNodes = new HashSet<VisibilityGraphNode>();
            foreach (var trackedObjectPosition in trackedObjects)
            {
                foreach (var node in nodes)
                {
                    if (node.Bounds.Contains(trackedObjectPosition))
                    {
                        startNodes.Add(node);
                        break;
                    }
                }
            }

            if (VisibilityDepth == 0)
            {
                return startNodes;
            }
            
            // Grow out from the start nodes
            var queue = new Queue<VisibilitySearchState>();
            foreach (var startNode in startNodes)
            {
                var state = new VisibilitySearchState();
                state.Node = startNode;
                state.Depth = 0;
                queue.Enqueue(state);
            }

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                visibleNodes.Add(state.Node);
                
                // Add the children
                if (state.Depth < VisibilityDepth)
                {
                    foreach (var childNode in state.Node.ConnectedNodes)
                    {
                        if (!visibleNodes.Contains(childNode))
                        {
                            var childState = new VisibilitySearchState();
                            childState.Node = childNode;
                            childState.Depth = state.Depth + 1;
                            queue.Enqueue(childState);
                        }
                    }
                }
            }

            return visibleNodes;
        }
    }
}
