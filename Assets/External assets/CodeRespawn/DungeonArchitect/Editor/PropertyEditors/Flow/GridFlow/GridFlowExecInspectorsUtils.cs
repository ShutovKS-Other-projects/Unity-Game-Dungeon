//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Impl.GridFlow;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.GridFlow
{
    class GridFlowExecNodePlacementSettingInspector
    {
        DAInspectorMonoScriptProperty<ITilemapItemPlacementStrategy> scriptProperty;
        string settingsVariableName;
        string title;
        TilemapItemPlacementSettings settings;
        public GridFlowExecNodePlacementSettingInspector(FlowExecNodeHandlerInspectorBase inspector, string settingsVariableName, string title, TilemapItemPlacementSettings settings)
        {
            this.settingsVariableName = settingsVariableName;
            this.settings = settings;
            this.title = title;
            scriptProperty = inspector.CreateScriptProperty<ITilemapItemPlacementStrategy>(settings.placementScriptClass);
        }

        public void Draw(FlowExecNodeHandlerInspectorBase inspector)
        {
            EditorGUILayout.Space();
            GUILayout.Label(title, InspectorStyles.HeaderStyle);
            {
                EditorGUI.indentLevel++;

                inspector.DrawProperties(settingsVariableName + ".placementMethod");
                if (settings.placementMethod == TilemapItemPlacementMethod.Script)
                {
                    scriptProperty.Draw(className => settings.placementScriptClass = className);
                }

                if (settings.placementMethod != TilemapItemPlacementMethod.Script)
                {
                    inspector.DrawProperties(settingsVariableName + ".avoidPlacingNextToDoors");
                }

                if (settings.placementMethod != TilemapItemPlacementMethod.RandomTile)
                {
                    inspector.DrawProperties(settingsVariableName + ".fallbackToRandomPlacement");
                }
                
                EditorGUI.indentLevel--;
            }
        }
    }
}