//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Builders.GridFlow;

namespace DungeonArchitect.Editors.Flow
{
    public abstract class FlowExecGraphEditorConfigInspector : Editor
    {
        protected SerializedObject sobject;
        private SerializedProperty randomizeSeed;
        private SerializedProperty seed;

        protected virtual void OnEnable()
        {
            sobject = new SerializedObject(target);
            randomizeSeed = sobject.FindProperty("randomizeSeed");
            seed = sobject.FindProperty("seed");
        }

        protected abstract void DrawDungeonProperty();
        
        public override void OnInspectorGUI()
        {
            sobject.Update();
            var config = target as FlowEditorConfig;

            GUILayout.Label("Linked Dungeon", EditorStyles.boldLabel);
            DrawDungeonProperty();

            GUILayout.Label("Preview Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomizeSeed);
            bool enableSeedEdit = true;
            if (config.FlowBuilder == null)
            {
                if (config.randomizeSeed)
                {
                    enableSeedEdit = false;
                }
            }
            else
            {
                enableSeedEdit = false;
                var dungeonConfig = config.FlowBuilder.gameObject.GetComponent<GridFlowDungeonConfig>();
                if (dungeonConfig != null)
                {
                    int seed = (int)dungeonConfig.Seed;
                    config.seed = seed;
                    EditorGUILayout.HelpBox("The seed cannot be modified, it is taken from the linked dungeon game object", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Invalid dungeon prefab configuration. Cannot find GridFlowDungeonConfig component", MessageType.Info);
                }
            }

            GUI.enabled = enableSeedEdit;
            EditorGUILayout.PropertyField(seed);
            GUI.enabled = true;

            sobject.ApplyModifiedProperties();
        }

    }

}
