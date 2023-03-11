//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad
{
    [System.Serializable]
    class ScreenPageNewsJsonData : ScreenPageJsonDataBase
    {
        public DataCardEntry featured = new DataCardEntry();
        public DataCardEntry[] cards = new DataCardEntry[0];
        public float featuredHeight = 0;
        public float thumbHeight = 0;
        public float cardHeight = 0;
    }

    public class ScreenPageNews : ScreenPage
    {
        IWidget BuildFeaturedWidget(ScreenPageNewsJsonData jsonData)
        {
            if (jsonData.featured == null) return new NullWidget();

            float featuredHeight = 300;
            if (jsonData.featuredHeight > 0)
            {
                featuredHeight = jsonData.featuredHeight;
            }

            var card = BuildCardWidget(jsonData.featured, 200);
            card.LinkClicked += Card_LinkClicked;

            var cards = new GridPanelWidget(GridPanelArrangementType.VerticalScroll)
                .SetPadding(10, 10)
                .SetCellSize(560, featuredHeight);

            cards.AddWidget(card);
            return cards;
        }

        LaunchPadCardWidget BuildCardWidget(DataCardEntry entry, float height)
        {
            var data = new LaunchPadCardWidgetData();
            data.thumbnail = ScreenPageLoader.LoadImageAsset(entry.image);
            data.title = entry.title;
            data.description = entry.desc;
            data.link = entry.link;
            data.url = entry.url;
            return
                new LaunchPadCardWidget(data, true)
                .SetThumbnailHeight(height);
        }

        IWidget BuildCardsWidget(ScreenPageNewsJsonData jsonData)
        {
            float cardHeight = 220;
            if (jsonData.cardHeight > 0)
            {
                cardHeight = jsonData.cardHeight;
            }

            float thumbHeight = 100;
            if (jsonData.thumbHeight > 0)
            {
                thumbHeight = jsonData.thumbHeight;
            }

            var cards = new GridPanelWidget(GridPanelArrangementType.VerticalScroll)
            .SetPadding(10, 10)
            .SetCellSize(180, cardHeight);

            foreach (var entry in jsonData.cards)
            {
                var card = BuildCardWidget(entry, thumbHeight);
                card.LinkClicked += Card_LinkClicked;
                cards.AddWidget(card);
            }
            return cards;
        }
        public override void Load(string json)
        {
            var jsonData = JsonUtility.FromJson<ScreenPageNewsJsonData>(json);
            Title = jsonData.title;


            var featured = BuildFeaturedWidget(jsonData);
            var cards = BuildCardsWidget(jsonData);

            StackPanelWidget content = new StackPanelWidget(StackPanelOrientation.Vertical);
            content.AddWidget(featured, 0, true);
            content.AddWidget(cards, 0, true);

            Widget = content;
        }

        private void Card_LinkClicked(string path)
        {
            NotifyLinkClicked(path);
        }
    }
}
