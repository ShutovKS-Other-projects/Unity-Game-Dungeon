//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.GridFlow
{
    
    [CustomEditor(typeof(GridFlowLayoutTaskMirrorGraph), false)]
    public class GridFlowExecNodeHandler_MirrorGraphInspector : BaseFlowExecNodeHandler_MirrorGraphInspector
    {
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            DrawHeader("Direction");
            {
                EditorGUI.indentLevel++;
                DrawProperties("mirrorX", "mirrorY");
                EditorGUI.indentLevel--;
            }
            
        }
    }
}