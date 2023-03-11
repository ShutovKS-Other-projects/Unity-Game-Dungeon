//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.RoomDesigner.Editors
{
    public class DungeonRoomDesignerToolbar
    {
        private int mainToolIndex = 1;
        public int MainToolIndex
        {
            get { return mainToolIndex; }
            set { mainToolIndex = value; }
        }

        private int subToolIndex = 0;
        public int SubToolIndex
        {
            get { return subToolIndex; }
            set { subToolIndex = value; }
        }


        public bool Draw(DungeonRoomDesigner room)
        {
            EditorGUI.BeginChangeCheck();
            DrawMainToolbar();
            DrawSubToolbar();
            return EditorGUI.EndChangeCheck();
        }

        void DrawMainToolbar()
        {
            Handles.BeginGUI();
            string[] tabs = new string[] { "Room", "Doors" };

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            mainToolIndex = GUILayout.Toolbar(mainToolIndex, tabs);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            Handles.EndGUI();
        }

        void DrawSubToolbar()
        {
            Handles.BeginGUI();
            string[] tabs;

            if (mainToolIndex == 0)
            {
                tabs = new string[] { "Move", "Bounds" };
            }
            else if (mainToolIndex == 1)
            {
                tabs = new string[] { "Move", "Bounds" };
            }
            else
            {
                tabs = new string[0];
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            subToolIndex = GUILayout.Toolbar(subToolIndex, tabs);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            Handles.EndGUI();
        }
        
    }
}
