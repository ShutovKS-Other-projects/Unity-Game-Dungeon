//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using DungeonArchitect.Grammar;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace DungeonArchitect.Editors.SnapFlow
{
    class RuleListViewConstants
    {
        public static readonly string DragDropID = "RuleDragOp";
        public static readonly Color ThemeColor = new Color(0.3f, 0.3f, 0.2f);
    }

    public class RuleListViewItem : ListViewTextItemWidget
    {
        HighlightWidget highlight;

        public RuleListViewItem(GrammarProductionRule rule)
            : base(rule, () => rule.ruleName)
        {
            DragDropEnabled = true;
            DragStart += RuleListViewItem_DragStart;

            highlight = new HighlightWidget()
                .SetContent(new NullWidget())
                .SetObjectOfInterest(rule);
        }

        private void RuleListViewItem_DragStart(Event e, UISystem uiSystem)
        {
            DragAndDrop.SetGenericData(RuleListViewConstants.DragDropID, ItemData);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            var children = new List<IWidget>();
            {
                IWidget[] baseChildren = base.GetChildWidgets();
                if (baseChildren != null)
                {
                    children.AddRange(baseChildren);
                }
            }
            children.Add(highlight);
            return children.ToArray();
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            base.DrawImpl(uiSystem, renderer);

            if (IsPaintEvent(uiSystem))
            {
                highlight.Draw(uiSystem, renderer);
            }
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            highlight.UpdateWidget(uiSystem, bounds);
        }
    }

    public class RuleListViewSource : ListViewSource<GrammarProductionRule>
    {
        SnapFlowAsset flowAsset;
        public RuleListViewSource(SnapFlowAsset flowAsset)
        {
            this.flowAsset = flowAsset;
        }
        public override GrammarProductionRule[] GetItems()
        {
            return flowAsset != null ? flowAsset.productionRules : null;
        }

        public override IWidget CreateWidget(GrammarProductionRule item)
        {
            var itemWidget = new RuleListViewItem(item);
            itemWidget.TextStyle.fontSize = 16;

            itemWidget.SelectedTextStyle = new GUIStyle(itemWidget.TextStyle);
            itemWidget.SelectedTextStyle.normal.textColor = Color.black;
            itemWidget.SelectedColor = RuleListViewConstants.ThemeColor * 2;

            return itemWidget;
        }
    }

    public class RuleListPanel : WidgetBase
    {
        SnapFlowAsset flowAsset;

        IWidget host;

        public ListViewWidget<GrammarProductionRule> ListView;
        ToolbarWidget toolbar;


        readonly static string BTN_ADD_ITEM = "AddItem";
        readonly static string BTN_REMOVE_ITEM = "RemoveItem";
        readonly static string BTN_MOVE_UP = "MoveUp";
        readonly static string BTN_MOVE_DOWN = "MoveDown";

        public RuleListPanel(SnapFlowAsset flowAsset)
        {
            this.flowAsset = flowAsset;

            toolbar = new ToolbarWidget();
            toolbar.ButtonSize = 24;
            toolbar.Padding = 4;
            toolbar.Background = new Color(0, 0, 0, 0);
            toolbar.AddButton(BTN_ADD_ITEM, UIResourceLookup.ICON_PLUS_16x);
            toolbar.AddButton(BTN_REMOVE_ITEM, UIResourceLookup.ICON_CLOSE_16x);
            toolbar.AddButton(BTN_MOVE_UP, UIResourceLookup.ICON_MOVEUP_16x);
            toolbar.AddButton(BTN_MOVE_DOWN, UIResourceLookup.ICON_MOVEDOWN_16x);
            toolbar.ButtonPressed += Toolbar_ButtonPressed;
            var toolbarSize = new Vector2(toolbar.Padding * 2 + toolbar.ButtonSize * 4, toolbar.Padding * 2 + toolbar.ButtonSize);

            ListView = new ListViewWidget<GrammarProductionRule>();
            ListView.ItemHeight = 45;
            ListView.Bind(new RuleListViewSource(flowAsset));

            IWidget toolWidget = new StackPanelWidget(StackPanelOrientation.Horizontal)
                                .AddWidget(new NullWidget())
                                .AddWidget(toolbar, toolbarSize.x);

            toolWidget = new BorderWidget(toolWidget)
                .SetPadding(0, 0, 0, 0)
                .SetDrawOutline(false)
                .SetColor(new Color(0, 0, 0, 0.25f));

            host = new BorderWidget()
                   .SetTitle("Rule List")
                   .SetColor(RuleListViewConstants.ThemeColor)
                   .SetContent(
                        new StackPanelWidget(StackPanelOrientation.Vertical)
                        .AddWidget(toolWidget, toolbarSize.y)
                        .AddWidget(new HighlightWidget()
                            .SetContent(ListView)
                            .SetObjectOfInterest(DungeonFlowEditorHighlightID.RulePanel)
                        )
                    );
        }


        private void Toolbar_ButtonPressed(UISystem uiSystem, string id)
        {
            if (flowAsset == null)
            {
                return;
            }

            if (id == BTN_ADD_ITEM)
            {
                var rule = SnapEditorUtils.AddProductionRule(flowAsset, "Rule");
                int index = System.Array.FindIndex(flowAsset.productionRules, r => r == rule);
                ListView.NotifyDataChanged();
                ListView.SetSelectedIndex(index);
            }
            else if (id == BTN_REMOVE_ITEM)
            {
                var rule = ListView.GetSelectedItem();
                if (rule != null)
                {
                    string message = string.Format("Are you sure you want to delete the rule \'{0}\'?", rule.ruleName);
                    bool removeItem = EditorUtility.DisplayDialog("Delete Rule?", message, "Delete", "Cancel");
                    if (removeItem)
                    {
                        int index = System.Array.FindIndex(flowAsset.productionRules, r => r == rule);
                        SnapEditorUtils.RemoveProductionRule(flowAsset, rule);
                        ListView.NotifyDataChanged();

                        if (index >= flowAsset.productionRules.Length)
                        {
                            index = flowAsset.productionRules.Length - 1;
                        }
                        ListView.SetSelectedIndex(index);
                    }
                }
            }
            else if (id == BTN_MOVE_UP)
            {
                var nodeType = ListView.GetSelectedItem();
                var list = new List<GrammarProductionRule>(flowAsset.productionRules);
                int index = list.IndexOf(nodeType);
                if (index > 0)
                {
                    list.RemoveAt(index);
                    index--;
                    list.Insert(index, nodeType);
                    flowAsset.productionRules = list.ToArray();

                    ListView.NotifyDataChanged();
                    ListView.SetSelectedIndex(index);
                }
            }
            else if (id == BTN_MOVE_DOWN)
            {
                var rule = ListView.GetSelectedItem();
                var list = new List<GrammarProductionRule>(flowAsset.productionRules);
                int index = list.IndexOf(rule);
                if (index + 1 < list.Count)
                {
                    list.RemoveAt(index);
                    index++;
                    list.Insert(index, rule);
                    flowAsset.productionRules = list.ToArray();

                    ListView.NotifyDataChanged();
                    ListView.SetSelectedIndex(index);
                }
            }
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

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            host.Draw(uiSystem, renderer);
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
