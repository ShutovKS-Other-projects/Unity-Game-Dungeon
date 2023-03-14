//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.Common
{
    public class BaseFlowExecNodeHandler_CreateKeyLockInspector : FlowExecNodeHandlerInspectorBase
    {
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            DrawHeader("Branch Info");
            {
                EditorGUI.indentLevel++;
                DrawProperties("keyBranch", "lockBranch");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Marker Names");
            {
                EditorGUI.indentLevel++;
                DrawProperties("keyMarkerName", "lockMarkerName");
                EditorGUI.indentLevel--;
            }
        }
    }
}