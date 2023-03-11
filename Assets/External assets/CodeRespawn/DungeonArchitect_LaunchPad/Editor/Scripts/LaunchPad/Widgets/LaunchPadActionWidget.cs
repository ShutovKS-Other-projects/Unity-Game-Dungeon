//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.Editors.LaunchPad.Actions;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad
{

    public class LaunchPadActionWidget : WidgetBase
    {
        protected IWidget Content;
        public LaunchPadActionType actionType = LaunchPadActionType.None;
        ILaunchPadAction action;


        public LaunchPadActionWidget(LaunchPadActionType actionType, ILaunchPadAction action, LaunchPadActionData data)
        {
            this.action = action;
            this.actionType = actionType;

            var title = action.GetText();
            if (data.title != null && data.title.Length > 0)
            {
                title = data.title;
            }

            if (action != null)
            {
                var host =
                    new BorderWidget(
                        new StackPanelWidget(StackPanelOrientation.Vertical)
                        .AddWidget(
                            new ImageWidget(action.GetIcon())
                            .SetDrawMode(ImageWidgetDrawMode.Fixed)
                        , 30, true)
                        .AddWidget(
                            new BorderWidget(
                                new LabelWidget(title)
                                .SetTextAlign(TextAnchor.MiddleCenter)
                                .SetColor(new Color(0.8f, 0.8f, 0.8f)))
                            .SetPadding(5, 5, 5, 5)
                            .SetTransparent()
                        , 0, true))
                    .SetPadding(2, 8, 2, 0)
                    .SetColor(new Color(0.1f, 0.1f, 0.1f));

                var link = new LinkWidget(host);
                link.LinkClicked += OnLinkClicked; ;

                Content = link;
            }
            else
            {
                this.Content = new NullWidget();
            }
        }

        private void OnLinkClicked(WidgetClickEvent clickEvent)
        {
            if (action != null)
            {
                action.Execute();
            }
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
