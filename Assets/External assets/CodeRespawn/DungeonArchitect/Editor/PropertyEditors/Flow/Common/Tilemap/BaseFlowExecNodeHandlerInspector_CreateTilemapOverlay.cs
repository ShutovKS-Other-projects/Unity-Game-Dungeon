//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Tilemap.Tasks;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.Common
{
    public class BaseFlowExecNodeHandlerInspector_CreateTilemapOverlay : FlowExecNodeHandlerInspectorBase
    {
        DAInspectorMonoScriptProperty<ITilemapFlowOverlayGenerator> generatorProperty;
        protected override void OnEnable()
        {
            base.OnEnable();

            var handler = target as GridFlowTilemapTaskCreateOverlay;
            generatorProperty = CreateScriptProperty<ITilemapFlowOverlayGenerator>(handler.generatorScriptClass);
        }

        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            var handler = target as GridFlowTilemapTaskCreateOverlay;

            DrawHeader("Visuals");
            {
                EditorGUI.indentLevel++;
                DrawProperties("markerName", "color");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Generation Settings");
            {
                EditorGUI.indentLevel++;
                DrawProperty("generationMethod");
                if (handler.generationMethod == TilemapFlowNodeHandler_CreateTilemapOverlayGenMethod.Noise)
                {
                    // Show noise settings
                    DrawProperty("noiseSettings", true);
                }
                else if (handler.generationMethod == TilemapFlowNodeHandler_CreateTilemapOverlayGenMethod.Script)
                {
                    // Show script settings
                    generatorProperty.Draw(className => handler.generatorScriptClass = className);
                }
                DrawProperty("overlayBlocksTile");
                
                EditorGUI.indentLevel--;
            }

            DrawHeader("Merge Settings");
            {
                EditorGUI.indentLevel++;
                DrawProperty("mergeConfig", true);
                EditorGUI.indentLevel++;
            }
        }
    }
}