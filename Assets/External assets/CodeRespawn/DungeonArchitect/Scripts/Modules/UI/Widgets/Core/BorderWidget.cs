//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class BorderWidget : WidgetBase
    {
        IWidget Content;

        float paddingLeft = 5;
        float paddingTop = 26;
        float paddingRight = 5;
        float paddingBottom = 5;
        Color color = new Color(0.3f, 0.3f, 0.3f);
        Color borderColor = Color.black;
        string title = "";
        int titleFontSize = 14;
        Color titleColor = new Color(0.75f, 0.75f, 0.75f);
        Vector2 titleOffset = new Vector2(5, 4);
        bool drawOutline = true;

        IWidget titleWidget;
        System.Func<string> TitleGetter;

        public BorderWidget() { }
        public BorderWidget(IWidget content)
        {
            this.Content = content;
        }

        public BorderWidget SetContent(IWidget content)
        {
            this.Content = content;
            return this;
        }

        public BorderWidget SetPadding(float left, float top, float right, float bottom)
        {
            paddingLeft = left;
            paddingTop = top;
            paddingRight = right;
            paddingBottom = bottom;
            return this;
        }

        public BorderWidget SetColor(Color color)
        {
            this.color = color;
            return this;
        }

        public BorderWidget SetBorderColor(Color borderColor)
        {
            this.borderColor = borderColor;
            return this;
        }

        public BorderWidget SetTransparent()
        {
            this.color = new Color(0, 0, 0, 0);
            this.borderColor = new Color(0, 0, 0, 0);
            return this;
        }

        public BorderWidget SetTitle(string title)
        {
            this.title = title;
            return this;
        }

        public BorderWidget SetTitleGetter(System.Func<string> getter)
        {
            this.TitleGetter = new System.Func<string>(getter);
            return this;
        }

        public BorderWidget SetTitleFontSize(int size)
        {
            this.titleFontSize = size;
            return this;
        }

        public BorderWidget SetTitleColor(Color color)
        {
            this.titleColor = color;
            return this;
        }

        public BorderWidget SetTitleOffset(Vector2 offset)
        {
            this.titleOffset = offset;
            return this;
        }

        public BorderWidget SetTitleWidget(IWidget widget)
        {
            this.titleWidget = widget;
            return this;
        }

        public BorderWidget SetDrawOutline(bool drawOutline)
        {
            this.drawOutline = drawOutline;
            return this;
        }


        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            var contentBounds = new Rect(Vector2.zero, WidgetBounds.size);
            contentBounds.x += paddingLeft;
            contentBounds.y += paddingTop;
            contentBounds.width -= paddingLeft + paddingRight;
            contentBounds.height -= paddingTop + paddingBottom;
            Content.UpdateWidget(uiSystem, contentBounds);

            if (titleWidget != null)
            {
                titleWidget.UpdateWidget(uiSystem, bounds);
            }
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            Rect borderBounds = new Rect(Vector2.zero, WidgetBounds.size);
            
            if (IsPaintEvent(uiSystem))
            {
                // Draw the border
                renderer.DrawRect(borderBounds, color);

                if (drawOutline)
                {
                    WidgetUtils.DrawWidgetFocusHighlight(renderer, borderBounds, borderColor);
                }
            }
            
                // Draw the label
                if (titleWidget != null)
                {
                    titleWidget.Draw(uiSystem, renderer);
                }
                else if (title.Length > 0 || TitleGetter != null)
                {
                    if (IsPaintEvent(uiSystem))
                    {
                        var titleStyle = new GUIStyle(GUI.skin.label);
                        titleStyle.fontSize = titleFontSize;
                        titleStyle.normal.textColor = titleColor;
                        titleStyle.alignment = TextAnchor.UpperLeft;
                        var titleBounds = borderBounds;
                        titleBounds.position += titleOffset;
                        string caption = title;
                        if (TitleGetter != null)
                        {
                            caption = TitleGetter();
                        }

                        renderer.Label(titleBounds, caption, titleStyle);
                    }
                }

            // Draw the content
            if (Content != null)
            {
                WidgetUtils.DrawWidgetGroup(uiSystem, renderer, Content);
            }
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { Content };
        }

        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            var contentSize = size;
            contentSize.x -= paddingLeft + paddingRight;
            contentSize.y -= paddingTop + paddingBottom;
            var desiredSize = Content.GetDesiredSize(contentSize, uiSystem);
            desiredSize.x += paddingLeft + paddingRight;
            desiredSize.y += paddingTop + paddingBottom;
            return desiredSize;
        }
    }
}
