//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
// Delaunay Triangulation by Paul Bourke
// http://paulbourke.net/papers/triangulate/

using UnityEngine;

namespace DungeonArchitect.Triangulator
{
    public struct DelauneyTriangle
    {
        public int p1;
        public int p2;
        public int p3;
    }

    struct IEDGE
    {
        public int p1;
        public int p2;
    }

    public class DelauneyV2
    {
        private static float EPSILON = 1e-6f;
        public static DelauneyTriangle[] Triangulate(Vector2[] vertices)
        {
            var nv = vertices.Length;

            {
                var paddedVerts = new Vector2[vertices.Length + 3];
                System.Array.Copy(vertices, paddedVerts, vertices.Length);
                paddedVerts[nv + 0] = Vector2.zero;
                paddedVerts[nv + 1] = Vector2.zero;
                paddedVerts[nv + 2] = Vector2.zero;
                vertices = paddedVerts;
            }

            var triangles = new DelauneyTriangle[nv * 3];
            for (int m = 0; m < triangles.Length; m++) triangles[m] = new DelauneyTriangle();
            
            int ntri = 0;


            bool[] complete = null;
            IEDGE[] edges = null;
            IEDGE[] p_EdgeTemp = null;
            int nedge = 0;
            int trimax, emax = 200;
            bool inside;
            int i, j, k;
            float xp, yp, x1, y1, x2, y2, x3, y3, xc = 0, yc = 0, r = 0;
            float xmin, xmax, ymin, ymax, xmid, ymid;
            float dx, dy, dmax;

            trimax = 4 * nv;
            complete = new bool[trimax];
            edges = new IEDGE[emax];
            for (int m = 0; m < edges.Length; m++) edges[m] = new IEDGE();

            xmin = vertices[0].x;
            ymin = vertices[0].y;
            xmax = xmin;
            ymax = ymin;
            for (i = 1; i < nv; i++)
            {
                if (vertices[i].x < xmin) xmin = vertices[i].x;
                if (vertices[i].x > xmax) xmax = vertices[i].x;
                if (vertices[i].y < ymin) ymin = vertices[i].y;
                if (vertices[i].y > ymax) ymax = vertices[i].y;
            }
            dx = xmax - xmin;
            dy = ymax - ymin;
            dmax = (dx > dy) ? dx : dy;
            xmid = (xmax + xmin) / 2.0f;
            ymid = (ymax + ymin) / 2.0f;
            vertices[nv + 0].x = xmid - 20 * dmax;
            vertices[nv + 0].y = ymid - dmax;
            vertices[nv + 1].x = xmid;
            vertices[nv + 1].y = ymid + 20 * dmax;
            vertices[nv + 2].x = xmid + 20 * dmax;
            vertices[nv + 2].y = ymid - dmax;
            triangles[0].p1 = nv;
            triangles[0].p2 = nv + 1;
            triangles[0].p3 = nv + 2;
            complete[0] = false;
            ntri = 1;

            for (i = 0; i < nv; i++)
            {
                xp = vertices[i].x;
                yp = vertices[i].y;
                nedge = 0;

                for (j = 0; j < ntri; j++)
                {
                    if (complete[j])
                        continue;
                    x1 = vertices[triangles[j].p1].x;
                    y1 = vertices[triangles[j].p1].y;
                    x2 = vertices[triangles[j].p2].x;
                    y2 = vertices[triangles[j].p2].y;
                    x3 = vertices[triangles[j].p3].x;
                    y3 = vertices[triangles[j].p3].y;
                    inside = CircumCircle(xp, yp, x1, y1, x2, y2, x3, y3, ref xc, ref yc, ref r);
                    if (xc + r < xp)
                        complete[j] = true;
                    if (inside)
                    {
                        if (nedge + 3 >= emax)
                        {
                            emax += 100;
                            p_EdgeTemp = new IEDGE[emax];
                            for (int m = 0; m < p_EdgeTemp.Length; m++) p_EdgeTemp[m] = new IEDGE();

                            for (int ik = 0; ik < nedge; ik++)
                            {
                                p_EdgeTemp[ik] = edges[ik];
                            }
                            edges = p_EdgeTemp;
                        }
                        edges[nedge + 0].p1 = triangles[j].p1;
                        edges[nedge + 0].p2 = triangles[j].p2;
                        edges[nedge + 1].p1 = triangles[j].p2;
                        edges[nedge + 1].p2 = triangles[j].p3;
                        edges[nedge + 2].p1 = triangles[j].p3;
                        edges[nedge + 2].p2 = triangles[j].p1;
                        nedge += 3;
                        triangles[j] = triangles[ntri - 1];
                        complete[j] = complete[ntri - 1];
                        ntri--;
                        j--;
                    }
                }
                for (j = 0; j < nedge - 1; j++)
                {
                    for (k = j + 1; k < nedge; k++)
                    {
                        if ((edges[j].p1 == edges[k].p2) && (edges[j].p2 == edges[k].p1))
                        {
                            edges[j].p1 = -1;
                            edges[j].p2 = -1;
                            edges[k].p1 = -1;
                            edges[k].p2 = -1;
                        }
                        if ((edges[j].p1 == edges[k].p1) && (edges[j].p2 == edges[k].p2))
                        {
                            edges[j].p1 = -1;
                            edges[j].p2 = -1;
                            edges[k].p1 = -1;
                            edges[k].p2 = -1;
                        }
                    }
                }
                for (j = 0; j < nedge; j++)
                {
                    if (edges[j].p1 < 0 || edges[j].p2 < 0)
                        continue;
                    triangles[ntri].p1 = edges[j].p1;
                    triangles[ntri].p2 = edges[j].p2;
                    triangles[ntri].p3 = i;
                    complete[ntri] = false;
                    ntri++;
                }
            }
            for (i = 0; i < ntri; i++)
            {
                if (triangles[i].p1 >= nv || triangles[i].p2 >= nv || triangles[i].p3 >= nv)
                {
                    triangles[i] = triangles[ntri - 1];
                    ntri--;
                    i--;
                }
            }

            var result = new DelauneyTriangle[ntri];
            System.Array.Copy(triangles, result, ntri);
            return result;
        }

        static bool CircumCircle(float xp, float yp, float x1, float y1, float x2,
            float y2, float x3, float y3, ref float xc, ref float yc, ref float r)
        {
            float m1, m2, mx1, mx2, my1, my2;
            float dx, dy, rsqr, drsqr;

            /* Check for coincident points */
            if (Mathf.Abs(y1 - y2) < EPSILON && Mathf.Abs(y2 - y3) < EPSILON)
            {
                return false;
            }

            if (Mathf.Abs(y2 - y1) < EPSILON)
            {
                m2 = -(x3 - x2) / (y3 - y2);
                mx2 = (x2 + x3) / 2.0f;
                my2 = (y2 + y3) / 2.0f;
                xc = (x2 + x1) / 2.0f;
                yc = m2 * (xc - mx2) + my2;
            }
            else if (Mathf.Abs(y3 - y2) < EPSILON)
            {
                m1 = -(x2 - x1) / (y2 - y1);
                mx1 = (x1 + x2) / 2.0f;
                my1 = (y1 + y2) / 2.0f;
                xc = (x3 + x2) / 2.0f;
                yc = m1 * (xc - mx1) + my1;
            }
            else
            {
                m1 = -(x2 - x1) / (y2 - y1);
                m2 = -(x3 - x2) / (y3 - y2);
                mx1 = (x1 + x2) / 2.0f;
                mx2 = (x2 + x3) / 2.0f;
                my1 = (y1 + y2) / 2.0f;
                my2 = (y2 + y3) / 2.0f;
                xc = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
                yc = m1 * (xc - mx1) + my1;
            }
            dx = x2 - xc;
            dy = y2 - yc;
            rsqr = dx * dx + dy * dy;
            r = Mathf.Sqrt(rsqr);
            dx = xp - xc;
            dy = yp - yc;
            drsqr = dx * dx + dy * dy;
            return (drsqr <= rsqr) ? true : false;
        }
    }
}
