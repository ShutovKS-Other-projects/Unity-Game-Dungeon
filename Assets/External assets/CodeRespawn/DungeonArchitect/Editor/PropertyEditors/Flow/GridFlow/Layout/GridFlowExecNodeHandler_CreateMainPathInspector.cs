//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.GridFlow
{
    [CustomEditor(typeof(GridFlowLayoutTaskCreateMainPath), false)]
    public class GridFlowExecNodeHandler_CreateMainPathInspector : BaseFlowExecNodeHandler_CreateMainPathInspector
    {
        GridFlowExecNodePlacementSettingInspector startPlacementInspector;
        GridFlowExecNodePlacementSettingInspector goalPlacementInspector;
        
        private DAInspectorMonoScriptProperty<IGridFlowLayoutNodePositionConstraint> positionConstraintScriptProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            var handler = target as GridFlowLayoutTaskCreateMainPath;
            startPlacementInspector = new GridFlowExecNodePlacementSettingInspector(this, "startPlacementSettings", "Start Item Placement", handler.startPlacementSettings);
            goalPlacementInspector = new GridFlowExecNodePlacementSettingInspector(this, "goalPlacementSettings", "Goal Item Placement", handler.goalPlacementSettings);
            
            
            // Create the position constraint script property
            {
                var className = (handler != null) ? handler.nodePositionConstraintScriptClassName : "";
                positionConstraintScriptProperty = new DAInspectorMonoScriptProperty<IGridFlowLayoutNodePositionConstraint>(className);
            }
        }

        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();
            var handler = target as GridFlowLayoutTaskCreateMainPath;
            
            startPlacementInspector.Draw(this);
            goalPlacementInspector.Draw(this);
            
            DrawHeader("Position Constraints");
            {
                EditorGUI.indentLevel++;
                DrawProperty("positionConstraintMode");

                if (handler.positionConstraintMode == GridFlowLayoutTaskCreateMainPath.NodeConstraintType.StartEndNode)
                {
                    EditorGUILayout.HelpBox(
                        "Provide a list of coords where the start / end nodes can be placed. Leave the array blank to ignore the constraints",
                        MessageType.Info);
                    DrawProperty("startNodePositionConstraints", true);
                    DrawProperty("endNodePositionConstraints", true);
                }
                else if (handler.positionConstraintMode == GridFlowLayoutTaskCreateMainPath.NodeConstraintType.Script)
                {
                    EditorGUILayout.HelpBox("Specify a script that inherits from ScriptableObject and implements IGridFlowLayoutNodePositionConstraint",
                        MessageType.Info);
                    positionConstraintScriptProperty.Draw(className => handler.nodePositionConstraintScriptClassName = className);
                }
                EditorGUI.indentLevel--;
            }
            
            DrawHeader("Size Constraints");
            {
                EditorGUI.indentLevel++;
                DrawProperties("fixedStartRoomSize", "fixedEndRoomSize");
                EditorGUI.indentLevel--;
            }
            
            DrawHeader("Advanced: Performance");
            {
                EditorGUI.indentLevel++;
                DrawProperties("numParallelSearches", "maxFramesToProcess");
                EditorGUI.indentLevel--;
            }
        }
    }
}