//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;


namespace DungeonArchitect.Editors
{
    [CustomEditor(typeof(DebugText3D)), CanEditMultipleObjects]
    public class DebugText3DEditor : Editor {
        protected virtual void OnSceneGUI()
        {
            DebugText3D debugText3D = (DebugText3D)target;
            foreach (var item in debugText3D.items)
            {
                Handles.color = item.color;
                Handles.Label(item.position, item.message);
            }
        }
    }
}