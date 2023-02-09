//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI
{
    public interface UIPlatform
    {
        // Inspector
        void ShowObjectProperty(object obj);
        void ShowObjectProperties(object[] obj);

        // Asset management
        string GetAssetPath(object obj);
        void MarkAssetDirty(object obj);
        void AddObjectToAsset(Object objectToAdd, Object assetObject);

        // Menu
        IContextMenu CreateContextMenu();
        UIDragDrop DragDrop { get; }

        double timeSinceStartup { get; }
        string clipboardText { get; set; }
        Event CurrentEvent { get; }
    }

    public interface IContextMenu
    {
        void AddItem(string path, ContextMenuFunction func);
        void AddItem(string path, ContextMenuFunctionUserData func, object userData);
        void AddSeparator(string path);
        void Show();
    }

    public enum UIDragDropVisualMode
    {
        None,
        Copy,
        Generic,
        Move,
        Reject
    }
    
    public interface UIDragDrop
    {
        void PrepareStartDrag();
        void StartDrag(string title);
        void SetVisualMode(UIDragDropVisualMode visualMode);
        void AcceptDrag();
    }

    public delegate void ContextMenuFunction();
    public delegate void ContextMenuFunctionUserData(object userData);


    public class NullPlatform : UIPlatform
    {
        public void ShowObjectProperty(object obj)
        {
        }

        public void ShowObjectProperties(object[] obj)
        {
        }

        public string GetAssetPath(object obj)
        {
            return "";
        }

        public void MarkAssetDirty(object obj)
        {
        }

        public void AddObjectToAsset(Object objectToAdd, Object assetObject)
        {
        }

        public IContextMenu CreateContextMenu()
        {
            return null;
        }

        public UIDragDrop DragDrop { get { return null; } }

        public double timeSinceStartup { get { return 0; } }
        public string clipboardText
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public Event CurrentEvent { get => Event.current; }
    }

}
