//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout.Tasks;
using DungeonArchitect.Flow.Items;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.Common
{
    public class BaseFlowExecNodeHandler_SpawnItemsInspector : FlowExecNodeHandlerInspectorBase
    {
        public enum InspectorFlowGraphItemType
        {
            Enemy = 2,
            Bonus = 3,
            Custom = 6,
        }
        
        void DrawItemTypeDropdown()
        {
            var itemTypeProperty = GetProperty("itemType");
            var currentValue = (InspectorFlowGraphItemType) itemTypeProperty.intValue;
            var edValue = (InspectorFlowGraphItemType) EditorGUILayout.EnumPopup("Item Type", currentValue);
            itemTypeProperty.intValue = (int)edValue;
        }
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            var handler = target as LayoutBaseFlowTaskSpawnItems;

            DrawHeader("Spawn Info");
            {
                EditorGUI.indentLevel++;
                DrawProperty("paths", true);
                DrawItemTypeDropdown();
                DrawProperties("markerName");

                if (handler.itemType == FlowGraphItemType.Custom)
                {
                    DrawProperty("customItemInfo", true);
                }

                DrawProperties("minCount", "maxCount");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Spawn Method");
            {
                EditorGUI.indentLevel++;
                DrawProperty("spawnMethod");
                if (handler.spawnMethod == LayoutFlowNodeHandler_SpawnItemMethod.CurveDifficulty)
                {
                    DrawProperty("spawnDistributionCurve");
                }

                if (handler.spawnMethod != LayoutFlowNodeHandler_SpawnItemMethod.RandomRange)
                {
                    DrawProperty("spawnDistributionVariance");
                }

                DrawProperties("minSpawnDifficulty", "spawnProbability");
                EditorGUI.indentLevel--;
            }

        }
    }
}