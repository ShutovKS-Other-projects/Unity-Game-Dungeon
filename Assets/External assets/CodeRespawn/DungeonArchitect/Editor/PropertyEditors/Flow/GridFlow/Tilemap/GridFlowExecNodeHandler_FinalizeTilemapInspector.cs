//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Common;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;
using UnityEditor;

namespace DungeonArchitect.Editors.Flow.GridFlow
{
    [CustomEditor(typeof(GridFlowTilemapTaskFinalize), false)]
    public class GridFlowExecNodeHandler_FinalizeTilemapInspector : BaseFlowExecNodeHandler_FinalizeTilemapInspector
    {
        protected override void DrawMiscProperties()
        {
            base.DrawMiscProperties();
            
            DrawProperties("debugUnwalkableCells");
        }
    }
}