//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class ScrollPanelWidget : WidgetBase
    {
        IWidget content;
        bool MouseScrollingEnabled = true;

        public ScrollPanelWidget(IWidget content)
            : this(content, true)
        {
        }

        public ScrollPanelWidget(IWidget content, bool mouseScrollingEnabled)
        {
            this.content = content;
            this.MouseScrollingEnabled = mouseScrollingEnabled;
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            ScrollPosition = renderer.BeginScrollView(bounds, ScrollPosition, content.WidgetBounds);
            content.Draw(uiSystem, renderer);
            renderer.EndScrollView(MouseScrollingEnabled);
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            var contentSize = content.GetDesiredSize(bounds.size, uiSystem);
            contentSize.x = Mathf.Max(contentSize.x, bounds.size.x);
            if (contentSize.y > bounds.height)
            {
                contentSize.x -= 16;
            }
            var contentBounds = new Rect(Vector2.zero, contentSize);
            content.UpdateWidget(uiSystem, contentBounds);
        }

        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            return content.GetDesiredSize(size, uiSystem);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { content };
        }
    }
}
