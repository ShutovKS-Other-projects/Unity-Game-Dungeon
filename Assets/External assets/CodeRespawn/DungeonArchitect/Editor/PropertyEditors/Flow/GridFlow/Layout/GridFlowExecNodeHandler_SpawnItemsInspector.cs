//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.GridFlow
{
    [CustomEditor(typeof(GridFlowLayoutTaskSpawnItems), false)]
    public class GridFlowExecNodeHandler_SpawnItemsInspector : BaseFlowExecNodeHandler_SpawnItemsInspector
    {
        GridFlowExecNodePlacementSettingInspector placementInspector;
        protected override void OnEnable()
        {
            base.OnEnable();

            var handler = target as GridFlowLayoutTaskSpawnItems;
            placementInspector = new GridFlowExecNodePlacementSettingInspector(this, "placementSettings", "Placement Method", handler.placementSettings);
        }
        
        public override void HandleInspectorGUI()
        {
            base.HandleInspectorGUI();
            
            placementInspector.Draw(this);
        }
    }
}