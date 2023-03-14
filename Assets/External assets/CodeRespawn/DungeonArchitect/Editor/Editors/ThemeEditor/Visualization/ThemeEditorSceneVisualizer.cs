//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Themeing;
using DungeonArchitect.Utils;
using UnityEditor;
using UnityEngine;
using MathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors.Visualization
{
    [System.Serializable]
    class ThemeEditorSceneVisualizer
    {
        [SerializeField]
        private DungeonMarkerVisualizerComponent visualizerComponentRefInternal = null;
        
        public void Build(Dungeon dungeon, string markerName)
        {
            var state = CreateState(dungeon, markerName);
            if (state == null)
            {
                Clear();
                return;
            }
            
            var visualizerComp = GetVisualizerComponent();
            if (visualizerComp != null)
            {
                visualizerComp.Build(state);
            }
        }

        public void Clear()
        {
            var visualizerComp = GetVisualizerComponent();
            if (visualizerComp != null)
            {
                visualizerComp.Clear();
            }
        }

        private DungeonMarkerVisualizerComponent GetVisualizerComponent()
        {
            if (visualizerComponentRefInternal != null)
            {
                return visualizerComponentRefInternal;
            }
            
            // Find the component in the scene
            visualizerComponentRefInternal = Object.FindObjectOfType<DungeonMarkerVisualizerComponent>();
            if (visualizerComponentRefInternal == null)
            {
                visualizerComponentRefInternal = CreateVisualizerGameObject_Internal();
            }
            return visualizerComponentRefInternal;
        }
        
        private static DungeonMarkerVisualizerComponent CreateVisualizerGameObject_Internal()
        {
            var gameObject = EditorUtility.CreateGameObjectWithHideFlags("ThemeEditorVisualizer", HideFlags.HideAndDontSave);  // TODO: Change to HideAndDontSave  
            var component = gameObject.AddComponent<DungeonMarkerVisualizerComponent>();
            return component;
        }
        
        ThemeEditorVisualizationState CreateState(Dungeon dungeon, string markerName)
        {
            if (dungeon == null || string.IsNullOrEmpty(markerName))
            {
                return null;
            }
            
            var dungeonBuilder = dungeon.GetComponent<DungeonBuilder>();
            if (dungeonBuilder == null)
            {
                return null;
            }
            
            var markerVisualizationBuilder = ThemeMarkerVisualizationBuilderFactory.Create(dungeonBuilder.GetType()); 
            if (markerVisualizationBuilder == null)
            {
                return null;
            }

            var state = new ThemeEditorVisualizationState();
            if (!markerVisualizationBuilder.Build(dungeon, markerName, out state.LocalGeometry, out state.Material))
            {
                return null;
            }

            var dungeonConfig = dungeon.GetComponent<DungeonConfig>();
            var mode2D = dungeonConfig != null && dungeonConfig.IsMode2D();
            var rotation2D = Quaternion.Euler(90, 0, 0);
            if (mode2D)
            {
                // Flip the geometry for 2D mode
                if (state.LocalGeometry != null)
                {
                    for (var i = 0; i < state.LocalGeometry.Vertices.Length; i++)
                    {
                        //state.LocalGeometry.Vertices[i] = rotation2D * state.LocalGeometry.Vertices[i];
                    }
                }
            }
            
            // Fill up the marker transform list
            var markerTransforms = new List<Matrix4x4>();
            foreach (var marker in dungeonBuilder.Markers)
            {
                if (marker.SocketType == markerName)
                {
                    var markerTransform = marker.Transform;
                    if (mode2D)
                    {
                        var position = Matrix.GetTranslation(ref markerTransform);
                        position = MathUtils.FlipYZ(position);
                        position.z = -30;
                        var scale = Matrix.GetScale(ref markerTransform);
                        markerTransform = Matrix4x4.TRS(position, rotation2D, scale);
                    }
                    
                    markerTransforms.Add(markerTransform);
                }
            }

            state.MarkerTransforms = markerTransforms.ToArray();

            return state;
        }
    }
}