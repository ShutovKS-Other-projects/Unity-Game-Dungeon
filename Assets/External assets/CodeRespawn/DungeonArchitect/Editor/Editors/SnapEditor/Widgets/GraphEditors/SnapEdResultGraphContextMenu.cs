//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class SnapEdResultGraphContextMenu : GraphContextMenu
    {
        class ItemInfo
        {
            public ItemInfo(UISystem uiSystem, SnapEdResultGraphEditorAction action)
            {
                this.uiSystem = uiSystem;
                this.action = action;
            }

            public UISystem uiSystem;
            public SnapEdResultGraphEditorAction action;
        }
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld, UISystem uiSystem)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Comment Node"), false, HandleContextMenu, new ItemInfo(uiSystem, SnapEdResultGraphEditorAction.CreateCommentNode));
            menu.ShowAsContext();
        }

        void HandleContextMenu(object userdata)
        {
            var item = userdata as ItemInfo;
            DispatchMenuItemEvent(item.action, BuildEvent(null, item.uiSystem));
        }
    }
}
