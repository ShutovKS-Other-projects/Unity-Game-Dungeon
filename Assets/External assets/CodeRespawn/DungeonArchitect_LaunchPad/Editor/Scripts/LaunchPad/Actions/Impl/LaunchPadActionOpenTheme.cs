//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.Graphs;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad.Actions.Impl
{
    public class LaunchPadActionOpenTheme : LaunchPadActionBase
    {
        string path;
        public LaunchPadActionOpenTheme(string path)
        {
            this.path = path;
        }

        public override Texture2D GetIcon()
        {
            return ScreenPageLoader.LoadImageAsset("icons/font_awesome/icon_theme");
        }

        public override string GetText()
        {
            return "Open Theme";
        }

        public override void Execute()
        {
            var graph = AssetDatabase.LoadAssetAtPath<Graph>(path);
            if (graph != null)
            {
                var window = EditorWindow.GetWindow<DungeonThemeEditorWindow>();
                if (window != null)
                {
                    window.Init(graph);
                }
            }
        }
    }
}