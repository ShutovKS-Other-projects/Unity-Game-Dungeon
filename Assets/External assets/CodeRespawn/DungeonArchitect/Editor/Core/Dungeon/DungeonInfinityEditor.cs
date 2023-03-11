//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.Editors
{
    [CustomEditor(typeof(InfinityDungeon))]
    public class DungeonInfinityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Build Dungeon"))
            {
                var dungeon = target as InfinityDungeon;
                dungeon.BuildDungeon();
            }
            if (GUILayout.Button("Destroy Dungeon"))
            {
                var dungeon = target as InfinityDungeon;
                dungeon.DestroyDungeon();
            }
        }

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
            var dungeon = target as InfinityDungeon;
            dungeon.EditorUpdate();
        }

    }
}
