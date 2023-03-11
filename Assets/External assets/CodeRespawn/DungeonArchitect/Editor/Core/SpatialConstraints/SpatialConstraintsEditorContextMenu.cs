//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.UI;

namespace DungeonArchitect.Editors.SpatialConstraints
{
    public class SpatialConstraintsEditorContextMenu : GraphContextMenu
    {
        class ItemInfo
        {
            public ItemInfo(UISystem uiSystem, SpatialConstraintEditorMenuAction action)
            {
                this.uiSystem = uiSystem;
                this.action = action;
            }
            public UISystem uiSystem;
            public SpatialConstraintEditorMenuAction action;
        }

        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld, UISystem uiSystem)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Rule Node"), false, HandleContextMenu, new ItemInfo(uiSystem, SpatialConstraintEditorMenuAction.CreateRuleNode));
            menu.AddItem(new GUIContent("Create Comment Node"), false, HandleContextMenu, new ItemInfo(uiSystem, SpatialConstraintEditorMenuAction.CreateCommentNode));
            menu.ShowAsContext();
        }

        void HandleContextMenu(object userdata)
        {
            var item = userdata as ItemInfo;

            DispatchMenuItemEvent(item.action, BuildEvent(null, item.uiSystem));
        }
    }
}