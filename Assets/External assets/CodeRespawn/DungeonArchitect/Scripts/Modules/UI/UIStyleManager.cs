//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI
{
    public interface UIStyleManager
    {
        GUIStyle GetToolbarButtonStyle();
        GUIStyle GetButtonStyle();
        GUIStyle GetBoxStyle();
        GUIStyle GetLabelStyle();
        GUIStyle GetBoldLabelStyle();
        Font GetFontStandard();
        Font GetFontBold();
        Font GetFontMini();
    }
}