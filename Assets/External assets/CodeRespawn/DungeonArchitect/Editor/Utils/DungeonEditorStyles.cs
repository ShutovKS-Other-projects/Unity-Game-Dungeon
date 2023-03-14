//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors
{
    public class DungeonEditorStyles
    {
        public static readonly GUIStyle discordToolButtonStyle;
        public static readonly GUIStyle docsToolButtonStyle;
        static DungeonEditorStyles()
        {
            discordToolButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
            discordToolButtonStyle.normal.textColor = Color.white;
            discordToolButtonStyle.hover.textColor = new Color(1, 1, 1, 0.9f);
            
            docsToolButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
            docsToolButtonStyle.normal.textColor = Color.white;
            docsToolButtonStyle.hover.textColor = new Color(1, 1, 1, 0.9f);
        }
    }
}