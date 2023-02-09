//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Graphs.Layouts.Layered
{
    class LayoutTreeNode<T>
    {
        public GraphLayoutNode<T> GraphNode;
        public float X;
        public int Depth;
        public float Mod;

        public LayoutTreeNode<T> Parent;
        public List<LayoutTreeNode<T>> Children = new List<LayoutTreeNode<T>>();
    }

    class LayoutTree<T>
    {
        public LayoutTreeNode<T> root;
        public List<LayoutTreeNode<T>> nodes = new List<LayoutTreeNode<T>>();
    }

    [System.Serializable]
    public class GraphLayoutLayeredConfig
    {
        [SerializeField]
        public Vector2 separation = new Vector2(130, 100);
    }

    public class GraphLayoutLayered<T> : GraphLayoutBase<T>
    {
        GraphLayoutLayeredConfig config;
        public GraphLayoutLayered(GraphLayoutLayeredConfig config)
        {
            this.config = config;
        }

        LayoutTreeNode<T> BuildTreeNode(LayoutTree<T> tree, LayoutTreeNode<T> parent, GraphLayoutNode<T> graphNode, HashSet<GraphLayoutNode<T>> visited)
        {
            visited.Add(graphNode);

            var treeNode = new LayoutTreeNode<T>();
            treeNode.GraphNode = graphNode;
            treeNode.Parent = parent;
            tree.nodes.Add(treeNode);

            foreach (var outgoingGraphNode in graphNode.Outgoing)
            {
                if (!visited.Contains(outgoingGraphNode))
                {
                    var childTreeNode = BuildTreeNode(tree, treeNode, outgoingGraphNode, visited);
                    treeNode.Children.Add(childTreeNode);
                }
            }

            return treeNode;
        }

        LayoutTree<T> BuildTree(GraphLayoutNode<T>[] nodes)
        {
            var tree = new LayoutTree<T>();

            // Find the root node
            if (nodes.Length == 0)
            {
                return tree;
            }

            var startNode = nodes[0];
            {
                var visited = new HashSet<GraphLayoutNode<T>>();
                while (startNode.Incoming.Count > 0)
                {
                    if (visited.Contains(startNode))
                    {
                        break;
                    }
                    visited.Add(startNode);

                    startNode = startNode.Incoming[0];
                }
            }

            {
                var visited = new HashSet<GraphLayoutNode<T>>();
                tree.root = BuildTreeNode(tree, null, startNode, visited);
            }
            return tree;
        }

        void TagNodeLevels(LayoutTreeNode<T> node, int depth)
        {
            node.Depth = depth;
            foreach (var child in node.Children)
            {
                TagNodeLevels(child, depth + 1);
            }
        }

        void CalculateInitialX(LayoutTreeNode<T> Node, LayoutTreeNode<T> LeftSibling)
        {
            LayoutTreeNode<T> LeftChild = null;
            foreach (LayoutTreeNode<T> Child in Node.Children)
            {
                CalculateInitialX(Child, LeftChild);
                LeftChild = Child;
            }

            bool bIsLeftMost = LeftSibling == null;
            bool bIsLeaf = (Node.Children.Count == 0);

            if (bIsLeaf)
            {
                if (bIsLeftMost)
                {
                    Node.X = 0;
                }
                else
                {
                    Node.X = LeftSibling.X + 1;
                }
            }
            else if (Node.Children.Count == 1)
            {
                if (bIsLeftMost)
                {
                    Node.X = Node.Children[0].X;
                }
                else
                {
                    Node.X = LeftSibling.X + 1;
                    Node.Mod = Node.X - Node.Children[0].X;
                }
            }
            else
            {
                float LeftX = Node.Children[0].X;
                float RightX = Node.Children[Node.Children.Count - 1].X;
                float MidX = (LeftX + RightX) / 2.0f;

                if (bIsLeftMost)
                {
                    Node.X = MidX;
                }
                else
                {
                    Node.X = LeftSibling.X + 1;
                    Node.Mod = Node.X - MidX;
                }
            }

            if (!bIsLeaf && !bIsLeftMost)
            {
                ResolveConflicts(Node);
            }
        }


        void ResolveConflicts(LayoutTreeNode<T> Node)
        {
            float ShiftValue = 0.0f;
            float MinDistance = 1.0f;

            var NodeContour = new Dictionary<int, float>();
            GetLeftContour(Node, 0, NodeContour);

            var NodeLevels = new List<int>(NodeContour.Keys);
            NodeLevels.Sort();

            LayoutTreeNode<T> Sibling = GetLeftMostSibling(Node);

            while (Sibling != null && Sibling != Node)
            {
                var SiblingContour = new Dictionary<int, float>();
                GetRightContour(Sibling, 0, SiblingContour);

                var SiblingLevels = new List<int>(SiblingContour.Keys);
                SiblingLevels.Sort();

                int MaxNodeLevel = NodeLevels[NodeLevels.Count - 1];
                int MaxSiblingLevel = SiblingLevels[SiblingLevels.Count - 1];

                int StartLevel = Node.Depth + 1;
                int EndLevel = Mathf.Min(MaxNodeLevel, MaxSiblingLevel);
                for (int Level = StartLevel; Level <= EndLevel; Level++)
                {
                    float Distance = NodeContour[Level] - SiblingContour[Level];
                    if (Distance + ShiftValue < MinDistance)
                    {
                        ShiftValue = MinDistance - Distance;
                    }
                }

                if (ShiftValue > 0)
                {
                    Node.X += ShiftValue;
                    Node.Mod += ShiftValue;

                    ShiftValue = 0;
                }

                Sibling = GetNextSibling(Sibling);
            }
        }


        void GetLeftContour(LayoutTreeNode<T> Node, float ModSum, Dictionary<int, float> ContourMap)
        {
            if (!ContourMap.ContainsKey(Node.Depth))
            {
                ContourMap.Add(Node.Depth, Node.X + ModSum);
            }
            else
            {
                ContourMap[Node.Depth] = Mathf.Min(ContourMap[Node.Depth], Node.X + ModSum);
            }

            foreach (var Child in Node.Children)
            {
                GetLeftContour(Child, ModSum + Node.Mod, ContourMap);
            }
        }

        void GetRightContour(LayoutTreeNode<T> Node, float ModSum, Dictionary<int, float> ContourMap)
        {
            if (!ContourMap.ContainsKey(Node.Depth))
            {
                ContourMap.Add(Node.Depth, Node.X + ModSum);
            }
            else
            {
                ContourMap[Node.Depth] = Mathf.Max(ContourMap[Node.Depth], Node.X + ModSum);
            }

            foreach (var Child in Node.Children)
            {
                GetRightContour(Child, ModSum + Node.Mod, ContourMap);
            }
        }

        LayoutTreeNode<T> GetLeftMostSibling(LayoutTreeNode<T> Node)
        {
            if (Node == null || Node.Parent == null)
            {
                return null;
            }

            return Node.Parent.Children[0];
        }

        LayoutTreeNode<T> GetNextSibling(LayoutTreeNode<T> Node)
        {
            if (Node == null || Node.Parent == null)
            {
                return null;
            }

            int NodeIdx = Node.Parent.Children.IndexOf(Node);
            if (NodeIdx == -1 || NodeIdx == Node.Parent.Children.Count - 1)
            {
                return null;
            }

            return Node.Parent.Children[NodeIdx + 1];
        }

        void CalculateFinalX(LayoutTreeNode<T> Node, float TotalMod)
        {
            Node.X += TotalMod;

            foreach (var Child in Node.Children)
            {
                CalculateFinalX(Child, TotalMod + Node.Mod);
            }
        }

        protected override void LayoutImpl(GraphLayoutNode<T>[] nodes)
        {
            var tree = BuildTree(nodes);

            TagNodeLevels(tree.root, 0);
            CalculateInitialX(tree.root, null);
            CalculateFinalX(tree.root, 0);

            float depthDistance = config.separation.x;
            float siblingDistance = config.separation.y;

            foreach (var node in tree.nodes)
            {
                var position = new Vector2(
                    depthDistance * node.Depth,
                    siblingDistance * node.X);

                node.GraphNode.Position = position;
            }
        }
    }
}
