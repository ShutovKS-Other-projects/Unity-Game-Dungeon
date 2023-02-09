//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for Theme override volumes
    /// </summary>
    [CustomEditor(typeof(ThemeOverrideVolume))]
    public class ThemeOverrideVolumeEditor : VolumeEditor
    {

        public override void OnUpdate(SceneView sceneView)
        {
            onlyReapplyTheme = true;
            base.OnUpdate(sceneView);
        }
    }
}
