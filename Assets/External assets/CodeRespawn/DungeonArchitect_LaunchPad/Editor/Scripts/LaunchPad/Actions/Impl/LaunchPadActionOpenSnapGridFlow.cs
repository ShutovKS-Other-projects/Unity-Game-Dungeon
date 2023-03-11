//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.Editors.Flow.Impl;
using DungeonArchitect.Flow.Impl.SnapGridFlow;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad.Actions.Impl
{
    public class LaunchPadActionOpenSnapGridFlow : LaunchPadActionBase
    {
        string path;
        bool readOnly;
        SnapGridFlowModuleDatabase moduleDatabase;

        public LaunchPadActionOpenSnapGridFlow(string path)
            : this(path, false, null)
        {
        }

        public LaunchPadActionOpenSnapGridFlow(string path, bool readOnly)
            : this(path, readOnly, null)
        {
            
        }

        public LaunchPadActionOpenSnapGridFlow(string path, bool readOnly, SnapGridFlowModuleDatabase moduleDatabase)
        {
            this.path = path;
            this.readOnly = readOnly;
            this.moduleDatabase = moduleDatabase;
        }

        public override Texture2D GetIcon()
        {
            return ScreenPageLoader.LoadImageAsset("icons/font_awesome/icon_gridflow");
        }

        public override string GetText()
        {
            return "Open Snap\r\nGrid Flow";
        }

        public override void Execute()
        {
            var flowAsset = AssetDatabase.LoadAssetAtPath<SnapGridFlowAsset>(path);
            if (flowAsset != null)
            {
                var window = EditorWindow.GetWindow<SnapGridFlowEditorWindow>();
                if (window != null)
                {
                    window.Init(flowAsset);
                    if (moduleDatabase != null)
                    {
                        window.SetModuleDatabase(moduleDatabase);
                    }
                    window.HandleExecuteButtonPressed();
                    window.SetReadOnly(readOnly);
                }
            }
        }
    }
}