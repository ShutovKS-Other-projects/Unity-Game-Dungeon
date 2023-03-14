//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using SVector3 = SharpNav.Geometry.Vector3;
using Triangle3 = SharpNav.Geometry.Triangle3;
using DungeonArchitect.Builders.SimpleCity;

namespace DungeonArchitect.Navigation
{
    public class CityLayoutTriangleProvider : NavigationTriangleProvider
    {
        public Dungeon dungeon;

        public override void AddNavTriangles(List<Triangle3> triangles)
        {
            if (dungeon == null)
            {
                Debug.LogWarning("CityLayoutTriangleProvider: Dungeon is not assigned");
                return;
            }

            var model = dungeon.ActiveModel as SimpleCityDungeonModel;
            if (model == null)
            {
                Debug.LogWarning("CityLayoutTriangleProvider: Dungeon model is invalid. Rebuild the dungeon");
                return;
            }

            var width = model.CityWidth;
            var height = model.CityHeight;
            
            var config = dungeon.Config as SimpleCityDungeonConfig;
            var cellSize2D = config.CellSize;
            var cellSize = new Vector3(cellSize2D.x, 0, cellSize2D.y);

            var verts = new SVector3[4];
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = new SVector3();
            }

            int padding = config.cityWallPadding;

            for (int cx = -padding; cx < width + padding; cx++)
            {
                for (int cz = -padding; cz < height + padding; cz++)
                {
                    var location = Vector3.Scale(new Vector3(cx, 0, cz), cellSize);
                    var size = cellSize;
                    //var bounds = cell.Bounds;
                    //var location = MathUtils.GridToWorld(config.GridCellSize, bounds.Location);
                    //var size = MathUtils.GridToWorld(config.GridCellSize, bounds.Size);

                    verts[0].Set(location.x, location.y, location.z);
                    verts[1].Set(location.x + size.x, location.y, location.z);
                    verts[2].Set(location.x + size.x, location.y, location.z + size.z);
                    verts[3].Set(location.x, location.y, location.z + size.z);

                    triangles.Add(new Triangle3(
                        verts[0],
                        verts[1],
                        verts[2]));

                    triangles.Add(new Triangle3(
                        verts[2],
                        verts[3],
                        verts[0]));
                }
            }
        }

    }
}
