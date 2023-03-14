//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.Common
{
    public class BaseFlowExecNodeHandler_CreatePathInspector : FlowExecNodeHandlerInspectorBase
    {
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            DrawHeader("Path Info");
            {
                EditorGUI.indentLevel++;
                DrawProperties("minPathSize", "maxPathSize", "pathName", "nodeColor");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Branching Info");
            {
                EditorGUI.indentLevel++;
                DrawProperties("startFromPath", "endOnPath");
                EditorGUI.indentLevel--;
            }
        }
    }
}