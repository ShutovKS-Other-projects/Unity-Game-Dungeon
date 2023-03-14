//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.SnapGridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.SnapGridFlow
{
    [CustomEditor(typeof(SGFLayoutTaskCreateMainPath), false)]
    public class SnapGridFlowExecNodeHandler_CreateMainPathInspector : BaseFlowExecNodeHandler_CreateMainPathInspector
    {
        private DAInspectorMonoScriptProperty<ISGFLayoutNodePositionConstraint> positionConstraintScriptProperty;
        private DAInspectorMonoScriptProperty<ISGFLayoutNodeCategoryConstraint> categoryConstraintScriptProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            var task = target as SGFLayoutTaskCreateMainPath;
            
            // Create the position constraint script property
            {
                var className = (task != null) ? task.nodePositionConstraintScriptClassName : "";
                positionConstraintScriptProperty = new DAInspectorMonoScriptProperty<ISGFLayoutNodePositionConstraint>(className);
            }
            
            // Create the module constraint script property
            {
                var className = (task != null) ? task.categoryConstraintScriptClassName : "";
                categoryConstraintScriptProperty = new DAInspectorMonoScriptProperty<ISGFLayoutNodeCategoryConstraint>(className);
            }
        }

        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();
            var task = target as SGFLayoutTaskCreateMainPath;

            DrawHeader("Snap Info");
            {
                EditorGUI.indentLevel++;
                DrawProperty("snapModuleCategories", true);
                EditorGUI.indentLevel--;
            }

            DrawHeader("Position Constraints");
            {
                EditorGUI.indentLevel++;
                DrawProperty("positionConstraintMode");

                if (task.positionConstraintMode == SGFLayoutTaskCreateMainPath.NodeConstraintType.StartEndNode)
                {
                    EditorGUILayout.HelpBox(
                        "Provide a list of coords where the start / end nodes can be placed. Leave the array blank to ignore the constraints",
                        MessageType.Info);
                    DrawProperty("startNodePositionConstraints", true);
                    DrawProperty("endNodePositionConstraints", true);
                }
                else if (task.positionConstraintMode == SGFLayoutTaskCreateMainPath.NodeConstraintType.Script)
                {
                    EditorGUILayout.HelpBox("Specify a script that inherits from ScriptableObject and implements ISGFLayoutNodePositionConstraint",
                        MessageType.Info);
                    positionConstraintScriptProperty.Draw(className => task.nodePositionConstraintScriptClassName = className);
                }
                EditorGUI.indentLevel--;
            }

            DrawHeader("Snap Module Constraints");
            {
                EditorGUI.indentLevel++;
                DrawProperty("categoryConstraintMode");
                if (task.categoryConstraintMode == SGFLayoutTaskCreateMainPath.NodeConstraintType.StartEndNode)
                {
                    EditorGUILayout.HelpBox("Provide a list of module categories for the start / end nodes. Leave the array blank to ignore the constraints",
                        MessageType.Info);
                    DrawProperty("startNodeCategoryConstraints", true);
                    DrawProperty("endNodeCategoryConstraints", true);
                }
                else if (task.categoryConstraintMode == SGFLayoutTaskCreateMainPath.NodeConstraintType.Script)
                {
                    EditorGUILayout.HelpBox("Specify a script that inherits from ScriptableObject and implements ISGFLayoutNodeCategoryConstraint",
                        MessageType.Info);
                    categoryConstraintScriptProperty.Draw(className => task.categoryConstraintScriptClassName = className);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}