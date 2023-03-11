//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad.Actions.Impl
{
    public class LaunchPadActionCloneGridFlow : LaunchPadActionBase
    {
        string templatePath;
        bool resourcePath;
        public LaunchPadActionCloneGridFlow(string templatePath, bool resourcePath)
        {
            this.templatePath = templatePath;
            this.resourcePath = resourcePath;
        }

        public override Texture2D GetIcon()
        {
            return ScreenPageLoader.LoadImageAsset("icons/font_awesome/icon_gridflow_new");
        }

        public override string GetText()
        {
            return "Clone Grid\r\nFlow Graph";
        }

        public override void Execute()
        {
            string targetPath;
            if (CloneAsset(templatePath, resourcePath, out targetPath, "Create a Grid Flow Graph from Template"))
            {
                PingAsset(targetPath);
                new LaunchPadActionOpenGridFlow(targetPath, false).Execute();
            }
        }
    }
}
