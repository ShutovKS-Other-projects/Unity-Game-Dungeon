//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Text;
using DungeonArchitect.Editors.Flow.Impl;
using DungeonArchitect.Flow.Impl.SnapGridFlow;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.SnapGridFlow
{
    [CustomEditor(typeof(SnapGridFlowModuleDatabase))]
    public class SnapGridFlowModuleDatabaseEditor : Editor
    {
        private SerializedObject sobject;
        private SerializedProperty moduleBoundsAsset;
        private SerializedProperty modules;
        
        protected virtual void OnEnable()
        {
            sobject = new SerializedObject(target);
            moduleBoundsAsset = sobject.FindProperty("ModuleBoundsAsset");
            modules = sobject.FindProperty("Modules");
        }

        void BeginSection(string title)
        {
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
        }

        void EndSection()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        void CompileDatabase()
        {
            var moduleDB = target as SnapGridFlowModuleDatabase;
            SnapGridFlowModuleDBCompiler.CompileResultEntry[] errors;
            SnapGridFlowModuleDBCompiler.Build(moduleDB, out errors);
            
            // Display the error messages
            if (errors.Length > 0)
            {
                var message = new StringBuilder();
                foreach (var error in errors)
                {
                    if (error.moduleIndex == -1)
                    {
                        message.AppendFormat("[{0}]: {1}", error.errorType.ToString(), error.message);
                    }
                    else
                    {
                        message.AppendFormat("[{0} on Module Index {1}]: {2}", error.errorType.ToString(), error.moduleIndex, error.message);
                    }

                    message.AppendLine();
                }
                EditorUtility.DisplayDialog("Compile Failed", message.ToString(), "OK");
            }
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(new GUIContent("Hit compile whenever you modify your module prefabs or change the module entries below"));
            if (GUILayout.Button("Compile Module Database"))
            {
                CompileDatabase();
            }
            
            sobject.Update();

            GUILayout.Space(10);

            BeginSection("Module Bounds");
            EditorGUILayout.PropertyField(moduleBoundsAsset);
            EditorGUILayout.HelpBox(new GUIContent("Assign the bounds asset that is used in all the registered module prefabs"), false);
            EndSection();

            BeginSection("Module Prefabs");
            EditorGUILayout.PropertyField(modules, true);
            EndSection();
            
            sobject.ApplyModifiedProperties();
        }
    }
}