//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad
{
    public class LaunchPadCardWidgetData
    {
        public string title;
        public string description;
        public Texture2D thumbnail;
        public string link = "";
        public string url;
    }

    public class LaunchPadCardWidget : WidgetBase
    {
        LaunchPadCardWidgetData data;
        IWidget layout;
        ImageWidget thumbnailWidget;
        LabelWidget titleWidget;
        LabelWidget descriptionWidget;
        float thumbnailHeight = 100;
        bool showDescription;
        public event OnScreenPageLinkClicked LinkClicked;


        public LaunchPadCardWidget(LaunchPadCardWidgetData data, bool showDescription)
        {
            this.data = data;
            this.showDescription = showDescription;
        }

        void BuildLayout(UISystem uiSystem)
        {
            thumbnailWidget =
                new ImageWidget(data.thumbnail)
                .SetDrawMode(ImageWidgetDrawMode.Fill);

            titleWidget =
                new LabelWidget(data.title)
                    .SetFontSize(18)
                    .SetTextAlign(TextAnchor.MiddleCenter)
                    .SetColor(new Color(0.85f, 0.85f, 0.85f));

            if (showDescription)
            {
                descriptionWidget =
                    new LabelWidget(data.description)
                        .SetFontSize(14)
                        .SetTextAlign(TextAnchor.UpperCenter)
                        .SetColor(new Color(0.65f, 0.65f, 0.65f))
                        .SetWordWrap(true);
            }

            float titlePadding = 5;
            float descPadding = 8;
            StackPanelWidget host =
                new StackPanelWidget(StackPanelOrientation.Vertical)
                .AddWidget(thumbnailWidget, thumbnailHeight)
                .AddWidget(new BorderWidget(titleWidget)
                    .SetPadding(titlePadding, titlePadding, titlePadding, 0)
                    .SetBorderColor(new Color(0, 0, 0, 0))
                    .SetColor(new Color(0, 0, 0, 0))
                    , 0, true);

            if (showDescription)
            {
                host.AddWidget(new BorderWidget(descriptionWidget)
                    .SetPadding(descPadding, 0, descPadding, descPadding)
                    .SetBorderColor(new Color(0, 0, 0, 0))
                    .SetColor(new Color(0, 0, 0, 0)));
            }

            var hostLink = new LinkWidget(host);
            hostLink.LinkClicked += OnCardClicked;

            layout = new BorderWidget(hostLink)
                        .SetColor(new Color(0.1f, 0.1f, 0.1f))
                        .SetBorderColor(new Color(0, 0, 0, 1))
                        .SetPadding(0, 0, 0, 0);
        }

        public LaunchPadCardWidget SetThumbnailHeight(float height)
        {
            this.thumbnailHeight = height;
            return this;
        }

        private void OnCardClicked(WidgetClickEvent clickEvent)
        {
            if (data != null)
            {
                if (data.url != null && data.url.Length > 0)
                {
                    Application.OpenURL(data.url);
                }

                if (data.link != null && data.link.Length > 0)
                {
                    if (LinkClicked != null)
                    {
                        LinkClicked.Invoke(data.link);
                    }
                }
            }
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            if (layout == null)
            {
                BuildLayout(uiSystem);
            }
            var contentBounds = new Rect(Vector2.zero, WidgetBounds.size);
            layout.UpdateWidget(uiSystem, contentBounds);
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            if (layout != null)
            {
                WidgetUtils.DrawWidgetGroup(uiSystem, renderer, layout);
            }
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { layout };
        }

        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            if (layout == null) return Vector2.zero;
            return layout.GetDesiredSize(size, uiSystem);
        }
    }
}
