//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.GridFlow
{
    [CustomEditor(typeof(GridFlowLayoutTaskFinalizeGraph), false)]
    public class GridFlowExecNodeHandler_FinalizeGraphInspector : BaseFlowExecNodeHandler_FinalizeGraphInspector
    {
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            DrawHeader("Layout");
            {
                EditorGUI.indentLevel++;
                DrawProperties("generateCaves", "generateCorridors", "maxEnemiesPerCaveNode");
                EditorGUI.indentLevel--;
            }
        }
    }
}