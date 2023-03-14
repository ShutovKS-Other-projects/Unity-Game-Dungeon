//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public abstract class ListViewSource<T>
    {
        public abstract T[] GetItems();
        public abstract IWidget CreateWidget(T item);
    }

    public delegate void OnListViewItemSelected(object data);
    public interface IListViewItemWidget
    {
        event OnListViewItemSelected ItemSelected;
        event OnListViewItemSelected ItemDoubleClicked;
        bool Selected { get; set; }
        object ItemData { get; set; }
        string GetCaption();
    }

    public class ListViewWidget<T> : WidgetBase
    {
        ListViewSource<T> dataSource;
        T selectedItem;

        public delegate void OnSelectionChanged(T Item);
        public event OnSelectionChanged SelectionChanged;
        public event OnSelectionChanged ItemClicked;
        public event OnSelectionChanged ItemDoubleClicked;

        public ScrollPanelWidget ScrollView;
        public int ItemHeight = 40;
        public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f);

        StackPanelWidget panel;
        IWidget layout;

        public void Bind(ListViewSource<T> dataSource)
        {
            this.dataSource = dataSource;
            selectedItem = default(T);
            NotifyDataChanged();
        }

        bool IsEqual(T a, T b)
        {
            return EqualityComparer<T>.Default.Equals(a, b);
        }

        void BuildLayout()
        {
            panel = new StackPanelWidget(StackPanelOrientation.Vertical);

            if (dataSource != null)
            {
                var items = dataSource.GetItems();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var itemWidget = dataSource.CreateWidget(item);
                        if (itemWidget is IListViewItemWidget)
                        {
                            var listViewItemWidget = itemWidget as IListViewItemWidget;
                            listViewItemWidget.ItemSelected += ListViewItemWidget_ItemSelected;
                            listViewItemWidget.ItemDoubleClicked += ListViewItemWidget_ItemDoubleClicked;
                        }
                        panel.AddWidget(itemWidget, ItemHeight);
                    }
                }
            }

            ScrollView = new ScrollPanelWidget(panel);
            layout = ScrollView;
        }

        private void ListViewItemWidget_ItemDoubleClicked(object data)
        {
            if (ItemDoubleClicked != null)
            {
                ItemDoubleClicked.Invoke((T)data);
            }
        }

        private void ListViewItemWidget_ItemSelected(object data)
        {
            var items = dataSource.GetItems();
            int index = System.Array.IndexOf(items, data);
            SetSelectedIndex(index);

            if (ItemClicked != null)
            {
                ItemClicked.Invoke((T)data);
            }
        }

        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            return layout.GetDesiredSize(size, uiSystem);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { layout };
        }
        
        public void NotifyDataChanged()
        {
            BuildLayout();
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            var layoutBounds = new Rect(Vector2.zero, bounds.size);
            layout.UpdateWidget(uiSystem, layoutBounds);
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            var guiState = new GUIState(renderer);
            if (IsPaintEvent(uiSystem))
            {
                var bounds = new Rect(Vector2.zero, WidgetBounds.size);
                renderer.DrawRect(bounds, backgroundColor);
            }

            WidgetUtils.DrawWidgetGroup(uiSystem, renderer, layout);

            guiState.Restore(); 
        }

        public override bool CanAcquireFocus() { return false; }

        public int GetSelectedIndex()
        {
            if (IsNull(selectedItem))
            {
                return -1;
            }

            var items = dataSource.GetItems();
            return System.Array.IndexOf(items, selectedItem);
        }
        public T GetSelectedItem()
        {
            return selectedItem;
        }

        public T GetItem(int index)
        {
            var items = dataSource.GetItems();
            if (items == null || index < 0 || index >= items.Length)
            {
                return default(T);
            }
            return items[index];
        }

        public void SetSelectedItem(UISystem uiSystem, T item)
        {
            SetSelectedItem(uiSystem, item, false);
        }

        public void SetSelectedItem(UISystem uiSystem, T item, bool selectOnInspector)
        {
            var items = dataSource.GetItems();
            int index = System.Array.IndexOf(items, item);
            SetSelectedIndex(index);

            if (selectOnInspector)
            {
                uiSystem.Platform.ShowObjectProperty(item as Object);
            }
        }

        bool IsNull(T item)
        {
            return IsEqual(selectedItem, default(T));
        }

        public void SetSelectedIndex(int index)
        {
            var items = dataSource.GetItems();
            if (items == null || index < 0 || index >= items.Length)
            {
                // invalid hit index. deselect
                
                if (!IsNull(selectedItem) && System.Array.IndexOf(items, selectedItem) == -1)
                {
                    selectedItem = default(T);

                    if (SelectionChanged != null)
                    {
                        SelectionChanged.Invoke(selectedItem);
                    }
                }
                return;
            }

            T previousSelectedItem = selectedItem;
            selectedItem = items[index];
            if (!IsEqual(previousSelectedItem, selectedItem))
            {
                // Notify the selection change event
                if (SelectionChanged != null)
                {
                    SelectionChanged.Invoke(selectedItem);
                }
            }

            var children = panel != null ? panel.GetChildWidgets() : null;
            if (children != null)
            {
                foreach (var itemWidget in children)
                {
                    if (itemWidget is IListViewItemWidget)
                    {
                        var listItemWidget = itemWidget as IListViewItemWidget;
                        listItemWidget.Selected = false;
                        if (listItemWidget.ItemData != null && !IsNull(selectedItem))
                        {
                            listItemWidget.Selected = listItemWidget.ItemData.Equals(selectedItem);
                        }
                    }
                }
            }
        }
    }

    public class ListViewTextItemWidget : WidgetBase, IListViewItemWidget
    {
        public bool Selected { get; set; }
        public object ItemData { get; set; }

        public GUIStyle TextStyle = new GUIStyle();
        public GUIStyle SelectedTextStyle = new GUIStyle();
        public Color SelectedColor = new Color(1.0f, 0.5f, 0.0f);
        public float OffsetX = 10;
        System.Func<string> captionGetter;

        public event OnListViewItemSelected ItemSelected;
        public event OnListViewItemSelected ItemDoubleClicked;

        public ListViewTextItemWidget(object itemData, System.Func<string> captionGetter)
        {
            this.captionGetter = captionGetter;
            this.ItemData = itemData;
            TextStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            var guiState = new GUIState(renderer);
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            if (Selected)
            {
                renderer.DrawRect(bounds, SelectedColor);
            }

            DrawText(renderer, bounds);

            guiState.Restore();
        }

        public virtual void DrawText(UIRenderer renderer, Rect bounds)
        {
            var style = Selected ? SelectedTextStyle : TextStyle;
            float x = OffsetX;
            float y = (bounds.height - style.lineHeight) / 2.0f;
            var content = new GUIContent(GetCaption());
            var textSize = style.CalcSize(content);
            var textBounds = new Rect(new Vector2(x, y), textSize);
            renderer.Label(textBounds, content, style);
        }

        public string GetCaption()
        {
            return captionGetter != null ? captionGetter() : "-";
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            base.HandleInput(e, uiSystem);

            var localBounds = new Rect(Vector2.zero, WidgetBounds.size);
            if (localBounds.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown)
                {
                    if (e.clickCount == 1)
                    {
                        if (ItemSelected != null)
                        {
                            ItemSelected.Invoke(ItemData);
                        }
                    }
                    else if (e.clickCount == 2)
                    {
                        if (ItemDoubleClicked != null)
                        {
                            ItemDoubleClicked.Invoke(ItemData);
                        }
                    }
                }
            }
        }

        protected override void HandleDragStart(Event e, UISystem uiSystem)
        {
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            if (bounds.Contains(e.mousePosition))
            {
                base.HandleDragStart(e, uiSystem);
            }
        }

        public override bool CanAcquireFocus()
        {
            return true;
        }
    }
}
