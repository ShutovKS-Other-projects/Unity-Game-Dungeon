//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect.Editors.LaunchPad.Actions;
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.LaunchPad
{
    [System.Serializable]
    class ScreenPageActionJsonData
    {
        public string type = "";
        public LaunchPadActionData data = new LaunchPadActionData();
    }


    [System.Serializable]
    class ScreenPageDetailJsonData : ScreenPageJsonDataBase
    {
        public string header = "";
        public string desc = "";
        public string image = "";
        public ScreenPageActionJsonData[] actions = new ScreenPageActionJsonData[0];
    }

    public class ScreenPageDetails : ScreenPage
    {
        public override void Load(string json)
        {
            var jsonData = JsonUtility.FromJson<ScreenPageDetailJsonData>(json);
            Title = jsonData.title;
            var image = ScreenPageLoader.LoadImageAsset(jsonData.image);
            if (image == null)
            {
                image = Texture2D.whiteTexture;
            }

            StackPanelWidget content = new StackPanelWidget(StackPanelOrientation.Vertical);
            content.AddWidget(
                new ImageWidget(image)
                .SetDrawMode(ImageWidgetDrawMode.Fit)
            , 0, true);

            content.AddWidget(
                new BorderWidget(
                    new LabelWidget(jsonData.header)
                        .SetFontSize(18)
                        .SetWordWrap(true)
                        .SetColor(new Color(0.85f, 0.85f, 0.85f))
                        .SetTextAlign(TextAnchor.MiddleCenter))
                    .SetTransparent()
                    .SetPadding(5, 5, 5, 5)
                , 0, true);

            content.AddWidget(
                new BorderWidget(
                    new LabelWidget(jsonData.desc)
                        .SetFontSize(14)
                        .SetWordWrap(true)
                        .SetColor(new Color(0.6f, 0.6f, 0.6f)))
                    .SetTransparent()
                    .SetPadding(10, 0, 10, 10)
                , 0, true);

            if (jsonData.actions.Length > 0)
            {
                var actions = new GridPanelWidget(GridPanelArrangementType.VerticalScroll);
                actions.SetAutoSize(true);
                foreach (var actionInfo in jsonData.actions)
                {
                    
                    try
                    {
                        LaunchPadActionType actionType = (LaunchPadActionType)System.Enum.Parse(typeof(LaunchPadActionType), actionInfo.type);
                        var action = LaunchPadActionFactory.Create(actionType, actionInfo.data);
                        if (action != null && action.IsValid())
                        {
                            var actionWidget = new LaunchPadActionWidget(actionType, action, actionInfo.data);
                            actions.AddWidget(actionWidget);
                        }
                    }
                    catch {}
                }

                content.AddWidget(actions, 0, true);
            }

            Widget = content;
        }

        private void OnActionLinkClicked(WidgetClickEvent clickEvent)
        {
            Debug.Log("Clicked");
        }
    }
}
