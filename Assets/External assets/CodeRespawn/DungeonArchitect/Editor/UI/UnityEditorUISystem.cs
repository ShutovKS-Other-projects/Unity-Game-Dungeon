//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.UI.Impl.UnityEditor
{
    public class UnityEditorUISystem : UISystem
    {
        protected override UIPlatform CreatePlatformInstance()
        {
            return new UnityEditorUIPlatform();
        }

        protected override UIStyleManager CreateStyleManagerInstance()
        {
            return new UnityEditorUIStyleManager();
        }

        protected override UIUndoSystem CreateUndoSystemInstance()
        {
            return new UnityEditorUIUndoSystem(this);
        }

        public override bool SupportsDragDrop
        {
            get
            {
                bool supported = true;
                
                #if UNITY_EDITOR_OSX
                // Disable drag/drop on mac as it is causing issues while rendering
                supported = false;
                #endif

                return supported;
            }
        }
    }
}