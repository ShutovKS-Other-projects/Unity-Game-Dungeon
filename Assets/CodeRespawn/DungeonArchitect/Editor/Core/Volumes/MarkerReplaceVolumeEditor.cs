//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for Negation volumes
    /// </summary>
    [CustomEditor(typeof(MarkerReplaceVolume))]
    public class MarkerReplaceVolumeEditor : VolumeEditor
    {
        public override void OnUpdate(SceneView sceneView)
        {
            dynamicUpdate = false;
            onlyReapplyTheme = true;
            base.OnUpdate(sceneView);
        }
    }
}
