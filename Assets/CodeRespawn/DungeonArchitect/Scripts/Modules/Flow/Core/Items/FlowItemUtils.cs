//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Flow.Items
{
    public class FlowItemUtils
    {
        public static string GetFlowItemText(FlowItem item)
        {
            switch (item.type)
            {
                case FlowGraphItemType.Entrance:
                    return "S";

                case FlowGraphItemType.Exit:
                    return "G";

                case FlowGraphItemType.Enemy:
                    return "E";

                case FlowGraphItemType.Key:
                    return "K";

                case FlowGraphItemType.Lock:
                    return "L";

                case FlowGraphItemType.Bonus:
                    return "B";

                case FlowGraphItemType.Custom:
                    return item.customInfo.text;

                default:
                    return "";
            }
        }

        public static void GetFlowItemColor(FlowItem item, out Color colorBackground, out Color colorText)
        {
            switch (item.type)
            {
                case FlowGraphItemType.Entrance:
                    colorBackground = new Color(0, 0.3f, 0);
                    colorText = Color.white;
                    break;

                case FlowGraphItemType.Exit:
                    colorBackground = new Color(0, 0, 0.3f);
                    colorText = Color.white;
                    break;

                case FlowGraphItemType.Enemy:
                    colorBackground = new Color(0.6f, 0, 0);
                    colorText = Color.white;
                    break;

                case FlowGraphItemType.Key:
                    colorBackground = Color.yellow;
                    colorText = Color.black;
                    break;

                case FlowGraphItemType.Lock:
                    colorBackground = Color.blue;
                    colorText = Color.white;
                    break;

                case FlowGraphItemType.Bonus:
                    colorBackground = new Color(0, 0.5f, 1);
                    colorText = Color.white;
                    break;

                case FlowGraphItemType.Custom:
                    colorBackground = item.customInfo.backgroundColor;
                    colorText = item.customInfo.textColor;
                    break;

                default:
                    colorBackground = Color.white;
                    colorText = Color.black;
                    break;
            }

        }
    }
}