//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors.Flow
{
    public class FlowExecResultNodeRenderer : FlowExecNodeRendererBase
    {
        protected override string GetCaption(GraphNode node)
        {
            return "Result";
        }

        protected override Color GetPinColor(GraphNode node)
        {
            return new Color(0.1f, 0.4f, 0.2f);
        }

        protected override Color GetBodyColor(GraphNode node)
        {
            return new Color(0.1f, 0.1f, 0.1f, 1);
        }
    }
}
