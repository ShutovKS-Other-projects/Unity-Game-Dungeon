//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;
namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for graph objects
    /// Shows the graph editor when a theme graph asset is selected
    /// </summary>
    [CustomEditor(typeof(Graph))]
    public class GraphInspector : Editor
    {
        SerializedObject sobject;

        public void OnEnable()
        {
            sobject = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            GUILayout.Label("Dungeon Theme", EditorStyles.boldLabel);

            sobject.ApplyModifiedProperties();

            ///ShowEditor();
        }

        void ShowEditor()
        {
            var graph = target as Graph;
            if (graph != null)
            {
                var window = EditorWindow.GetWindow<DungeonThemeEditorWindow>();
                if (window != null)
                {
                    window.Init(graph);
                }
            }
            else
            {
                Debug.LogWarning("Invalid Dungeon theme file");
            }
        }
    }
}
