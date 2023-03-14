//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Builders.Maze
{
	public static class MazeDungeonMarkerNames
    {
        public static readonly string GroundBlock = "GroundBlock";
        public static readonly string WallBlock = "WallBlock";
    }

	public class MazeDungeonBuilder : DungeonBuilder {

		MazeDungeonConfig MazeConfig;
		MazeDungeonModel MazeModel;

		new System.Random random;

		/// <summary>
		/// Builds the dungeon layout.  In this method, you should build your dungeon layout and save it in your model file
		/// No markers should be emitted here.   (EmitMarkers function will be called later by the engine to do that)
		/// </summary>
		/// <param name="config">The builder configuration</param>
		/// <param name="model">The dungeon model that the builder will populate</param>
		public override void BuildDungeon(DungeonConfig config, DungeonModel model)
		{
			base.BuildDungeon(config, model);

			random = new System.Random((int)config.Seed);

			// We know that the dungeon prefab would have the appropriate config and models attached to it
			// Cast and save it for future reference
			MazeConfig = config as MazeDungeonConfig;
			MazeModel = model as MazeDungeonModel;
			MazeModel.Config = MazeConfig;

			// Generate the city layout and save it in a model.   No markers are emitted here. 
			GenerateLevelLayout();
		}

		/// <summary>
		/// Override the builder's emit marker function to emit our own markers based on the layout that we built
		/// You should emit your markers based on the layout you have saved in the model generated previously
		/// When the user is designing the theme interactively, this function will be called whenever the graph state changes,
		/// so the theme engine can populate the scene (BuildDungeon will not be called if there is no need to rebuild the layout again)
		/// </summary>
		public override void EmitMarkers()
		{
			base.EmitMarkers();

			EmitLevelMarkers();

			ProcessMarkerOverrideVolumes();
		}

        bool IsVisited(bool[,] visited, int x, int y)
        {
            if (x < 0 || y < 0 || x >= visited.GetLength(0) || y >= visited.GetLength(1)) return true;
            return visited[x, y];
        }

        bool CanDigToPoint(IntVector2 point, bool[,] visited)
        {
            if (IsVisited(visited, point.x, point.y))
            {
                // Already visited
                return false;
            }

            int neighborPathways = 0;
            neighborPathways += IsVisited(visited, point.x - 1, point.y + 0) ? 1 : 0;
            neighborPathways += IsVisited(visited, point.x + 1, point.y + 0) ? 1 : 0;
            neighborPathways += IsVisited(visited, point.x + 0, point.y + 1) ? 1 : 0;
            neighborPathways += IsVisited(visited, point.x + 0, point.y - 1) ? 1 : 0;
            /*
            neighborPathways += IsVisited(visited, point.x - 1, point.y + 1) ? 1 : 0;
            neighborPathways += IsVisited(visited, point.x + 1, point.y + 1) ? 1 : 0;
            neighborPathways += IsVisited(visited, point.x - 1, point.y - 1) ? 1 : 0;
            neighborPathways += IsVisited(visited, point.x + 1, point.y - 1) ? 1 : 0;
            */

            return neighborPathways <= 1;
        }

        bool GetNextNeighbor(IntVector2 currentPoint, out IntVector2 nextPoint, bool[,] visited)
        {
            var offsets = new List<IntVector2>
            {
                new IntVector2(-1, 0),
                new IntVector2(1, 0),
                new IntVector2(0, -1),
                new IntVector2(0, 1)
            };

            while (offsets.Count > 0)
            {
                int i = random.Next() % offsets.Count;
                var offset = offsets[i];
                offsets.RemoveAt(i);

                if (CanDigToPoint(currentPoint + offset, visited))
                {
                    nextPoint = currentPoint + offset;
                    return true;
                }
            }

            nextPoint = currentPoint;
            return false;
            
        }

		void GenerateLevelLayout()
        {
            MazeConfig.mazeWidth = Mathf.Max(MazeConfig.mazeWidth, 6);
            MazeConfig.mazeHeight = Mathf.Max(MazeConfig.mazeHeight, 6);

            int w = MazeConfig.mazeWidth;
            int h = MazeConfig.mazeHeight;

            var visited = new bool[MazeConfig.mazeWidth, MazeConfig.mazeHeight];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    //bool boundary = (x == 0 || y == 0 || x == w - 1 || y == h - 1);
                    visited[x, y] = false;
                }
            }

            var stack = new Stack<IntVector2>();
            {
                var startPoint = new IntVector2(0, 2 + random.Next() % (MazeConfig.mazeHeight - 4));
                visited[startPoint.x, startPoint.y] = true;
                stack.Push(startPoint);
            }

            while (stack.Count > 0)
            {
                var currentPoint = stack.Peek();
                IntVector2 nextPoint;
                if (GetNextNeighbor(currentPoint, out nextPoint, visited))
                {
                    visited[nextPoint.x, nextPoint.y] = true;
                    stack.Push(nextPoint);
                }
                else
                {
                    stack.Pop();
                }
            }

            // Fill in the model
            MazeModel.tileStates = new MazeTileState[w, h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    MazeModel.tileStates[x, y] = visited[x, y] ? MazeTileState.Empty : MazeTileState.Blocked;
                }
            }
        }

		void EmitLevelMarkers()
        {
            var gridSize = new Vector3(MazeConfig.gridSize.x, 0, MazeConfig.gridSize.y);
            int w = MazeConfig.mazeWidth;
            int h = MazeConfig.mazeHeight;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    var position = Vector3.Scale(new Vector3(x + 0.5f, 0, y + 0.5f), gridSize);
                    var markerTransform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                    var markerName = MazeModel.tileStates[x, y] == MazeTileState.Blocked
                            ? MazeDungeonMarkerNames.WallBlock
                            : MazeDungeonMarkerNames.GroundBlock;
                    EmitMarker(markerName, markerTransform, new IntVector(x, 0, y), -1);
                }
            }

        } 

	}
}