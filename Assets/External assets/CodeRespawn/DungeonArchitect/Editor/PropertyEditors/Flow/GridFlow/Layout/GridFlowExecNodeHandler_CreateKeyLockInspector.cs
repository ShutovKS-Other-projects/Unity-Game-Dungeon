//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.GridFlow
{
    [CustomEditor(typeof(GridFlowLayoutTaskCreateKeyLock), false)]
    public class GridFlowExecNodeHandler_CreateKeyLockInspector : BaseFlowExecNodeHandler_CreateKeyLockInspector
    {
        GridFlowExecNodePlacementSettingInspector placementInspector;
        protected override void OnEnable()
        {
            base.OnEnable();

            var handler = target as GridFlowLayoutTaskCreateKeyLock;
            placementInspector = new GridFlowExecNodePlacementSettingInspector(this, "placementSettings", "Key Placement", handler.placementSettings);
        }

        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();

            placementInspector.Draw(this);
        }
    }
}