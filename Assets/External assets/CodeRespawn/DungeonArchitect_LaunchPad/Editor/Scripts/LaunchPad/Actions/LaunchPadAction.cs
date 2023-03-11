//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.Editors.LaunchPad.Actions.Impl;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad.Actions
{
    public enum LaunchPadActionType
    {
        None,
        OpenFolder,
        OpenScene,
        OpenTheme,
        OpenSnapFlow,
        OpenGridFlow,
        OpenSnapGridFlow,
        CloneScene,
        CloneSceneAndBuild,
        CloneInfinityScene,
        CloneInfinitySceneAndBuild,
        CloneTheme,
        CloneSnapFlow,
        CloneSnapGridFlow,
        CloneGridFlow,
        Documentation,
        Video,
    }

    [System.Serializable]
    public class LaunchPadActionData
    {
        public string path = "";
        public bool resource_path = false;
        public bool readOnly = false;
        public string icon = "";
        public string title = "";
    }

    public interface ILaunchPadAction 
    {
        Texture2D GetIcon();
        string GetText();
        void Execute();
        bool IsValid();
    }

    public abstract class LaunchPadActionBase : ILaunchPadAction
    {
        public abstract Texture2D GetIcon();
        public abstract string GetText();
        public abstract void Execute();
        public virtual bool IsValid() { return true; }

        protected string ConvertResourceToAssetPath(string resourcePath)
        {
            var obj = Resources.Load<Object>(resourcePath);
            var assetPath = AssetDatabase.GetAssetPath(obj);
            return GetRelativePath(assetPath);
        }

        protected bool CloneAsset(string sourceTemplatePath, bool resourcePath, out string targetPath, string title)
        {
            string sourcePath;
            if (resourcePath)
            {
                var sourceResourcePath = "LaunchPad/templates/" + sourceTemplatePath;
                sourcePath = ConvertResourceToAssetPath(sourceResourcePath);
            }
            else
            {
                sourcePath = sourceTemplatePath;
            }
            var fileInfo = new System.IO.FileInfo(sourcePath);
            var extension = fileInfo.Extension.Length > 0 ? fileInfo.Extension.Substring(1) : "";
            targetPath = EditorUtility.SaveFilePanelInProject(title, fileInfo.Name, extension, "Please choose a path to saved the cloned asset(s)");
            if (targetPath.Length == 0) return false;
            return AssetDatabase.CopyAsset(sourcePath, targetPath);
        }

        protected string GetRelativePath(string path)
        {
            if (path.StartsWith(Application.dataPath))
            {
                path = "Asset" + path.Substring(Application.dataPath.Length);
            }
            return path;
        }

        protected string ExtractFilename(string path)
        {
            var fileInfo = new System.IO.FileInfo(path);
            return fileInfo.Name;
        }

        protected bool CloneTemplateReferencedAsset(Object templateObj, string destFolder, out string clonedObjPath)
        {
            var templateObjPath = AssetDatabase.GetAssetPath(templateObj);
            var templateObjFilename = ExtractFilename(templateObjPath);
            clonedObjPath = destFolder + templateObjFilename;
            clonedObjPath = AssetDatabase.GenerateUniqueAssetPath(clonedObjPath);
            return AssetDatabase.CopyAsset(templateObjPath, clonedObjPath);
        }

        protected void PingAsset(string path)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        protected void OpenLink(string path)
        {
            if (path.StartsWith("http"))
            {
                Application.OpenURL(path);
            }
        }

    }

    public class LaunchPadActionFactory
    {
        public static ILaunchPadAction Create(LaunchPadActionType actionType, LaunchPadActionData data)
        {
            switch(actionType)
            {
                case LaunchPadActionType.OpenFolder: return new LaunchPadActionOpenFolder(data.path);
                case LaunchPadActionType.OpenScene: return new LaunchPadActionOpenScene(data.path);
                case LaunchPadActionType.OpenTheme: return new LaunchPadActionOpenTheme(data.path);
                case LaunchPadActionType.OpenSnapFlow: return new LaunchPadActionOpenSnapFlow(data.path);
                case LaunchPadActionType.OpenGridFlow: return new LaunchPadActionOpenGridFlow(data.path, data.readOnly);
                case LaunchPadActionType.OpenSnapGridFlow: return new LaunchPadActionOpenSnapGridFlow(data.path, data.readOnly);
                case LaunchPadActionType.CloneScene: return new LaunchPadActionCloneScene(data.path, data.resource_path);
                case LaunchPadActionType.CloneSceneAndBuild: return new LaunchPadActionCloneSceneAndBuild(data.path, data.resource_path);
                case LaunchPadActionType.CloneInfinityScene: return new LaunchPadActionCloneInfinityScene(data.path, data.resource_path);
                case LaunchPadActionType.CloneInfinitySceneAndBuild: return new LaunchPadActionCloneInfinitySceneAndBuild(data.path, data.resource_path);
                case LaunchPadActionType.CloneTheme: return new LaunchPadActionCloneTheme(data.path, data.resource_path);
                case LaunchPadActionType.CloneSnapFlow: return new LaunchPadActionCloneSnapFlow(data.path, data.resource_path);
                case LaunchPadActionType.CloneSnapGridFlow: return new LaunchPadActionCloneSnapGridFlow(data.path, data.resource_path);
                case LaunchPadActionType.CloneGridFlow: return new LaunchPadActionCloneGridFlow(data.path, data.resource_path);
                case LaunchPadActionType.Documentation: return new LaunchPadActionDocumentation(data.path);
                case LaunchPadActionType.Video: return new LaunchPadActionVideo(data.path);
                default: return null;
            }
        }
    }
}
