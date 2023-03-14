//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.GridFlow;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors
{
    //[CustomEditor(typeof(DungeonConfig), true)]
    public abstract class DungeonConfigEditorBase : DAInspectorBase
    {
        protected abstract string Title { get; }

        
        public override void OnInspectorGUI()
        {
            sobject.Update();

            DrawHeader(Title, false);
            using (new EditorGUI.IndentLevelScope())
            {
                DrawProperty("Seed");
                DrawConfigProperties();
            }
            
            DrawExtraConfigProperties();

            InspectorNotify.Dispatch(sobject, target);
        }

        protected abstract void DrawConfigProperties();
        protected abstract void DrawExtraConfigProperties();
    }
    
    [CustomEditor(typeof(GridFlowDungeonConfig), true)]
    public class GridFlowDungeonConfigEditor : DungeonConfigEditorBase
    {
        protected override string Title => "Grid Flow Dungeon";
        protected override void DrawConfigProperties()
        {
            DrawProperties("flowAsset", "gridSize", "numGraphRetries", "Mode2D");
        }

        protected override void DrawExtraConfigProperties()
        {
            DrawHeader("Advanced");
            using (new EditorGUI.IndentLevelScope())
            {
                DrawProperty("flipEdgeWalls");
            }
        }
    }
}
