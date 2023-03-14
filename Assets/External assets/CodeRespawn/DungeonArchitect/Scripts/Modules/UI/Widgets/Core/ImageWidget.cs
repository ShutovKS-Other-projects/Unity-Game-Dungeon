//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public enum ImageWidgetDrawMode
    {
        Streach,
        Fit,
        Fill,
        Fixed
    }

    public class ImageWidget : WidgetBase
    {
        Texture2D texture;
        ImageWidgetDrawMode drawMode = ImageWidgetDrawMode.Streach;
        Color tint = Color.white;

        public ImageWidget() {}
        public ImageWidget(Texture2D texture)
        {
            this.texture = texture;
        }

        public ImageWidget SetImage(Texture2D texture)
        {
            this.texture = texture;
            return this;
        }

        public ImageWidget SetDrawMode(ImageWidgetDrawMode drawMode)
        {
            this.drawMode = drawMode;
            return this;
        }

        public ImageWidget SetTint(Color tint)
        {
            this.tint = tint;
            return this;
        }

        Vector2 CalculateFitSize(Vector2 size)
        {
            if (texture == null) return size;

            var scale = size.x / texture.width;
            scale = Mathf.Min(1, scale);
            var sizeX = texture.width * scale;
            var sizeY = texture.height * scale;
            return new Vector2(sizeX, sizeY);
        }

        Vector2 CalculateFillSize(Vector2 size)
        {
            if (texture == null) return size;

            var scaleX = size.x / texture.width;
            var scaleY = size.y / texture.height;

            var scale = Mathf.Max(scaleX, scaleY);
            
            var sizeX = texture.width * scale;
            var sizeY = texture.height * scale;
            return new Vector2(sizeX, sizeY);
        }

        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            if (texture == null)
            {
                return size;
            }

            if (drawMode == ImageWidgetDrawMode.Fit)
            {
                return CalculateFitSize(size);
            }
            else if (drawMode == ImageWidgetDrawMode.Fill)
            {
                return CalculateFillSize(size);
            }
            else if (drawMode == ImageWidgetDrawMode.Fit)
            {
                return new Vector2(texture.width, texture.height);
            }
            else
            {
                return new Vector2(texture.width, texture.height);
            }
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            if (IsPaintEvent(uiSystem) && texture != null)
            {
                var size = WidgetBounds.size;
                size.x--;
                var bounds = new Rect(Vector2.zero, size);
                var state = new GUIState(renderer);

                var drawSize = bounds.size;
                Vector2 offset = Vector2.zero;
                if (drawMode == ImageWidgetDrawMode.Fit)
                {
                    drawSize = CalculateFitSize(size);
                    var offsetX = (bounds.size.x - drawSize.x) * 0.5f;
                    offset = new Vector2(offsetX, 0);
                }
                else if (drawMode == ImageWidgetDrawMode.Fill)
                {
                    drawSize = CalculateFillSize(size);
                    var offsetX = (bounds.size.x - drawSize.x) * 0.5f;
                    var offsetY = (bounds.size.y - drawSize.y) * 0.5f;
                    offset = new Vector2(offsetX, offsetY);
                }
                else if (drawMode == ImageWidgetDrawMode.Fixed)
                {
                    drawSize = new Vector2(texture.width, texture.height);
                    var offsetX = (bounds.size.x - drawSize.x) * 0.5f;
                    var offsetY = (bounds.size.y - drawSize.y) * 0.5f;
                    offset = new Vector2(offsetX, offsetY);
                }

                var drawBounds = new Rect(bounds.position + offset, drawSize);
                renderer.DrawTexture(drawBounds, texture, ScaleMode.StretchToFill, true, tint);
                state.Restore();
            }
        }
    }
}
