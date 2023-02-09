//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Grid
{
    /// <summary>
    /// Editor tooling for the grid based dungeon builder. Lets you paint with a grid based brush
    /// </summary>
    [ExecuteInEditMode]
    public class DungeonPaintModeGrid : DungeonPaintMode
    {
        /// <summary>
        /// The height of the cursor in grid cooridnates. Can also be changed with the mouse wheel in the editor when activated
        /// </summary>
        public int cursorLogicalHeight = 0;

        /// <summary>
        /// The opacity of the overlay colored tiles
        /// </summary>
        public float overlayOpacity = 0.1f;

		/// <summary>
		/// Indicates if the painting is to be done in 2D mode (for 2D dungeons)
		/// This flag is used for the editor tooling.  The model still stores it in 3D
		/// </summary>
		public bool mode2D = false;

		/// <summary>
		/// The size of the brush.  This would create a brush of size NxN
		/// </summary>
		public int brushSize = 1;

        /// <summary>
        /// Reference to the grid model used by the grid builder
        /// </summary>
        private GridDungeonModel gridModel;

        public float GetCursorHeight()
        {
            var gridConfig = GetDungeonConfig() as GridDungeonConfig;
            if (gridConfig == null) return 0;
            return cursorLogicalHeight * gridConfig.GridCellSize.y;
        }

        public void SetElevationDelta(int delta)
        {
            cursorLogicalHeight += delta;
        }

        
        public GridDungeonModel GetDungeonModelGrid()
        {
            var model = base.GetDungeonModel();
            gridModel = model as GridDungeonModel;
            if (gridModel == null)
            {
                Debug.LogWarning("Invalid dungeon model type for this type of paint tool.  Expected DungeonModelGrid.  Received:" + (model != null ? model.GetType().ToString() : "null"));
            }

            return gridModel;
        }
    }
}