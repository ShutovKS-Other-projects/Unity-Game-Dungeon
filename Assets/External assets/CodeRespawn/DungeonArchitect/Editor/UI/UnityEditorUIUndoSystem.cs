//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace DungeonArchitect.UI.Impl.UnityEditor
{
    public class UnityEditorUIUndoSystem : UIUndoSystem
    {
        UISystem uiSystem;
        event UIUndoRedoDelegate undoRedoPerformed;
        public event UIUndoRedoDelegate UndoRedoPerformed
        {
            add
            {
                var references = 0;
                if (undoRedoPerformed == null || !undoRedoPerformed.GetInvocationList().Contains(undoRedoPerformed))
                {
                    undoRedoPerformed += value;
                    references = undoRedoPerformed.GetInvocationList().Length;
                }

                if (references > 0)
                {
                    CreateHook();
                }
            }
            remove
            {
                undoRedoPerformed -= value;

                var references = 0;
                if (undoRedoPerformed != null)
                {
                    references = undoRedoPerformed.GetInvocationList().Length;
                }
                if (references == 0)
                {
                    RemoveHook();
                }
            }
        }

        public UnityEditorUIUndoSystem(UISystem uiSystem)
        {
            this.uiSystem = uiSystem;
        }

        public void CreateHook()
        {
            Undo.undoRedoPerformed += __InternalUndoRedoCallback;
        }

        public void RemoveHook()
        {
            Undo.undoRedoPerformed -= __InternalUndoRedoCallback;
        }

        
        public void __InternalUndoRedoCallback()
        {
            if (undoRedoPerformed != null)
            {
                undoRedoPerformed.Invoke(uiSystem);
            }
        }

        public void RecordObject(object obj, string name)
        {
            var unityObj = obj as Object;
            if (unityObj != null)
            {
                Undo.RecordObject(unityObj, name);
            }
        }
        public void RegisterCreatedObjectUndo(object obj, string name)
        {
            var unityObj = obj as Object;
            if (unityObj != null)
            {
                Undo.RegisterCreatedObjectUndo(unityObj, name);
            }
        }
        public void DestroyObjectImmediate(object obj)
        {
            var unityObj = obj as Object;
            if (unityObj != null)
            {
                Undo.DestroyObjectImmediate(unityObj);
            }
        }

        public void RegisterCompleteObjectUndo(object obj, string name)
        {
            var unityObj = obj as Object;
            if (unityObj != null)
            {
                Undo.RegisterCompleteObjectUndo(unityObj, name);
            }
        }
    }
}