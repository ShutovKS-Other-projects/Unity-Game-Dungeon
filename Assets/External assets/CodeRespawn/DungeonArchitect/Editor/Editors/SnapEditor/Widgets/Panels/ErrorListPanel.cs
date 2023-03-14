//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.SnapFlow
{
    public enum DungeonFlowErrorType
    {
        Error,
        Warning,
        Info,
        Success
    }


    public class DungeonFlowErrorEntry
    {
        public string Message = "";
        public DungeonFlowErrorType ErrorType = DungeonFlowErrorType.Info;
        public ISnapEdValidatorAction Action;

        public DungeonFlowErrorEntry() { }
        public DungeonFlowErrorEntry(string message)
        {
            this.Message = message;
        }

        public DungeonFlowErrorEntry(string message, DungeonFlowErrorType errorType)
        {
            this.Message = message;
            this.ErrorType = errorType;
        }

        public DungeonFlowErrorEntry(string message, DungeonFlowErrorType errorType, ISnapEdValidatorAction action)
        {
            this.Message = message;
            this.ErrorType = errorType;
            this.Action = action;
        }
    }

    public class DungeonFlowErrorList
    {
        public List<DungeonFlowErrorEntry> Errors = new List<DungeonFlowErrorEntry>();
    }

    public class ErrorListViewSource : ListViewSource<DungeonFlowErrorEntry>
    {
        DungeonFlowErrorList errorList;
        public ErrorListViewSource(DungeonFlowErrorList errorList)
        {
            this.errorList = errorList;
        }

        public override DungeonFlowErrorEntry[] GetItems()
        {
            return (errorList != null && errorList.Errors != null) 
                ? errorList.Errors.ToArray() : null;
        }

        public override IWidget CreateWidget(DungeonFlowErrorEntry item)
        {
            var itemWidget = new ErrorListViewItem(item);

            return itemWidget;
        }
    }

    public class ErrorListViewItem : ListViewTextItemWidget
    {
        public ErrorListViewItem(DungeonFlowErrorEntry entry)
            : base(entry, () => entry.Message)
        {
            OffsetX = 2;

            int fontSize = 14;

            TextStyle.fontSize = fontSize;
            SelectedTextStyle.normal.textColor = Color.blue;

            SelectedTextStyle.fontSize = fontSize;
            SelectedTextStyle.normal.textColor = Color.black;

            SelectedColor = ErrorListPanel.ThemeColor * 2.0f;
        }

        Texture GetTexture(UIRenderer renderer)
        {
            var entry = ItemData as DungeonFlowErrorEntry;
            if (entry != null)
            {
                switch(entry.ErrorType)
                {
                    case DungeonFlowErrorType.Error:
                        return renderer.GetResource<Texture>(UIResourceLookup.ICON_ERROR_16x) as Texture;

                    case DungeonFlowErrorType.Warning:
                        return renderer.GetResource<Texture>(UIResourceLookup.ICON_WARNING_16x) as Texture;

                    case DungeonFlowErrorType.Info:
                        return renderer.GetResource<Texture>(UIResourceLookup.ICON_INFO_16x) as Texture;

                    case DungeonFlowErrorType.Success:
                        return renderer.GetResource<Texture>(UIResourceLookup.ICON_SUCCESS_16x) as Texture;
                }
            }
            return renderer.GetResource<Texture>(UIResourceLookup.ICON_INFO_16x) as Texture;
        }

        public override void DrawText(UIRenderer renderer, Rect bounds)
        {
            var iconOffset = new Vector2(1, 1) * (bounds.height - 16) * 0.5f;
            var iconBounds = new Rect(iconOffset, new Vector2(16, 16));
            renderer.DrawTexture(iconBounds, GetTexture(renderer));

            var style = Selected ? SelectedTextStyle : TextStyle;
            float x = OffsetX + bounds.height;
            float y = (bounds.height - style.lineHeight) / 2.0f - 1;
            string message = GetCaption();
            var content = new GUIContent(message, message);
            var textSize = style.CalcSize(content);
            var textBounds = new Rect(new Vector2(x, y), textSize);
            renderer.Label(textBounds, content, style);
        }
    }

    public class ErrorListPanel : WidgetBase
    {
        IWidget host;
        public DungeonFlowErrorList errorList { get; private set; }

        public static readonly Color ThemeColor = new Color(0.3f, 0.2f, 0.2f);
        public ListViewWidget<DungeonFlowErrorEntry> ListView;

        public ErrorListPanel(DungeonFlowErrorList errorList)
        {
            this.errorList = errorList;

            ListView = new ListViewWidget<DungeonFlowErrorEntry>();
            ListView.Bind(new ErrorListViewSource(errorList));
            ListView.ItemHeight = 20;

            host = new BorderWidget()
                   .SetTitle("Error List")
                   .SetColor(ThemeColor)
                   .SetContent(ListView)
                    ;

        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            host.Draw(uiSystem, renderer);
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            if (host != null)
            {
                var childBounds = new Rect(Vector2.zero, bounds.size);
                host.UpdateWidget(uiSystem, childBounds);
            }
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            host.HandleInput(e, uiSystem);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { host };
        }
    }
}