//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.Common
{
    public class BaseFlowExecNodeHandler_CreateTilemapElevationsInspector : FlowExecNodeHandlerInspectorBase
    {
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            DrawHeader("Marker");
            {
                EditorGUI.indentLevel++;
                DrawProperties("markerName");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Noise Settings");
            {
                EditorGUI.indentLevel++;
                DrawProperties("noiseOctaves", "noiseFrequency", "noiseValuePower", "numSteps");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Height data");
            {
                EditorGUI.indentLevel++;
                DrawProperties("minHeight", "maxHeight", "seaLevel");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Colors");
            {
                EditorGUI.indentLevel++;
                DrawProperties("landColor", "seaColor", "minColorMultiplier");
                EditorGUI.indentLevel--;
            }
        }
    }
}