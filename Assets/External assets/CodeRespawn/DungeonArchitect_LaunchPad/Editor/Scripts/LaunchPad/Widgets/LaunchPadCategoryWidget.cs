//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad
{
    public struct LaunchPadCategoryData
    {
        public LaunchPadCategoryData(string path, string displayText)
        {
            this.path = path;
            this.displayText = displayText;
        }

        public string path;
        public string displayText;
    }

    public class LaunchPadCategoryDataSource : ListViewSource<LaunchPadCategoryData>
    {
        LaunchPadCategoryData[] items;
        
        public int Count
        {
            get => (items != null) ? items.Length : 0;
        }
        
        public void SetItems(LaunchPadCategoryData[] items)
        {
            this.items = items;
        }

        public override LaunchPadCategoryData[] GetItems()
        {
            return items;
        }

        public override IWidget CreateWidget(LaunchPadCategoryData item)
        {
            var itemWidget = new LaunchPadCategoryItem(item);
            itemWidget.TextStyle.fontSize = 16;

            itemWidget.SelectedTextStyle = new GUIStyle(itemWidget.TextStyle);
            itemWidget.SelectedTextStyle.normal.textColor = Color.white;
            itemWidget.SelectedColor = new Color(0.2f, 0.2f, 0.2f);

            return itemWidget;
        }
    }

    public class LaunchPadCategoryItem : ListViewTextItemWidget
    {
        public LaunchPadCategoryItem(LaunchPadCategoryData category)
            : base(category, () => category.displayText)
        {
        }
    }
}
