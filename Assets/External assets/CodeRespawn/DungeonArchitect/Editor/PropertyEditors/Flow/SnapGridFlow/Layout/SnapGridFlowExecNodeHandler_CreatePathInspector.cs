//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.SnapGridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.SnapGridFlow
{
    [CustomEditor(typeof(SGFLayoutTaskCreatePath), false)]
    public class SnapGridFlowExecNodeHandler_CreatePathInspector : BaseFlowExecNodeHandler_CreatePathInspector
    {
        private DAInspectorMonoScriptProperty<ISGFLayoutNodePositionConstraint> positionConstraintScriptProperty;
        private DAInspectorMonoScriptProperty<ISGFLayoutNodeCategoryConstraint> categoryConstraintScriptProperty;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            var task = target as SGFLayoutTaskCreatePath;
            
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
            var task = target as SGFLayoutTaskCreatePath;

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

                if (task.positionConstraintMode == SGFLayoutTaskCreatePath.NodeConstraintType.Script)
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
                if (task.categoryConstraintMode == SGFLayoutTaskCreatePath.NodeConstraintType.Script)
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