//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class BreadCrumbWidgetNode
    {
        public BreadCrumbWidgetNode(string displayText, object userdata)
        {
            this.displayText = displayText;
            this.userdata = userdata;
        }

        public string displayText;
        public object userdata;
    }

    public delegate void OnBreadCrumbLinkClicked(object userdata);
    public class BreadCrumbWidget : WidgetBase
    {
        IWidget content;
        int padding = 5;
        bool requestRebuild = false;

        List<BreadCrumbWidgetNode> items = new List<BreadCrumbWidgetNode>();
        public event OnBreadCrumbLinkClicked LinkClicked;
        public int FontSize { get; set; }
        public Color TextColor { get; set; }

        public BreadCrumbWidget()
        {
            content = new NullWidget();
        }

        public BreadCrumbWidget SetPadding(int padding)
        {
            this.padding = padding;
            requestRebuild = true;
            return this;
        }

        public object GetTopItemUserData()
        {
            if (items.Count == 0) return null;
            return items[items.Count - 1].userdata;
        }

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

        public void PushPage(string displayName, object userdata)
        {
            items.Add(new BreadCrumbWidgetNode(displayName, userdata));
            requestRebuild = true;
        }

        public void MoveToPage(BreadCrumbWidgetNode node)
        {
            while (items.Count > 0)
            {
                var lastIndex = items.Count - 1;
                var lastItem = items[lastIndex];
                if (lastItem == node)
                {
                    break;
                }
                items.RemoveAt(lastIndex);
            }
            requestRebuild = true;
        }
        
        public void Clear()
        {
            items.Clear();
            requestRebuild = true;
        }

        void Rebuild(UISystem uiSystem)
        {
            var stackPanel = new StackPanelWidget(StackPanelOrientation.Horizontal);
            var firstNode = true;
            foreach (var node in items)
            {
                if (!firstNode)
                {
                    var arrow = 
                        new BorderWidget(
                            new LabelWidget(">")
                                .SetFontSize(FontSize)
                                .SetColor(TextColor))
                            .SetPadding(padding, padding, padding, padding)
                            .SetTransparent();

                    stackPanel.AddWidget(arrow, 0, true);
                }
                firstNode = false;

                var crumb = new LinkWidget(
                    new BorderWidget(
                        new LabelWidget(node.displayText)
                            .SetFontSize(FontSize)
                            .SetColor(TextColor))
                        .SetPadding(padding, padding, padding, padding)
                        .SetTransparent());
                crumb.SetUserData(node);
                crumb.SetDrawLinkOutline(false);

                crumb.LinkClicked += OnCrumbItemClicked;

                stackPanel.AddWidget(crumb, 0, true);
            }

            content = stackPanel;

            requestRebuild = false;
        }

        private void OnCrumbItemClicked(WidgetClickEvent clickEvent)
        {
            var node = clickEvent.userdata as BreadCrumbWidgetNode;
            if (node != null)
            {
                MoveToPage(node);
                if (LinkClicked != null)
                {
                    LinkClicked.Invoke(node.userdata);
                }
            }
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            if (content == null || requestRebuild)
            {
                Rebuild(uiSystem);
            }
            var contentBounds = new Rect(Vector2.zero, WidgetBounds.size);
            content.UpdateWidget(uiSystem, contentBounds);
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            if (content != null)
            {
                WidgetUtils.DrawWidgetGroup(uiSystem, renderer, content);
            }
        }

    }
}
