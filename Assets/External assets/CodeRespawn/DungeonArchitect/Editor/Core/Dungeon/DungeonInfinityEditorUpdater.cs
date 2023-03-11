//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.Editors
{
    [CustomEditor(typeof(InfinityDungeonEditorUpdate))]
    public class DungeonInfinityEditorUpdater : Editor
    {

        protected virtual void OnEnable()
        {
            EditorApplication.update += EditorUpdate;
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }

        void EditorUpdate()
        {
            var updater = target as InfinityDungeonEditorUpdate;
            updater.EditorUpdate();
        }

    }
}
