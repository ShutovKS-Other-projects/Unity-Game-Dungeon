//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.UI.Impl.UnityEditor
{
    public class UnityEditorUIStyleManager : UIStyleManager
    {
        public GUIStyle GetToolbarButtonStyle()
        {
            return new GUIStyle(EditorStyles.toolbarButton);
        }

        public GUIStyle GetButtonStyle()
        {
            return new GUIStyle(GUI.skin.button);
        }

        public GUIStyle GetBoxStyle()
        {
            return new GUIStyle(GUI.skin.box);
        }

        public GUIStyle GetLabelStyle()
        {
            return new GUIStyle(GUI.skin.label);
        }

        public GUIStyle GetBoldLabelStyle()
        {
            return new GUIStyle(EditorStyles.boldLabel);
        }

        public Font GetFontStandard()
        {
            return EditorStyles.standardFont;
        }

        public Font GetFontBold()
        {
            return EditorStyles.boldFont;
        }

        public Font GetFontMini()
        {
            return EditorStyles.miniFont;
        }
    }
}
