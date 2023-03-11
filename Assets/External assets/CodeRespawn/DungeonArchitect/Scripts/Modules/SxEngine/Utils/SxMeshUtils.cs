//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.SxEngine.Utils
{
    public class SxMeshUtils
    {
        public static SxMesh CreateGridMesh(int numCells, float cellSize)
        {
            var gridMesh = new SxMesh();

            var vertices = new List<SxMeshVertex>();
        
            float start = -(numCells / 2) * cellSize;
            float end = start + numCells * cellSize;
            var gridColor = new Color(0, 0, 0, 0.05f);
            for (int i = 0; i <= numCells; i++)
            {
                var a = (i - numCells / 2) * cellSize;

                var colorX = gridColor;
                var colorZ = gridColor;
                vertices.Add(new SxMeshVertex(new Vector3(start, 0, a), colorX));
                vertices.Add(new SxMeshVertex(new Vector3(end, 0, a), colorX));

                vertices.Add(new SxMeshVertex(new Vector3(a, 0, start), colorZ));
                vertices.Add(new SxMeshVertex(new Vector3(a, 0, end), colorZ));
                
                if (i == numCells / 2)
                {
                    colorX = new Color(1, 0.5f, 0.5f, 1);
                    colorZ = new Color(0.5f, 0.5f, 1, 1);
                    
                    vertices.Add(new SxMeshVertex(new Vector3(0, 0, a), colorX));
                    vertices.Add(new SxMeshVertex(new Vector3(1, 0, a), colorX));

                    vertices.Add(new SxMeshVertex(new Vector3(a, 0, 0), colorZ));
                    vertices.Add(new SxMeshVertex(new Vector3(a, 0, 1), colorZ));
                }
            }
        
            vertices.Add(new SxMeshVertex(new Vector3(0, 0, 0), Color.green));
            vertices.Add(new SxMeshVertex(new Vector3(0, 1, 0), Color.green));
            
            gridMesh.CreateSection(0, GL.LINES, vertices.ToArray());
            return gridMesh;
        }
    }
}