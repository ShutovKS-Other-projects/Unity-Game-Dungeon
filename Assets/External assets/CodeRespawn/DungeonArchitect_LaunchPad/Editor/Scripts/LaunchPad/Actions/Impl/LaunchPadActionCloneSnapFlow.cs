//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad.Actions.Impl
{
    public class LaunchPadActionCloneSnapFlow : LaunchPadActionBase
    {
        string templatePath;
        bool resourcePath;
        public LaunchPadActionCloneSnapFlow(string templatePath, bool resourcePath)
        {
            this.templatePath = templatePath;
            this.resourcePath = resourcePath;
        }

        public override Texture2D GetIcon()
        {
            return ScreenPageLoader.LoadImageAsset("icons/font_awesome/icon_graph_new");
        }

        public override string GetText()
        {
            return "Clone Snap\r\nFlow Graph";
        }

        public override void Execute()
        {
            string targetPath;
            if (CloneAsset(templatePath, resourcePath, out targetPath, "Create a Snap Flow graph from template"))
            {
                PingAsset(targetPath);
                new LaunchPadActionOpenSnapFlow(targetPath).Execute();
            }
        }
    }
}
