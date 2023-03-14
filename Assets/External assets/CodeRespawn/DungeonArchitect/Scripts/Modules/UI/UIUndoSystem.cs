//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.UI
{
    public delegate void UIUndoRedoDelegate(UISystem uiSystem);
    public interface UIUndoSystem
    {
        void RecordObject(object obj, string name);
        void RegisterCreatedObjectUndo(object obj, string name);
        void DestroyObjectImmediate(object obj);
        void RegisterCompleteObjectUndo(object obj, string name);
        event UIUndoRedoDelegate UndoRedoPerformed;
    }
}
