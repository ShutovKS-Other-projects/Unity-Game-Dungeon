//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad.Actions.Impl
{
    public class LaunchPadActionOpenFolder : LaunchPadActionBase
    {
        string path;
        public LaunchPadActionOpenFolder(string path)
        {
            this.path = path;
        }

        public override Texture2D GetIcon()
        {
            return ScreenPageLoader.LoadImageAsset("icons/font_awesome/icon_open_folder");
        }

        public override string GetText()
        {
            return "Open Folder";
        }

        public override void Execute()
        {
            PingAsset(path);
        }
    }
}