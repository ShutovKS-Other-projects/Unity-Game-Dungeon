//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using UnityEngine;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class GrammarExecEntryNodeRenderer : GrammarNodeRendererBase
    {
        protected override string GetCaption(GraphNode node)
        {
            return "Entry";
        }

        protected override Color GetPinColor(GraphNode node)
        {
            return new Color(0.1f, 0.4f, 0.4f);
        }

        protected override Color GetBodyColor(GraphNode node)
        {
            return new Color(0.1f, 0.1f, 0.1f, 1);
        }
    }
}
