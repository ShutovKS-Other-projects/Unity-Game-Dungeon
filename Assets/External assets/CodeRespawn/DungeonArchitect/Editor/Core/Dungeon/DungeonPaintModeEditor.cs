//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;
using DMathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for the paint mode object
    /// </summary>
    public class DungeonPaintModeEditor : Editor
    {
        protected virtual void OnEnable()
        {
            SceneView.duringSceneGui += SceneGUI;
        }
        
        protected virtual void OnDisable()
        {
            SceneView.duringSceneGui -= SceneGUI;
        }

        protected virtual void SceneGUI(SceneView sceneview) {
        }
    }
}
