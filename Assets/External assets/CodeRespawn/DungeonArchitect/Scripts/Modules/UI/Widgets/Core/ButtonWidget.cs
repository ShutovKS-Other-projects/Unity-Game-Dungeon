//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class ButtonWidget : WidgetBase
    {
        GUIContent content;
        Color color = new Color(0.8f, 0.8f, 0.8f);

        public delegate void OnButtonPressed(UISystem uiSystem);
        public event OnButtonPressed ButtonPressed;

        public ButtonWidget(GUIContent content)
        {
            this.content = content;
        }

        public ButtonWidget SetColor(Color color)
        {
            this.color = color;
            return this;
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.black;

            var state = new GUIState(renderer);
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            renderer.color = color;
            if (renderer.Button(bounds, content, style))
            {
                if (ButtonPressed != null)
                {
                    ButtonPressed.Invoke(uiSystem);
                }
            }
            state.Restore();
        }
    }
}
