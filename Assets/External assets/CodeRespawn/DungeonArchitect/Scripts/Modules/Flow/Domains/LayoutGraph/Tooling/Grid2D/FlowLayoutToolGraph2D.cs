//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph2D
{
    public class FlowLayoutToolGraph2D : Graph
    {
        public override void OnEnable()
        {
            base.OnEnable();

            hideFlags = HideFlags.HideInHierarchy;
        }
    }
}
