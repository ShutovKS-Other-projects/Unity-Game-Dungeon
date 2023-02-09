//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.Common
{
    public class BaseFlowExecNodeHandler_FinalizeGraphInspector : FlowExecNodeHandlerInspectorBase
    {
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();
            
            DrawHeader("One-way Doors");
            {
                EditorGUI.indentLevel++;
                DrawProperties("oneWayDoorPromotionWeight");
                EditorGUI.indentLevel--;
            }
        }
    }
}