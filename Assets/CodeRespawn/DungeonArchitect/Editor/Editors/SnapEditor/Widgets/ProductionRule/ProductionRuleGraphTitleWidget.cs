//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class ProductionRuleRHSTitleWidget : WidgetBase
    {
        public delegate void OnDeletePressed(ProductionRuleWidgetRHSState state);
        public event OnDeletePressed DeletePressed;

        public ProductionRuleWidgetRHSState State;

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            Vector2 titleOffset = new Vector2(5, 4);

            var titleStyle = new GUIStyle(EditorStyles.whiteLabel);
            titleStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);

            var titleBounds = WidgetBounds;
            titleBounds.position += titleOffset;
            titleBounds.size -= titleOffset * 2;

            renderer.BeginGroup(titleBounds);

            float PX_BUTTON_SIZE = 18;
            float PX_BUTTON_PADDING = 10;
            float PX_PADDING = 5;
            float PX_WEIGHT_INPUT = 40;
            float PX_HEIGHT = 16;

            float x = titleBounds.width;
            x -= PX_BUTTON_SIZE;

            var guiState = new GUIState(renderer);
            renderer.backgroundColor = new Color(0.8f, 0.1f, 0.1f, 1.0f);
            bool deletePressed = false;
            if (renderer.Button(new Rect(x, 0, PX_BUTTON_SIZE, PX_BUTTON_SIZE), "X"))
            {
                deletePressed = true;
            }
            guiState.Restore();

            var WeightGraph = State.WeightedGraph;

            x -= PX_BUTTON_PADDING;
            x -= PX_WEIGHT_INPUT;

            string Weight = (WeightGraph != null) ? WeightGraph.weight.ToString() : "";
            guiState.Save();
            renderer.backgroundColor = new Color(0.6f, 0.6f, 0.6f);
            if (renderer.Button(new Rect(x, 1, PX_WEIGHT_INPUT, PX_HEIGHT), Weight, EditorStyles.miniButton))
            {
                Selection.activeObject = WeightGraph;
            }
            guiState.Restore();

            x -= PX_PADDING;

            //if (IsPaintEvent(uiSystem))
            {
                var weightCaption = new GUIContent("Weight:");
                float PX_WEIGHT_LABEL = titleStyle.CalcSize(weightCaption).x;

                x -= PX_WEIGHT_LABEL;
                renderer.Label(new Rect(x, 1, PX_WEIGHT_LABEL, PX_HEIGHT), weightCaption, titleStyle);

                float remainingWidth = x;
                if (x > 0)
                {
                    renderer.Label(new Rect(0, 0, remainingWidth, PX_HEIGHT), "RHS Graph", titleStyle);
                }
            }

            renderer.EndGroup();

            if (deletePressed)
            {
                if (DeletePressed != null)
                {
                    DeletePressed.Invoke(State);
                }
            }
        }
    }

    public class ProductionRuleLHSTitleWidget : WidgetBase
    {
        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            if (IsPaintEvent(uiSystem))
            {
                Vector2 titleOffset = new Vector2(5, 5);

                var titleStyle = new GUIStyle(EditorStyles.whiteLabel);
                titleStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);

                var titleBounds = WidgetBounds;
                titleBounds.position += titleOffset;

                renderer.Label(titleBounds, "LHS Graph", titleStyle);
            }
        }

    }

}
