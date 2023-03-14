//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.UI
{
    public delegate void OnWidgetFocus(IWidget widget);
    public delegate void OnWidgetLostFocus(IWidget widget);
    public delegate void OnDragEvent();

    public abstract class UISystem
    {
        public IWidget FocusedWidget { get; private set; }

        public bool IsDragDrop { get; private set; }

        public IWidget Layout { get; private set; }

        public UIPlatform Platform { get; private set; }
        public UIStyleManager StyleManager { get; private set; }
        public UIUndoSystem Undo { get; private set; }
        
        public abstract bool SupportsDragDrop { get; }
        
        public UISystem()
        {
            Platform = CreatePlatformInstance();
            StyleManager = CreateStyleManagerInstance();
            Undo = CreateUndoSystemInstance();

            FocusedWidget = null;
            IsDragDrop = false;
            Layout = new NullWidget();
        }

        protected abstract UIPlatform CreatePlatformInstance();
        protected abstract UIStyleManager CreateStyleManagerInstance();
        protected abstract UIUndoSystem CreateUndoSystemInstance();

        public void Draw(UIRenderer uiRenderer)
        {
            if (Layout != null)
            {
                Layout.Draw(this, uiRenderer);
            }
        }

        public void Update(Rect bounds)
        {
            if (Layout != null)
            {
                Layout.UpdateWidget(this, bounds);
            }
        }

        public void SetLayout(IWidget layout)
        {
            this.Layout = layout;
        }

        public void ClearLayout()
        {
            Layout = new NullWidget();
        }
        
        public void RequestFocus(IWidget widget)
        {
            GUI.FocusControl("");
            // Notify that the old widget has lost focus
            if (FocusedWidget != null)
            {
                FocusedWidget.LostFocus();
                if (WidgetLostFocus != null)
                {
                    WidgetLostFocus.Invoke(FocusedWidget);
                }
            }

            FocusedWidget = widget;
            if (FocusedWidget != null)
            {
                FocusedWidget.OnFocus();
                if (WidgetFocused != null)
                {
                    WidgetFocused.Invoke(FocusedWidget);
                }
            }
        }

        public void SetDragging(bool dragging)
        {
            if (IsDragDrop == dragging) return;

            IsDragDrop = dragging;
            if (IsDragDrop)
            {
                if (DragStart != null)
                {
                    DragStart.Invoke();
                }
            }
            else
            {
                if (DragEnd != null)
                {
                    DragEnd.Invoke();
                }
            }
        }

        public event OnWidgetFocus WidgetFocused;
        public event OnWidgetLostFocus WidgetLostFocus;
        public event OnDragEvent DragStart;
        public event OnDragEvent DragEnd;
    }


    public enum UICursorType
    {
        Normal,
        ResizeHorizontal,
        ResizeVertical,
        Link,
    }
}
