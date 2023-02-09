//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.Common
{
    public class BaseFlowExecNodeHandler_CreateMainPathInspector : FlowExecNodeHandlerInspectorBase
    {
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            DrawHeader("Path Info");
            {
                EditorGUI.indentLevel++;
                DrawProperties("pathSize", "pathName", "nodeColor");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Marker Names");
            {
                EditorGUI.indentLevel++;
                DrawProperties("startMarkerName", "goalMarkerName");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Start / Goal Nodes");
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox(
                    "You can give a different path name to the start / goal nodes. This way when other branches connect to this main path, they don't connect to the start / goal nodes. Leave it blank to make it part of the main branch",
                    MessageType.Info);

                DrawProperties("startNodePathName", "goalNodePathName");
                EditorGUI.indentLevel--;
            }
        }
    }
}