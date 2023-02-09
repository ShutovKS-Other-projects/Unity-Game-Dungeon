//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class PanelWidget : WidgetBase
    {
        protected IWidget Content;

        public PanelWidget() { }

        public PanelWidget(IWidget content)
        {
            this.Content = content;
        }

        public PanelWidget SetContent(IWidget content)
        {
            this.Content = content;
            return this;
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            if (Content != null)
            {
                var contentBounds = new Rect(Vector2.zero, WidgetBounds.size);
                Content.UpdateWidget(uiSystem, contentBounds);
            }
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
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
            if (Content != null)
            {
                return Content.GetDesiredSize(size, uiSystem);
            }
            else
            {
                return base.GetDesiredSize(size, uiSystem);
            }
        }
    }
}
