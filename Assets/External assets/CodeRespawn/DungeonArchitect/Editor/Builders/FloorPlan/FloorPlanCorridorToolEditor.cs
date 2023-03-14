//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.FloorPlan;
using DungeonArchitect.Builders.FloorPlan.Tooling;
using UnityEditor;
using UnityEngine;
using MathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(FloorPlanCorridorTool))]
    public class FloorPlanCorridorToolEditor : Editor
    {
        IntVector positionOnGrid;
        bool requestRebuild = false;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Rebuild Dungeon"))
            {
                requestRebuild = true;
            }
        }


        void OnEnable()
        {
            EditorApplication.update += EditorUpdate;
            SceneView.duringSceneGui += OnUpdate;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnUpdate;
            EditorApplication.update -= EditorUpdate;
        }

        void EditorUpdate()
        {
            if (requestRebuild)
            {
                Rebuild();
                requestRebuild = false;
            }
        }
        public virtual void OnUpdate(SceneView sceneView)
        {
            var tool = target as FloorPlanCorridorTool;
            if (tool != null)
            {
                if (tool.dynamicUpdate)
                {
                    var transform = tool.transform;
                    if (transform.hasChanged)
                    {
                        OnTransformModified(tool);
                        transform.hasChanged = false;
                    }
                }
            }
        }

        void Rebuild()
        {
            var tool = target as FloorPlanCorridorTool;
            if (tool != null && tool.dungeon != null)
            {
                var dungeon = tool.dungeon;
                dungeon.Build(new EditorDungeonSceneObjectInstantiator());
            }
        }

        protected void OnTransformModified(FloorPlanCorridorTool tool)
        {
            if (tool == null || tool.dungeon == null)
            {
                return;
            }
            var config = tool.dungeon.GetComponent<FloorPlanConfig>();
            if (config == null)
            {
                return;
            }

            var newPositionOnGrid = MathUtils.ToIntVector(MathUtils.Divide(tool.transform.position, config.GridSize));

            if (!positionOnGrid.Equals(newPositionOnGrid))
            {
                positionOnGrid = newPositionOnGrid;
                OnGridTransformModified();
            }

        }

        void OnGridTransformModified()
        {
            requestRebuild = true;
        }
        
    }
}