//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class LinkWidget : WidgetBase
    {
        IWidget content;
        bool drawLinkOutline = true;
        bool hovered = false;
        object userdata = null;
        Color highlightColor = WidgetUtils.FOCUS_HIGHLITE_COLOR;

        public event OnWidgetClicked LinkClicked;

        public LinkWidget() : this(null) { }
        public LinkWidget(IWidget content)
        {
            this.content = content;
        }

        public LinkWidget SetContent(IWidget content)
        {
            this.content = content;
            return this;
        }

        public LinkWidget SetHighlightColor(Color color)
        {
            this.highlightColor = color;
            return this;
        }

        public LinkWidget SetUserData(object userdata)
        {
            this.userdata = userdata;
            return this;
        }

        public LinkWidget SetDrawLinkOutline(bool drawLinkOutline)
        {
            this.drawLinkOutline = drawLinkOutline;
            return this;
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            var contentBounds = new Rect(Vector2.zero, WidgetBounds.size);
            content.UpdateWidget(uiSystem, contentBounds);
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            if (content != null)
            {
                Rect linkBounds = new Rect(Vector2.zero, WidgetBounds.size);
                renderer.AddCursorRect(linkBounds, UICursorType.Link);

                WidgetUtils.DrawWidgetGroup(uiSystem, renderer, content);

                var eventType = uiSystem.Platform.CurrentEvent.type;
                if (eventType == EventType.Repaint || eventType == EventType.MouseMove)
                {
                    if (hovered && drawLinkOutline)
                    {
                        WidgetUtils.DrawWidgetFocusHighlight(renderer, linkBounds, highlightColor);
                    }
                }
            }
            hovered = false;
        }

        public override bool RequiresInputEveryFrame() { return true; }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { content };
        }

        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            return content.GetDesiredSize(size, uiSystem);
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            base.HandleInput(e, uiSystem);

            Rect linkBounds = new Rect(Vector2.zero, WidgetBounds.size);
            bool insideBounds = linkBounds.Contains(e.mousePosition);

            hovered = insideBounds;
            if (e.type == EventType.MouseDown && insideBounds)
            {
                if (LinkClicked != null)
                {
                    var clickEvent = new WidgetClickEvent();
                    clickEvent.e = e;
                    clickEvent.uiSystem = uiSystem;
                    clickEvent.userdata = userdata;
                    LinkClicked.Invoke(clickEvent);
                }
            }
        }

    }
}
