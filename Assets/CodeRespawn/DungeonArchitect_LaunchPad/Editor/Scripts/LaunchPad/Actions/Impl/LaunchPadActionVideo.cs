//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad.Actions.Impl
{
    public class LaunchPadActionVideo : LaunchPadActionBase
    {
        string path;
        public LaunchPadActionVideo(string path)
        {
            this.path = path;
        }

        public override Texture2D GetIcon()
        {
            return ScreenPageLoader.LoadImageAsset("icons/font_awesome/icon_video");
        }

        public override string GetText()
        {
            return "Watch Demo";
        }

        public override void Execute()
        {
            OpenLink(path);
        }

        public override bool IsValid()
        {
            if (path.Length == 0)
            {
                return false;
            }
            return true;
        }

    }
}