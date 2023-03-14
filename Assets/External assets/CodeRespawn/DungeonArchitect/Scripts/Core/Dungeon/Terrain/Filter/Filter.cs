//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect {

    /// <summary>
    /// A data filter applied over a 2D data array
    /// </summary>
	public interface Filter {
		float[,] ApplyFilter(float[,] data);
	}
}
