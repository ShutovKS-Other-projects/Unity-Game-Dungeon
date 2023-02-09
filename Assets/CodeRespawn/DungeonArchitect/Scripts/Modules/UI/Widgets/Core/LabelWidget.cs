//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class LabelWidget : WidgetBase
    {
        GUIStyle style;
        public string Text { get; private set; }
        public Color Color { get; private set; }
        public int FontSize { get; private set; }
        public bool WordWrap { get; set; }
        public TextAnchor TextAlign { get; private set; }

        public LabelWidget(string text)
        {
            this.Text = text;
            Color = Color.black;
            WordWrap = false;
            TextAlign = TextAnchor.UpperLeft;
        }

        public LabelWidget SetColor(Color color)
        {
            this.Color = color;
            style = null;
            return this;
        }

        public LabelWidget SetFontSize(int size)
        {
            this.FontSize = size;
            style = null;
            return this;
        }

        public LabelWidget SetTextAlign(TextAnchor align)
        {
            this.TextAlign = align;
            style = null;
            return this;
        }

        public LabelWidget SetWordWrap(bool wordWrap)
        {
            this.WordWrap = wordWrap;
            style = null;
            return this;
        }

        GUIStyle CreateStyle(UIStyleManager styleManager)
        {
            var style = styleManager.GetLabelStyle();
            style.fontSize = FontSize;
            style.alignment = TextAlign;
            style.normal.textColor = Color;
            style.wordWrap = WordWrap;
            return style;
        }

        public float CalcHeight(UIStyleManager styleManager, string text, float width)
        {
            if (style == null)
            {
                style = CreateStyle(styleManager);
            }
            return style.CalcHeight(new GUIContent(text), width);
        }

        public Vector2 CalcSize(UISystem uiSystem, string text)
        {
            if (style == null)
            {
                style = CreateStyle(uiSystem.StyleManager);
            }
            return style.CalcSize(new GUIContent(text));
        }

        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            if (WordWrap)
            {
                var height = CalcHeight(uiSystem.StyleManager, Text, size.x);
                return new Vector2(size.x, height);
            }
            else
            {
                return CalcSize(uiSystem, Text);
            }
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            if (IsPaintEvent(uiSystem))
            {
                var bounds = new Rect(Vector2.zero, WidgetBounds.size);
                if (style == null)
                {
                    style = CreateStyle(uiSystem.StyleManager);
                }
                renderer.Label(bounds, Text, style);
            }
        }
    }
}
