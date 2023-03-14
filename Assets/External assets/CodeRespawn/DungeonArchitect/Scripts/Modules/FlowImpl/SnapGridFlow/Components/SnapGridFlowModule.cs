//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow.Components
{
    public class SnapGridFlowModule : MonoBehaviour
    {
        public Vector3Int numChunks = new Vector3Int(1, 1, 1);
        public SnapGridFlowModuleBounds moduleBounds;
        public bool drawBounds = true;

        void OnDrawGizmosSelected()
        {
            DrawGizmo(true);
        }

        void OnDrawGizmos()
        {
            DrawGizmo(false);
        }

        void DrawGizmo(bool selected)
        {
            if (!drawBounds || moduleBounds == null) return;
            var localToWorld = transform.localToWorldMatrix;
            
            var boxSize = Vector3.Scale(moduleBounds.chunkSize, MathUtils.ToVector3(numChunks));
            var extent = boxSize * 0.5f;
            var center = extent;
            
            // Draw the module bounds
            Gizmos.color = moduleBounds.boundsColor;
            DrawWireCube(localToWorld, center, extent);
            //Gizmos.DrawWireCube(center, boxSize);

            // Draw internal chunk bounds
            {
                var wireColor = moduleBounds.boundsColor;
                wireColor *= 0.5f;
                Gizmos.color = wireColor;
                
                Func<int, Vector2, Vector3Int, Vector3> funcCoordX = (i, p, chunkCount) => new Vector3(i, p.x * chunkCount.y, p.y * chunkCount.z);
                Func<int, Vector2, Vector3Int, Vector3> funcCoordY = (i, p, chunkCount) => new Vector3(p.x * chunkCount.x, i, p.y * chunkCount.z);
                Func<int, Vector2, Vector3Int, Vector3> funcCoordZ = (i, p, chunkCount) => new Vector3(p.x * chunkCount.x, p.y * chunkCount.y, i);

                Func<Vector3Int, int> funcSizeX = (v) => v.x;
                Func<Vector3Int, int> funcSizeY = (v) => v.y;
                Func<Vector3Int, int> funcSizeZ = (v) => v.z;

                DrawInterChunkBounds(localToWorld, funcCoordX, funcSizeX);
                DrawInterChunkBounds(localToWorld, funcCoordY, funcSizeY);
                DrawInterChunkBounds(localToWorld, funcCoordZ, funcSizeZ);
            }
            
            // Draw door indicators
            {
                Gizmos.color = moduleBounds.doorColor;
                var offsetY = moduleBounds.doorOffsetY;

                Vector3[] doorPoints = new Vector3[Constants.DoorPoints.Length];
                for (var i = 0; i < doorPoints.Length; i++)
                {
                    doorPoints[i] = Constants.DoorPoints[i] * moduleBounds.doorDrawSize;
                }
                Vector3[] verticalDoorPoints = new Vector3[Constants.VerticalDoorPoints.Length];
                for (var i = 0; i < verticalDoorPoints.Length; i++)
                {
                    verticalDoorPoints[i] = Constants.VerticalDoorPoints[i] * moduleBounds.doorDrawSize;
                }

                var localToWorldMatrix = transform.localToWorldMatrix;
                
                // Draw along the X-axis
                {
                    var rotationX = Quaternion.identity;
                    for (int x = 0; x < numChunks.x; x++)
                    {
                        for (int y = 0; y < numChunks.y; y++)
                        {
                            var coordA = new Vector3(x + 0.5f, y, 0);
                            var coordB = new Vector3(x + 0.5f, y, numChunks.z);
                            var rotation = Quaternion.identity;

                            var doorPosA = Vector3.Scale(coordA, moduleBounds.chunkSize);
                            var doorPosB = Vector3.Scale(coordB, moduleBounds.chunkSize);

                            doorPosA.y += offsetY;
                            doorPosB.y += offsetY;

                            var transformA = localToWorldMatrix * Matrix4x4.TRS(doorPosA, rotationX, Vector3.one);
                            DrawLines(transformA, doorPoints);

                            var transformB = localToWorldMatrix * Matrix4x4.TRS(doorPosB, rotationX, Vector3.one);
                            DrawLines(transformB, doorPoints);
                        }
                    }
                }

                // Draw along the Z-axis
                {
                    var rotationZ = Quaternion.AngleAxis(90, Vector3.up);
                    for (int z = 0; z < numChunks.z; z++)
                    {
                        for (int y = 0; y < numChunks.y; y++)
                        {
                            var coordA = new Vector3(0, y, z + 0.5f);
                            var coordB = new Vector3(numChunks.x, y, z + 0.5f);
                            var rotation = Quaternion.identity;

                            var doorPosA = Vector3.Scale(coordA, moduleBounds.chunkSize);
                            var doorPosB = Vector3.Scale(coordB, moduleBounds.chunkSize);

                            doorPosA.y += offsetY;
                            doorPosB.y += offsetY;

                            var transformA = localToWorldMatrix * Matrix4x4.TRS(doorPosA, rotationZ, Vector3.one);
                            DrawLines(transformA, doorPoints);

                            var transformB = localToWorldMatrix * Matrix4x4.TRS(doorPosB, rotationZ, Vector3.one);
                            DrawLines(transformB, doorPoints);
                        }
                    }
                }
                
                // Draw along the Y-axis
                {
                    var rotationY = Quaternion.identity;
                    
                    for (int x = 0; x < numChunks.x; x++)
                    {
                        for (int z = 0; z < numChunks.z; z++)
                        {
                            var coordA = new Vector3(x + 0.5f, 0, z + 0.5f);
                            var coordB = new Vector3(x + 0.5f, numChunks.y, z + 0.5f);
                            var rotation = Quaternion.identity;

                            var doorPosA = Vector3.Scale(coordA, moduleBounds.chunkSize);
                            var doorPosB = Vector3.Scale(coordB, moduleBounds.chunkSize);

                            var transformA = localToWorldMatrix * Matrix4x4.TRS(doorPosA, rotationY, Vector3.one);
                            DrawLines(transformA, verticalDoorPoints);

                            var transformB = localToWorldMatrix * Matrix4x4.TRS(doorPosB, rotationY, Vector3.one);
                            DrawLines(transformB, verticalDoorPoints);
                        }
                    }
                }
            }
        }

        class Constants
        {
            public static readonly Vector2[] LocalPoints = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            static readonly float doorSizeX = 0.5f;
            static readonly float doorSizeY = 1;

            public static readonly Vector3[] DoorPoints = new Vector3[]
            {
                // Horizontal Line
                new Vector3(-doorSizeX, 0, 0),
                new Vector3(doorSizeX, 0, 0),

                // Vertical Line
                new Vector3(0, 0, 0),
                new Vector3(0, doorSizeY, 0)
            };

            public static readonly Vector3[] VerticalDoorPoints;

            static Constants()
            {
                // Build the vertical door points
                {
                    float circleRadius = 0.5f;
                    float plusRadius = 0.25f;
                    var circlePoints  = new List<Vector3>();
                    int numPoints = 12;
                    for (int i = 0; i < numPoints; i++)
                    {
                        var angle = i / (float) numPoints * 2 * Mathf.PI;
                        var x = Mathf.Cos(angle) * circleRadius;
                        var z = Mathf.Sin(angle) * circleRadius;
                        circlePoints.Add(new Vector3(x, 0, z));
                    }

                    var verticalDoorPoints = new List<Vector3>();
                    for (var i = 0; i < circlePoints.Count; i++)
                    {
                        var p0 = circlePoints[i];
                        var p1 = circlePoints[(i + 1) % circlePoints.Count];
                        verticalDoorPoints.Add(p0);
                        verticalDoorPoints.Add(p1);
                    }

                    verticalDoorPoints.Add(new Vector3(-plusRadius, 0, 0));
                    verticalDoorPoints.Add(new Vector3(plusRadius, 0, 0));
                    verticalDoorPoints.Add(new Vector3(0, 0, -plusRadius));
                    verticalDoorPoints.Add(new Vector3(0, 0, plusRadius));

                    VerticalDoorPoints = verticalDoorPoints.ToArray();
                }
            }
        }

        void DrawLines(Matrix4x4 transform, Vector3[] points)
        {
            for (var i = 0; i + 1 < points.Length; i += 2)
            {
                var p0 = transform.MultiplyPoint(points[i]);
                var p1 = transform.MultiplyPoint(points[i + 1]);
                Gizmos.DrawLine(p0, p1);
            }
        }
        
        void DrawInterChunkBounds(Matrix4x4 transform, Func<int, Vector2, Vector3Int, Vector3> funcCoord, Func<Vector3Int, int> funcSize)
        {
            int count = funcSize(numChunks);
            for (int i = 1; i < count; i++)
            {
                var points = new List<Vector3>();
                foreach (var localPoint in Constants.LocalPoints)
                {
                    var coord = funcCoord(i, localPoint, numChunks);
                    points.Add(Vector3.Scale(coord, moduleBounds.chunkSize));
                }
                    
                // Draw the points
                for (var ip = 0; ip < points.Count; ip++)
                {
                    var p0 = transform.MultiplyPoint(points[ip]);
                    var p1 = transform.MultiplyPoint(points[(ip + 1) % points.Count]);
                    Gizmos.DrawLine(p0, p1);
                }
            }
        }

        private static readonly Vector3[] LocalCubeVerts = new Vector3[]
        {
            new Vector3(-1, -1, -1),
            new Vector3(1, -1, -1),
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, 1),
        };
        
        void DrawWireCube(Matrix4x4 transform, Vector3 center, Vector3 extent)
        {
            var boxLines = new List<Vector3>();
            var wirePoints = new List<Vector3>();
            foreach (var localVert in LocalCubeVerts)
            {
                wirePoints.Add(Vector3.Scale(localVert, extent));
            }
            
            // Bottom
            for (int i = 0; i < 4; i++)
            {
                boxLines.Add(wirePoints[i]);
                boxLines.Add(wirePoints[(i + 1) % 4]);
            }
            
            // Top
            for (int i = 0; i < 4; i++)
            {
                boxLines.Add(wirePoints[4 + i]);
                boxLines.Add(wirePoints[4 + (i + 1) % 4]);
            }
            
            // Sides
            for (int i = 0; i < 4; i++)
            {
                boxLines.Add(wirePoints[i]);
                boxLines.Add(wirePoints[i + 4]);
            }

            var boxTransform = transform * Matrix4x4.Translate(center);
            DrawLines(boxTransform, boxLines.ToArray());
        }
    }
}