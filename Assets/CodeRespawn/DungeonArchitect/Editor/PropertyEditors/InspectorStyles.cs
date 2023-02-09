//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors
{
    public class InspectorStyles
    {
        public static readonly GUIStyle TitleStyle;
        public static readonly GUIStyle HeaderStyle;

        static InspectorStyles()
        {
            TitleStyle = new GUIStyle(EditorStyles.label);
            TitleStyle.fontSize += 4;
            
            HeaderStyle = new GUIStyle(EditorStyles.boldLabel);
            HeaderStyle.fontSize += 2;  
        }
    }
}