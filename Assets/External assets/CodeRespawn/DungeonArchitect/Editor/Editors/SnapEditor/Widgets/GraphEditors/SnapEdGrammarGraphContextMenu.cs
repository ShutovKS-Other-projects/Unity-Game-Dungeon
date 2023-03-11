//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.UI;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class SnapEdGrammarGraphContextMenu : GraphContextMenu
    {
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld, UISystem uiSystem)
        {
            this.sourcePin = sourcePin;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Wildcard Node"), false, HandleContextMenu, new SnapEdGrammarGraphEditorContextMenuData(uiSystem, SnapEdGrammarGraphEditorAction.CreateWildcard));
            menu.AddSeparator("");

            var grammarGraphEditor = graphEditor as SnapEdGrammarGraphEditor;
            var flowAsset = (grammarGraphEditor != null) ? grammarGraphEditor.FlowAsset : null;
            if (flowAsset != null && flowAsset.nodeTypes != null && flowAsset.nodeTypes.Length > 0)
            {
                foreach (var nodeType in flowAsset.nodeTypes)
                {
                    var data = new SnapEdGrammarGraphEditorContextMenuData(uiSystem, SnapEdGrammarGraphEditorAction.CreateTaskNode, nodeType);
                    menu.AddItem(new GUIContent("Add Node: " + nodeType.nodeName), false, HandleContextMenu, data);
                }
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent("Add Comment Node"), false, HandleContextMenu, new SnapEdGrammarGraphEditorContextMenuData(uiSystem, SnapEdGrammarGraphEditorAction.CreateCommentNode));
            menu.ShowAsContext();
        }

        void HandleContextMenu(object userdata)
        {
            var item = userdata as SnapEdGrammarGraphEditorContextMenuData;
            DispatchMenuItemEvent(userdata, BuildEvent(null, item.uiSystem));
        }
    }
}
