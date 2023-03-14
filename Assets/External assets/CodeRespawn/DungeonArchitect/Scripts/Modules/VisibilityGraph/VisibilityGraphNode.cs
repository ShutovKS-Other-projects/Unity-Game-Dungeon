//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonArchitect.Visibility
{
    public abstract class VisibilityGraphNode
    {
        private Bounds bounds;
        private bool _visible = true;
        private HashSet<VisibilityGraphNode> connectedNodes = new HashSet<VisibilityGraphNode>();

        public VisibilityGraphNode[] ConnectedNodes
        {
            get => connectedNodes.ToArray();
        }
        
        public Bounds Bounds
        {
            get => bounds;
        }
        
        public void Initialize()
        {
            bounds = CalculateBounds();
        }
        
        public void AddConnection(VisibilityGraphNode node)
        {
            if (node == null || node == this) return;
            
            connectedNodes.Add(node);
        }

        public virtual bool IsVisible()
        {
            return _visible;
        }

        public void SetVisible(bool visible)
        {
            if (IsVisible() != visible)
            {
                _visible = visible;
                SetVisibleImpl(visible);
            }
        }
        
        public abstract void SetVisibleImpl(bool visible);
        public abstract Bounds CalculateBounds();
    }
}
