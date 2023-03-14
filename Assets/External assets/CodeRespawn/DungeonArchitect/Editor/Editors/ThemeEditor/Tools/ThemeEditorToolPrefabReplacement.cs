//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;


namespace DungeonArchitect.Editors.ThemeEditorTools
{

    public class ThemeEditorToolPrefabReplacement
    {
        [ThemeEditorTool("Advanced/Bulk Replace Prefab", 200)]
        public static void BuildReplacePrefab(DungeonThemeEditorWindow editor)
        {
            var window = (BulkPrefabReplacementWindow)EditorWindow.GetWindow(typeof(BulkPrefabReplacementWindow));
            if (window != null)
            {
                window.titleContent = new GUIContent("Bulk Prefab Replacer");
                window.GraphEditor = editor;
                window.Show();
            }
        }
    }

    public enum BulkPrefabReplacementRuleType
    {
        Directory
    }

    interface IBulkPrefabReplaceRuleProcessor
    {
        void DrawGUI();
        BulkReplacementEntry[] Process(DungeonThemeEditorWindow editor);
    }

    class BulkReplacementEntry
    {
        public GameObjectNode node;
        public FileInfo currentPrefabInfo;
        public FileInfo replacementPrefabInfo;
    }

    class DirectoryBulkPrefabReplaceRuleProcessor : IBulkPrefabReplaceRuleProcessor
    {
        string targetDirectory = "";
        public void DrawGUI()
        {
            targetDirectory = EditorGUILayout.TextField("Target Directory", targetDirectory);
        }

        
        /// <summary>
        /// Returns a map of the filename (not path just name) to file info mapping for all target directory prefabs
        /// </summary>
        /// <returns></returns>
        Dictionary<string, FileInfo> GetTargetFiles()
        {
            var result = new Dictionary<string, FileInfo>();
            var targetAssetGuids = AssetDatabase.FindAssets("", new string[] { targetDirectory });
            for (int i = 0; i < targetAssetGuids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(targetAssetGuids[i]);
                var fileInfo = new FileInfo(path);
                if (!result.ContainsKey(fileInfo.Name))
                {
                    result.Add(fileInfo.Name, fileInfo);
                }
            }
            return result;
        }
        

        public BulkReplacementEntry[] Process(DungeonThemeEditorWindow editor)
        {
            var graphEditor = editor.GraphEditor;
            var targetFiles = GetTargetFiles();
            var replacements = new List<BulkReplacementEntry>();
            foreach (var node in graphEditor.Graph.Nodes)
            {
                if (node is GameObjectNode)
                {
                    var goNode = node as GameObjectNode;
                    if (goNode.Template != null) {
                        string path = AssetDatabase.GetAssetPath(goNode.Template);
                        var pathInfo = new FileInfo(path);
                        if (pathInfo != null)
                        {
                            if (targetFiles.ContainsKey(pathInfo.Name))
                            {
                                // Found a target file. Replace
                                var replacementEntry = new BulkReplacementEntry();
                                replacementEntry.node = goNode;
                                replacementEntry.currentPrefabInfo = pathInfo;
                                replacementEntry.replacementPrefabInfo = targetFiles[pathInfo.Name];

                                // Make sure the current and target are not the same prefabs, to avoid unnecessary replacements
                                if (replacementEntry.currentPrefabInfo.FullName != replacementEntry.replacementPrefabInfo.FullName)
                                {
                                    replacements.Add(replacementEntry);
                                }
                            }
                        }
                    }
                }
            }

            return replacements.ToArray();
        }
    }

    class BulkPrefabReplaceRuleFactory
    {
        public static IBulkPrefabReplaceRuleProcessor CreateProcessor(BulkPrefabReplacementRuleType RuleType)
        {
            switch (RuleType)
            {
                case BulkPrefabReplacementRuleType.Directory:
                    return new DirectoryBulkPrefabReplaceRuleProcessor();

                default:
                    return null;
            }
        }
    }

    class BulkPrefabReplacementWindow : EditorWindow
    {
        BulkPrefabReplacementRuleType ruleType = BulkPrefabReplacementRuleType.Directory;
        IBulkPrefabReplaceRuleProcessor ruleProcessor = null;
        DungeonThemeEditorWindow graphEditor = null;
        public DungeonArchitect.Editors.DungeonThemeEditorWindow GraphEditor
        {
            get { return graphEditor; }
            set { graphEditor = value; }
        }

        void OnGUI()
        {
            GUILayout.Label("Bulk Prefab Replacer", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("Bulk Replace prefabs in your theme node based on a rule.  Backup your theme file before proceeding", MessageType.Info);

            GUI.changed = false;
            ruleType = (BulkPrefabReplacementRuleType)EditorGUILayout.EnumPopup("Replacement Rule:", ruleType);
            if (ruleProcessor == null || GUI.changed)
            {
                ruleProcessor = BulkPrefabReplaceRuleFactory.CreateProcessor(ruleType);
            }
            
            BeginIndent();

            EditorGUILayout.HelpBox(GetRuleDescription(ruleType), MessageType.None);

            if (ruleProcessor != null)
            {
                ruleProcessor.DrawGUI();
                if (GUILayout.Button("Bulk Replace"))
                {
                    var replacements = ruleProcessor.Process(graphEditor);
                    bool replaced = PerformReplacements(replacements);
                    if (replaced)
                    {
                        // Refresh the thumbnails
                        CommonThemeEditorTools.RefreshThumbnails(graphEditor);
                    }
                }
            }

            EndIndent();
        }

        bool PerformReplacements(BulkReplacementEntry[] replacements)
        {
            if (replacements.Length == 0)
            {
                EditorUtility.DisplayDialog("Bulk Replacement", "Nothing to replace", "Ok");
                return false;
            }

            // Show confirmation
            bool confirm = EditorUtility.DisplayDialog("Perform replacements?", GetReplacementReport(replacements), "Replace", "Cancel");
            if (!confirm)
            {
                return false;
            }

            foreach (var replacement in replacements) {
                var path = ChopPreAssetPath(replacement.replacementPrefabInfo.FullName);
                var template = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                replacement.node.Template = template;
            }

            return true;
        }

        static string ChopPreAssetPath(string path)
        {
            int index = path.IndexOf("Assets");
            return (index != -1) ? path.Substring(index) : path;
        }

        string GetReplacementReport(BulkReplacementEntry[] replacements)
        {
            var report = new System.Text.StringBuilder();
            foreach (var replacement in replacements)
            {
                report.AppendFormat("{0} => {1},    ", 
                    ChopPreAssetPath(replacement.currentPrefabInfo.FullName),
                    ChopPreAssetPath(replacement.replacementPrefabInfo.FullName));
            }
            return report.ToString();
        }

        static string GetRuleDescription(BulkPrefabReplacementRuleType ruleType)
        {
            switch (ruleType)
            {
                case BulkPrefabReplacementRuleType.Directory:
                    return "Replaces with similar named prefabs found in the target directory for each visual graph node in the current theme graph.   Does nothing if not found. Directory path should start with Asset/...";

                default:
                    return "";
            }
        }

        static readonly int INDENT_SPACING = 10;

        void BeginIndent()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(INDENT_SPACING);
            EditorGUILayout.BeginVertical();
        }

        void EndIndent()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

}