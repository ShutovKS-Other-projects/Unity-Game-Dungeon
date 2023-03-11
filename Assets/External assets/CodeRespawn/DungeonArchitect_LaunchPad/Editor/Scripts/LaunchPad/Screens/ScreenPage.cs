//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.UI.Widgets;

namespace DungeonArchitect.Editors.LaunchPad
{
    public class ScreenPageJsonDataBase
    {
        public string title;
        public string layout;
    }
    public delegate void OnScreenPageLinkClicked(string path);

    public enum ScreenPageType
    {
        CardGrid,
        Details,
        News,
        Empty
    }

    public abstract class ScreenPage
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public IWidget Widget { get; protected set; }
        public abstract void Load(string json);
        public event OnScreenPageLinkClicked LinkClicked;

        protected void NotifyLinkClicked(string path)
        {
            if (LinkClicked != null)
            {
                LinkClicked.Invoke(path);
            }
        }

    }

    public class ScreenPageFactory
    {
        public static ScreenPage Create(ScreenPageType pageType)
        {
            if (pageType == ScreenPageType.CardGrid)
            {
                return new ScreenPageCardGrid();
            }
            else if (pageType == ScreenPageType.Details)
            {
                return new ScreenPageDetails();
            }
            else if (pageType == ScreenPageType.News)
            {
                return new ScreenPageNews();
            }
            return null;
        }
    }
}