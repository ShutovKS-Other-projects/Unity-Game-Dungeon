//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.GridFlow
{

    [CustomEditor(typeof(GridFlowTilemapTaskInitialize), false)]
    public class GridFlowExecNodeHandlerInspector_InitializeTilemap : BaseFlowExecNodeHandlerInspector_InitializeTilemap
    {
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            DrawHeader("Layout Settings");
            {
                EditorGUI.indentLevel++;
                DrawProperties("tilemapSizePerNode", "perturbAmount", "corridorLaneWidth", "layoutPadding", "cropTilemap", "wallGenerationMethod");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Cave Settings");
            {
                EditorGUI.indentLevel++;
                DrawProperties("caveThickness", "caveAutomataNeighbors", "caveAutomataIterations");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Color Settings");
            {
                EditorGUI.indentLevel++;
                DrawProperties("roomColorSaturation", "roomColorBrightness");
                EditorGUI.indentLevel--;
            }
        }
    }
}