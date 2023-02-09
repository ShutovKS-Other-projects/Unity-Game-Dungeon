//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using DungeonArchitect.Builders.Grid;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for Platform volumes
    /// </summary>
    [CustomEditor(typeof(PlatformVolume))]
    public class PlatformVolumeEditor : VolumeEditor
    {
        public override void OnUpdate(SceneView sceneView)
        {
            base.OnUpdate(sceneView);
            /*
            var platform = target as PlatformVolume;
            if (platform != null)
            {
                var transform = platform.gameObject.transform;
                if (transform.hasChanged)
                {
                    OnTransformModified(platform);
                    transform.hasChanged = false;
                }
            }
            */
        }

    }
}
