//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad
{
    [System.Serializable]
    class DataCardEntry
    {
        public string title = "";
        public string desc = "";
        public string image = "";
        public string link = "";
        public string url = "";
    }

    [System.Serializable]
    class DataCategory
    {
        public string category = "";
        public DataCardEntry[] cards = new DataCardEntry[0];
    }

    [System.Serializable]
    class ScreenPageGridCardJsonData : ScreenPageJsonDataBase
    {
        public DataCategory[] categories = new DataCategory[0];
        public bool showCategories = false;
        public bool showDescription = true;
        public int heightOverride = 0;
        public string description = "";
    }

    public class ScreenPageCardGrid : ScreenPage
    {
        public override void Load(string json)
        {
            var jsonData = JsonUtility.FromJson<ScreenPageGridCardJsonData>(json);
            Title = jsonData.title;
            StackPanelWidget content = new StackPanelWidget(StackPanelOrientation.Vertical);

            foreach (var category in jsonData.categories)
            {
                if (category.cards == null) continue;

                var showDescription = jsonData.showDescription;
                var cellHeight = 220;

                if (jsonData.heightOverride > 0)
                {
                    cellHeight = jsonData.heightOverride;
                }

                var cards = new GridPanelWidget(GridPanelArrangementType.VerticalScroll)
                .SetPadding(10, 10)
                .SetCellSize(180, cellHeight);

                foreach (var entry in category.cards)
                {
                    var data = new LaunchPadCardWidgetData();
                    data.thumbnail = ScreenPageLoader.LoadImageAsset(entry.image);
                    data.title = entry.title;
                    data.description = entry.desc;
                    data.link = entry.link;
                    data.url = entry.url;
                    var card = new LaunchPadCardWidget(data, showDescription);
                    card.LinkClicked += Card_LinkClicked;
                    cards.AddWidget(card);
                }

                if (jsonData.showCategories)
                {
                    content.AddWidget(
                        new BorderWidget(
                            new LabelWidget(category.category)
                                .SetFontSize(18)
                                .SetColor(new Color(0.6f, 0.6f, 0.6f)))
                            .SetTransparent()
                            .SetPadding(10, 20, 5, 5)
                        , 0, true);
                }
                content.AddWidget(cards, 0, true);
            }

            var containsDescription = (jsonData.description != null && jsonData.description.Length > 0);
            if (!containsDescription)
            {
                Widget = content;
            }
            else
            {
                var descPaddingOut = 20;
                Widget = new StackPanelWidget(StackPanelOrientation.Vertical)
                    .AddWidget(
                        new BorderWidget(
                            new LabelWidget(jsonData.description)
                                .SetFontSize(16)
                                .SetColor(new Color(0.8f, 0.8f, 0.8f))
                                .SetWordWrap(true))
                            .SetPadding(descPaddingOut, 5, descPaddingOut, 5)
                            .SetTransparent()
                    , 0, true)
                    .AddWidget(content, 0, true);
            }
        }

        private void Card_LinkClicked(string path)
        {
            NotifyLinkClicked(path);
        }
    }
}
