//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.GridFlow
{
    [CustomEditor(typeof(GridFlowLayoutTaskCreatePath), false)]
    public class GridFlowExecNodeHandler_CreatePathInspector : BaseFlowExecNodeHandler_CreatePathInspector
    {
        private DAInspectorMonoScriptProperty<IGridFlowLayoutNodePositionConstraint> positionConstraintScriptProperty;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            var handler = target as GridFlowLayoutTaskCreatePath;
            
            // Create the position constraint script property
            {
                var className = (handler != null) ? handler.nodePositionConstraintScriptClassName : "";
                positionConstraintScriptProperty = new DAInspectorMonoScriptProperty<IGridFlowLayoutNodePositionConstraint>(className);
            }
        }
        
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();
            var handler = target as GridFlowLayoutTaskCreatePath;
            
            DrawHeader("Position Constraints");
            {
                EditorGUI.indentLevel++;
                DrawProperty("positionConstraintMode");

                if (handler.positionConstraintMode == GridFlowLayoutTaskCreatePath.NodeConstraintType.Script)
                {
                    EditorGUILayout.HelpBox("Specify a script that inherits from ScriptableObject and implements IGridFlowLayoutNodePositionConstraint",
                        MessageType.Info);
                    positionConstraintScriptProperty.Draw(className => handler.nodePositionConstraintScriptClassName = className);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}