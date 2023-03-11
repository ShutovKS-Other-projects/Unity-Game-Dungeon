//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Graphs;
using DungeonArchitect.Flow.Exec;

namespace DungeonArchitect.Editors.Flow
{
    public class FlowExecRuleNodeRenderer : FlowExecNodeRendererBase
    {
        protected override string GetDescText(GraphNode node)
        {
            var handler = GetHandler(node);
            return (handler != null) ? handler.description : "";
        }

        protected override string GetCaption(GraphNode node)
        {
            var handler = GetHandler(node);
            var menuAttribute = handler != null ? FlowExecNodeInfoAttribute.GetHandlerAttribute(handler.GetType()) : null;
            return (menuAttribute != null) ? menuAttribute.Title : "[INVALID]";
        }

        protected override Color GetPinColor(GraphNode node)
        {
            var handler = GetHandler(node);
            return (handler != null) ? new Color(0.3f, 0.3f, 0.5f) : Color.red;
        }

        protected override Color GetBodyColor(GraphNode node)
        {
            var handler = GetHandler(node);
            return (handler != null) ? new Color(0.1f, 0.1f, 0.1f, 1) : Color.red;
        }
    }
}
