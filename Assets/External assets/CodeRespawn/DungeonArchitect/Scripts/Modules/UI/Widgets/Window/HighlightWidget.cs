//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DMathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.UI.Widgets
{
    public class HighlightWidget : WidgetBase
    {
        public IWidget Widget;
        public object ObjectOfInterest;
        public Color HighlightColor = Color.red;
        public float HighlightThickness = 3.0f;
        public float HighlightTime = 1.0f;
        double lastUpdateTime = 0;

        float remainingTime = 0;
        Texture2D lineTexture;
        

        public HighlightWidget()
        { 
            lastUpdateTime = 0;
        }

        public HighlightWidget SetContent(IWidget widget)
        {
            this.Widget = widget;
            return this;
        }

        public HighlightWidget SetHighlightColor(Color highlightColor)
        {
            this.HighlightColor = highlightColor;
            return this;
        }

        public HighlightWidget SetHighlightThickness(float highlightThickness)
        {
            this.HighlightThickness = highlightThickness;
            return this;
        }

        public HighlightWidget SetHighlightTime(float highlightTime)
        {
            this.HighlightTime = highlightTime;
            return this;
        }

        public HighlightWidget SetObjectOfInterest(object objectOfInterest)
        {
            this.ObjectOfInterest = objectOfInterest;
            return this;
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            if (lineTexture == null)
            {
                lineTexture = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_WHITE_16x) as Texture2D;
            }

            if (Widget != null)
            {
                WidgetUtils.DrawWidgetGroup(uiSystem, renderer, Widget);
            }

            if (remainingTime > 0 && IsPaintEvent(uiSystem))
            {
                var bounds = new Rect(Vector2.zero, WidgetBounds.size);
                bounds = DMathUtils.ExpandRect(bounds, -HighlightThickness * 0.5f);
                float intensity = Mathf.Sin(Mathf.PI * remainingTime * 2);
                intensity = Mathf.Abs(intensity);
                var color = HighlightColor;
                color.a *= intensity;
                WidgetUtils.DrawWidgetFocusHighlight(renderer, bounds, color, HighlightThickness, lineTexture);
            }
        }

        public void Activate(UISystem uiSystem)
        {
            remainingTime = HighlightTime;
            lastUpdateTime = uiSystem.Platform.timeSinceStartup;
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            if (remainingTime > 0)
            {
                double currentTime = uiSystem.Platform.timeSinceStartup;
                float deltaTime = (float)(currentTime - lastUpdateTime);

                remainingTime -= deltaTime;
                remainingTime = Mathf.Max(0, remainingTime);
                lastUpdateTime = currentTime;
            }

            if (Widget != null)
            {
                var contentBounds = new Rect(Vector2.zero, WidgetBounds.size);
                Widget.UpdateWidget(uiSystem, contentBounds);
            }

        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return Widget != null ? new[] { Widget } : null;
        }

    }
}
