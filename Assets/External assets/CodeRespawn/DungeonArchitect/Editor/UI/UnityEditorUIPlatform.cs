//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.UI.Impl.UnityEditor
{
    public class UnityEditorUIPlatform : UIPlatform
    {
        public void ShowObjectProperty(object obj)
        {
            Selection.activeObject = obj as Object;
        }

        public void ShowObjectProperties(object[] objs)
        {
            var unityObjs = new List<Object>();
            foreach (var obj in objs)
            {
                var unityObj = obj as Object;
                if (unityObj != null)
                {
                    unityObjs.Add(unityObj);
                }
            }
            Selection.objects = unityObjs.ToArray();
        }

        public string GetAssetPath(object obj)
        {
            var unityObj = obj as Object;
            return unityObj != null ? AssetDatabase.GetAssetPath(unityObj) : "";
        }

        public void MarkAssetDirty(object obj)
        {
            var unityObj = obj as Object;
            if (unityObj != null)
            {
                EditorUtility.SetDirty(unityObj);
            }
        }

        public IContextMenu CreateContextMenu()
        {
            return new UnityEditorContextMenu();
        }

        public void AddObjectToAsset(Object objectToAdd, Object assetObject)
        {
            AssetDatabase.AddObjectToAsset(objectToAdd, assetObject);
        }

        public double timeSinceStartup
        {
            get
            {
                return EditorApplication.timeSinceStartup;
            }
        }

        public string clipboardText { get; set; }
        public Event CurrentEvent { get => Event.current; }

        UIDragDrop dragDropImpl = new UnityEditorUIDragDrop();
        public UIDragDrop DragDrop
        {
            get
            {
                return dragDropImpl;
            }
        }
    }


    public class UnityEditorContextMenu : IContextMenu
    {
        GenericMenu menu;
        public UnityEditorContextMenu()
        {
            menu = new GenericMenu();
        }

        public void AddItem(string path, ContextMenuFunction func)
        {
            menu.AddItem(new GUIContent(path), false, () => func());
        }

        public void AddItem(string path, ContextMenuFunctionUserData func, object userData)
        {
            menu.AddItem(new GUIContent(path), false, (data) => func(data), userData);
        }
        public void AddSeparator(string path)
        {
            menu.AddSeparator(path);
        }

        public void Show()
        {
            menu.ShowAsContext();
        }
    }

    public class UnityEditorUIDragDrop : UIDragDrop
    {
        public void PrepareStartDrag()
        {
            DragAndDrop.PrepareStartDrag();
        }

        public void StartDrag(string title)
        {
            DragAndDrop.StartDrag(title);
        }

        public void SetVisualMode(UIDragDropVisualMode visualMode)
        {
            DragAndDropVisualMode unityVisualMode;
            if (visualMode == UIDragDropVisualMode.Copy) unityVisualMode = DragAndDropVisualMode.Copy;
            else if (visualMode == UIDragDropVisualMode.Generic) unityVisualMode = DragAndDropVisualMode.Generic;
            else if (visualMode == UIDragDropVisualMode.Move) unityVisualMode = DragAndDropVisualMode.Move;
            else if (visualMode == UIDragDropVisualMode.None) unityVisualMode = DragAndDropVisualMode.None;
            else if (visualMode == UIDragDropVisualMode.Reject) unityVisualMode = DragAndDropVisualMode.Rejected;
            else unityVisualMode = DragAndDropVisualMode.Generic;

            DragAndDrop.visualMode = unityVisualMode;
        }

        public void AcceptDrag()
        {
            DragAndDrop.AcceptDrag();
        }
    }

}
