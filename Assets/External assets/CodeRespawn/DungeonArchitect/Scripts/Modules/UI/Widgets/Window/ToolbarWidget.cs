//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class ToolbarWidget : WidgetBase
    {
        public float ButtonSize = 20;
        public float Padding = 0;
        public Color Background = new Color(0, 0, 0, 0.25f);
        public delegate void OnButtonPressed(UISystem uiSystem, string id);
        public event OnButtonPressed ButtonPressed;
        public List<ButtonInfo> buttons = new List<ButtonInfo>();

        public class ButtonInfo
        {
            public string ButtonId;
            public string IconId;
            public Rect Bounds;
        }
        GUIStyle buttonStyle;

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            var size = new Vector2(
                Padding * 2 + buttons.Count * ButtonSize,
                Padding * 2 + ButtonSize);
            WidgetBounds = new Rect(WidgetBounds.position, size);
            UpdateButtonBounds();
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            if (buttonStyle == null)
            {
                var skin = renderer.GetResource<GUISkin>(UIResourceLookup.SKIN_TOOLBAR_BUTTONS) as GUISkin;
                buttonStyle = skin.button;
            }
            if (buttonStyle == null)
            {
                buttonStyle = renderer.StyleManager.GetToolbarButtonStyle();
            }

            if (IsPaintEvent(uiSystem))
            {
                var toolbarBounds = WidgetBounds;
                renderer.DrawRect(toolbarBounds, Background);
            }

            foreach (var button in buttons)
            {
                var icon = renderer.GetResource<Texture>(button.IconId) as Texture;
                if (renderer.Button(button.Bounds, new GUIContent(icon), buttonStyle))
                {
                    if (ButtonPressed != null)
                    {
                        ButtonPressed.Invoke(uiSystem, button.ButtonId);
                    }
                }
            }
        }

        public void AddButton(string buttonId, string iconId)
        {
            var button = new ButtonInfo();
            button.ButtonId = buttonId;
            button.IconId = iconId;
            buttons.Add(button);
        }

        void UpdateButtonBounds()
        {
            float x = Padding;
            float y = Padding;
            foreach (var button in buttons)
            {
                button.Bounds = new Rect(x, y, ButtonSize, ButtonSize);
                x += ButtonSize;
            }
        }

        public ToolbarWidget SetBackground(Color background)
        {
            this.Background = background;
            return this;
        }

        public ToolbarWidget SetButtonSize(int buttonSize)
        {
            this.ButtonSize = buttonSize;
            return this;
        }

    }
}
