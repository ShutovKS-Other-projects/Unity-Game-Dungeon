//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.Graphs;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonArchitect.Editors.LaunchPad.Actions.Impl
{
    public class LaunchPadActionCloneInfinityScene : LaunchPadActionBase
    {
        string templatePath;
        bool resourcePath;
        public LaunchPadActionCloneInfinityScene(string templatePath, bool resourcePath)
        {
            this.templatePath = templatePath;
            this.resourcePath = resourcePath;
        }

        public override Texture2D GetIcon()
        {
            return ScreenPageLoader.LoadImageAsset("icons/create_scene");
        }

        public override string GetText()
        {
            return "Clone Scene";
        }

        protected virtual bool ShouldRebuildDungeon() { return false; }

        public override void Execute()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Bailing out of clone");
                return;
            }

            string targetPath;
            if (CloneAsset(templatePath, resourcePath, out targetPath, "Create a Scene from Template"))
            {
                PingAsset(targetPath);
                EditorSceneManager.OpenScene(targetPath);

                var dungeons = GameObject.FindObjectsOfType<InfinityDungeon>();

                var destFileInfo = new System.IO.FileInfo(targetPath);
                var destFilename = destFileInfo.Name;
                var destFolder = targetPath.Substring(0, targetPath.Length - destFilename.Length);

                foreach (var dungeon in dungeons)
                {
                    var clonedThemes = new List<Graph>();
                    foreach (var theme in dungeon.dungeonThemes)
                    {
                        // Clone the themes
                        string destThemePath;
                        if (CloneTemplateReferencedAsset(theme, destFolder, out destThemePath))
                        {
                            var clonedTheme = AssetDatabase.LoadAssetAtPath<Graph>(destThemePath);
                            clonedThemes.Add(clonedTheme);
                        }
                        else
                        {
                            Debug.Log("Failed to copy referenced theme files while cloning the scene");
                        }
                    }

                    dungeon.dungeonThemes.Clear();
                    dungeon.dungeonThemes.AddRange(clonedThemes);
                    EditorUtility.SetDirty(dungeon);

                    if (ShouldRebuildDungeon())
                    {
                        dungeon.BuildDungeon();
                    }
                }
                if (dungeons.Length > 0)
                {
                    Selection.activeGameObject = dungeons[0].gameObject;
                }

                var currentScene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(currentScene);
                EditorSceneManager.SaveScene(currentScene);

                // Ping the cloned scene asset
                {
                    Object sceneAsset = AssetDatabase.LoadAssetAtPath<Object>(targetPath);
                    EditorGUIUtility.PingObject(sceneAsset);
                }

            }
        }
    }

    public class LaunchPadActionCloneInfinitySceneAndBuild : LaunchPadActionCloneInfinityScene
    {
        public LaunchPadActionCloneInfinitySceneAndBuild(string templatePath, bool resourcePath) : base(templatePath, resourcePath) { }
        protected override bool ShouldRebuildDungeon() { return true; }
    }
}
