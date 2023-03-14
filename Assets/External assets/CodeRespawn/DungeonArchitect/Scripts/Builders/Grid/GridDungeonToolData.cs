//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Builders.Grid
{
    public class GridDungeonToolData : DungeonToolData
    {
        // The cells painted by the "Paint" tool
        [SerializeField]
        [HideInInspector]
        public List<IntVector> paintedCells = new List<IntVector>();
        
        

        /// <summary>
        /// Registers a painted cell. Returns true if state was modified
        /// </summary>
        /// <param name="location">the location of the painted cell, in grid cooridnates</param>
        /// <param name="automaticRebuild">if true, the dungeon would be rebuilt, if the data model has changed due to this request</param>
		public bool AddPaintCell(IntVector location) {
			bool overlappingCell = false;
			IntVector overlappingCellValue = new IntVector();
			foreach (var cellData in paintedCells) {
				if (cellData.x == location.x && cellData.z == location.z) {
					if (cellData.y != location.y) {
						overlappingCell = true;
						overlappingCellValue = cellData;
						break;
					}
					else {
						// Cell with this data already exists.  Ignore the request
						return false;
					}
				}
			}
			if (overlappingCell) {
				paintedCells.Remove(overlappingCellValue);
			}

			paintedCells.Add(location);
			return true;
        }
        
        /// <summary>
        /// Remove a previous painted cell. Returns true if state was modified
        /// </summary>
        /// <param name="location">the location of the painted cell to remove, in grid cooridnates</param>
        /// <param name="automaticRebuild">if true, the dungeon would be rebuilt, if the data model has changed due to this request</param>
        public bool RemovePaintCell(IntVector location)
        {
	        if (paintedCells.Contains(location)) {
		        paintedCells.Remove(location);
		        return true;
	        }
	        return false;
        }


        /// <summary>
        /// Clears all overlay data
        /// </summary>
        /// <param name="automaticRebuild"></param>
        public bool ClearToolOverlayData()
        {
	        var stateModified = paintedCells.Count > 0;
			paintedCells.Clear();
			return stateModified;
        }
    }
}