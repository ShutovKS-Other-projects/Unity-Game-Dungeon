//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;
using UnityEngine;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class SnapEdExecutionGraphContextMenu : GraphContextMenu
    {
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld, UISystem uiSystem)
        {
            this.sourcePin = sourcePin;
            var execEditor = graphEditor as SnapEdExecutionGraphEditor;
            var flowAsset = (execEditor != null) ? execEditor.FlowAsset : null;

            var menu = uiSystem.Platform.CreateContextMenu();
            if (flowAsset != null && flowAsset.productionRules.Length > 0)
            {
                foreach (var rule in flowAsset.productionRules)
                {
                    string text = "Add Rule: " + rule.ruleName;
                    menu.AddItem(text, HandleContextMenu, new SnapEdExecutionGraphEditorMenuData(uiSystem, SnapEdExecutionGraphEditorAction.CreateRuleNode, rule));
                }
                menu.AddSeparator("");
            }
            menu.AddItem("Add Comment Node", HandleContextMenu, new SnapEdExecutionGraphEditorMenuData(uiSystem, SnapEdExecutionGraphEditorAction.CreateCommentNode));
            menu.Show();
        }

        void HandleContextMenu(object action)
        {
            var item = action as SnapEdExecutionGraphEditorMenuData;
            DispatchMenuItemEvent(action, BuildEvent(null, item.uiSystem));
        }
    }
}
