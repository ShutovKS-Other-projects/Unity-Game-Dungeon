//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.RoomDesigner.Editors
{
    [CustomEditor(typeof(DungeonRoomDesigner))]
    public class DungeonRoomDesignerEditor : Editor
    {
        private DungeonRoomDesignerTools activeTool = null;
        private DungeonRoomDesignerToolbar toolbar = new DungeonRoomDesignerToolbar();

        protected virtual void OnSceneGUI()
        {
            var room = target as DungeonRoomDesigner;
            if (toolbar.Draw(room) || activeTool == null)
            {
                CreateToolInstance();
            }

            // Draw the active tool
            if (activeTool != null)
            {
                activeTool.DrawSceneGUI(room);
            }
        }


        void CreateToolInstance()
        {
            activeTool = null;
            if (toolbar.MainToolIndex == 0 && toolbar.SubToolIndex == 0)
            {
                activeTool = new DungeonRoomDesignerTool_RoomMove();
            }
            else if (toolbar.MainToolIndex == 0 && toolbar.SubToolIndex == 1)
            {
                activeTool = new DungeonRoomDesignerTool_RoomBounds();
            }

            else if (toolbar.MainToolIndex == 1 && toolbar.SubToolIndex == 0)
            {
                activeTool = new DungeonRoomDesignerTool_DoorMove();
            }
            else if (toolbar.MainToolIndex == 1 && toolbar.SubToolIndex == 1)
            {
                activeTool = new DungeonRoomDesignerTool_DoorBounds();
            }
        }

    }
}
