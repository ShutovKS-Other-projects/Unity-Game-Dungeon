//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.SxEngine.Utils;
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public class SxSceneGraph
    {
        public SxRootSceneNode RootNode { get; } = new SxRootSceneNode();

        public void IterateNodes(System.Action<ISxSceneNode> visit)
        {
            var stack = new Stack<ISxSceneNode>();
            stack.Push(RootNode);

            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top == null) continue;
                
                visit(top);
                
                foreach (var childNode in top.Children)
                {
                    stack.Push(childNode);
                }
            }
        }

        public void Remove(ISxSceneNode nodeToRemove)
        {
            var stack = new Stack<ISxSceneNode>();
            stack.Push(RootNode);
            
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top == null) continue;

                if (top.Children.Contains(nodeToRemove))
                {
                    top.RemoveChild(nodeToRemove);
                    break;
                }
            }
        }
    }

    public interface ISxSceneNode
    {
        SxTransform WorldTransform { get; set; }
        void Draw(SxRenderContext context, Matrix4x4 accumWorldTransform, SxRenderCommandList renderCommandList);
        void Tick(SxRenderContext context, float deltaTime);
        void Destroy();

        ISxSceneNode[] Children { get; }
        void AddChild(ISxSceneNode child);
        void RemoveChild(ISxSceneNode child);
        ISxSceneNode Parent { get; set; }
    }

    public abstract class SxSceneNodeBase : ISxSceneNode
    {
        public SxTransform WorldTransform { get; set; } = SxTransform.identity;
        public ISxSceneNode[] Children
        {
            get => children.ToArray();
        }
        private List<ISxSceneNode> children = new List<ISxSceneNode>();

        public abstract void Draw(SxRenderContext context, Matrix4x4 accumWorldTransform, SxRenderCommandList renderCommandList);
        public abstract void Tick(SxRenderContext context, float deltaTime);
        public abstract void Destroy();
        public void AddChild(ISxSceneNode child)
        {
            children.Add(child);
        }

        public void RemoveChild(ISxSceneNode child)
        {
            child.RemoveChild(child);
        }

        public ISxSceneNode Parent { get; set; }

        public void RemoveAllChildren()
        {
            children.Clear();
        }
    }
    
    public class SxSceneGraphUtils
    {
        public static Matrix4x4 AccumulateTransforms(ISxSceneNode node)
        {
            if (node == null) return Matrix4x4.identity;
            return AccumulateTransforms(node.Parent) * node.WorldTransform.Matrix;
        }
        public static Matrix4x4 FindAbsoluteTransform(ISxSceneNode node)
        {
            if (node == null) return Matrix4x4.identity;
            return FindAbsoluteTransform(node.Parent) * node.WorldTransform.Matrix;
        }
    }

    public class SxRootSceneNode : SxSceneNodeBase
    {
        public override void Draw(SxRenderContext context, Matrix4x4 accumWorldTransform, SxRenderCommandList renderCommandList)
        {
        }

        public override void Tick(SxRenderContext context, float deltaTime)
        {
        }

        public override void Destroy()
        {
        }
    }
    
}