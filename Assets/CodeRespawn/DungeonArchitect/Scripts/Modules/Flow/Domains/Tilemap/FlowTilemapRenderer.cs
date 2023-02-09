//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Tilemap
{
    public class FlowTilemapRenderResources
    {
        public Texture2D iconOneWayDoor = Texture2D.whiteTexture;
        public TexturedMaterialInstances materials;
    }

    /// <summary>
    /// Renders the tilemap on to a render texture
    /// </summary>
    public class FlowTilemapRenderer
    {
        public static void Render(RenderTexture tilemapTexture, FlowTilemap tilemap, int tileSize, FlowTilemapRenderResources resources, System.Func<FlowTilemapCell, bool> FuncCellSelected)
        {
            var oldRTT = RenderTexture.active;
            RenderTexture.active = tilemapTexture;

            GL.PushMatrix();
            GL.LoadOrtho();
            float texWidth = tilemapTexture.width;
            float texHeight = tilemapTexture.height;

            var layers = BuildQuadData(tilemap, tileSize, resources, FuncCellSelected);
            foreach (var layer in layers)
            {
                layer.material.SetPass(0);
                GL.Begin(GL.QUADS);
                var quads = layer.quads;
                foreach (var quad in quads)
                {
                    GL.Color(quad.color);

                    for (int i = 0; i < 4; i++)
                    {
                        var vert = quad.verts[(i + quad.rotateUV) % 4];
                        var uv = vert.uv;
                        GL.TexCoord2(uv.x, uv.y);

                        vert = quad.verts[i];
                        var p = vert.position;
                        GL.Vertex3(p.x, p.y, quad.z);
                    }
                }
                GL.End();
            }

            var lineMaterial = resources.materials.GetMaterial(Texture2D.whiteTexture);
            lineMaterial.SetPass(0);
            // Draw the grid lines
            GL.Begin(GL.LINES);
            GL.Color(new Color(0.0f, 0.0f, 0.0f, 0.1f));
            for (int x = 0; x < tilemap.Width; x++)
            {
                float x0 = (x * tileSize) / texWidth;
                GL.Vertex3(x0, 0, 0);
                GL.Vertex3(x0, 1, 0);
            }
            for (int y = 0; y < tilemap.Height; y++)
            {
                float y0 = (y * tileSize) / texHeight;
                GL.Vertex3(0, y0, 0);
                GL.Vertex3(1, y0, 0);
            }
            GL.End();


            GL.PopMatrix();

            RenderTexture.active = oldRTT;
        }

        static Color GetEdgeColor(FlowTilemapEdgeType edgeType)
        {
            if (edgeType == FlowTilemapEdgeType.Wall) return Color.red;
            else if (edgeType == FlowTilemapEdgeType.Fence) return Color.black;
            else if (edgeType == FlowTilemapEdgeType.Door) return Color.blue;
            else return Color.black;
        }

        static TilemapLayerRenderData[] BuildQuadData(FlowTilemap tilemap, int tileSize, FlowTilemapRenderResources resources, System.Func<FlowTilemapCell, bool> FuncCellSelected)
        {
            var textureSize = new IntVector2(tilemap.Width, tilemap.Height) * tileSize;
            float texWidth = textureSize.x;
            float texHeight = textureSize.y;
            float tileSizeU = tileSize / texWidth;
            float tileSizeV = tileSize / texHeight;

            var quadsByMaterial = new Dictionary<Material, List<TilemapRenderQuad>>();
            var materialDefault = resources.materials.GetMaterial(Texture2D.whiteTexture);
            var oneWayTexture = resources.iconOneWayDoor;

            // Draw the cells
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    var cell = tilemap.Cells[x, y];
                    var selected = FuncCellSelected.Invoke(cell);

                    Color tileColor;
                    bool canUseCustomColor = cell.CellType != FlowTilemapCellType.Door
                        && cell.CellType != FlowTilemapCellType.Wall
                        //&& cell.CellType != FlowTilemapCellType.Empty
                        ;

                    if (canUseCustomColor && cell.UseCustomColor)
                    {
                        tileColor = cell.CustomColor;
                        if (selected)
                        {
                            tileColor = GetSelectedCellColor(tileColor);
                        }
                    }
                    else
                    {
                        tileColor = GetCellColor(cell);
                    }

                    if (cell.CustomCellInfo != null && cell.CellType == FlowTilemapCellType.Custom)
                    {
                        tileColor = cell.CustomCellInfo.defaultColor;
                    }

                    tileColor.a = 1;
                    float x0 = (x * tileSize) / texWidth;
                    float y0 = (y * tileSize) / texHeight;
                    float x1 = x0 + tileSizeU;
                    float y1 = y0 + tileSizeV;

                    TilemapRenderQuad quad;
                    {
                        var v0 = new TilemapRenderVert(new Vector2(x0, y0), new Vector2(0, 1));
                        var v1 = new TilemapRenderVert(new Vector2(x0, y1), new Vector2(0, 0));
                        var v2 = new TilemapRenderVert(new Vector2(x1, y1), new Vector2(1, 0));
                        var v3 = new TilemapRenderVert(new Vector2(x1, y0), new Vector2(1, 1));
                        quad = new TilemapRenderQuad(v0, v1, v2, v3, tileColor, 0);
                        AddLayerQuad(quadsByMaterial, quad, materialDefault);
                    }


                    var overlay = cell.Overlay;
                    if (overlay != null)
                    {
                        float overlayScale = 0.5f;
                        var overlayQuad = quad.Clone();
                        var shrinkY = (overlayQuad.verts[1].position.y - overlayQuad.verts[0].position.y) * Mathf.Clamp01(1 - overlayScale) * 0.5f;
                        var shrinkX = (overlayQuad.verts[2].position.x - overlayQuad.verts[1].position.x) * Mathf.Clamp01(1 - overlayScale) * 0.5f;

                        overlayQuad.verts[0].position.x += shrinkX;
                        overlayQuad.verts[0].position.y += shrinkY;
                        overlayQuad.verts[1].position.x += shrinkX;
                        overlayQuad.verts[1].position.y -= shrinkY;
                        overlayQuad.verts[2].position.x -= shrinkX;
                        overlayQuad.verts[2].position.y -= shrinkY;
                        overlayQuad.verts[3].position.x -= shrinkX;
                        overlayQuad.verts[3].position.y += shrinkY;
                        overlayQuad.color = overlay.color;
                        overlayQuad.z = 1;
                        AddLayerQuad(quadsByMaterial, overlayQuad, materialDefault);
                    }

                    if (cell.CellType == FlowTilemapCellType.Door)
                    {
                        var doorMeta = cell.Userdata as FlowTilemapCellDoorInfo;
                        if (doorMeta != null)
                        {
                            if (doorMeta.oneWay)
                            {
                                var doorQuad = quad.Clone();
                                doorQuad.color = Color.white;
                                if (doorMeta.nodeA.x < doorMeta.nodeB.x) doorQuad.rotateUV = 1;
                                if (doorMeta.nodeA.x > doorMeta.nodeB.x) doorQuad.rotateUV = 3;
                                if (doorMeta.nodeA.y < doorMeta.nodeB.y) doorQuad.rotateUV = 2;
                                if (doorMeta.nodeA.y > doorMeta.nodeB.y) doorQuad.rotateUV = 0;
                                var materialOneWayDoor = resources.materials.GetMaterial(oneWayTexture);

                                AddLayerQuad(quadsByMaterial, doorQuad, materialOneWayDoor);
                            }
                        }
                    }
                }
            }

            // Draw the edges

            for (int y = 0; y <= tilemap.Height; y++)
            {
                for (int x = 0; x <= tilemap.Width; x++)
                {
                    var edgeH = tilemap.Edges.GetHorizontal(x, y);
                    var edgeV = tilemap.Edges.GetVertical(x, y);

                    float x0 = (x * tileSize) / texWidth;
                    float y0 = (y * tileSize) / texHeight;
                    float x1 = x0 + tileSizeU;
                    float y1 = y0 + tileSizeV;

                    float thickness = 0.2f;


                    if (edgeH.EdgeType != FlowTilemapEdgeType.Empty)
                    {
                        var edgeColor = GetEdgeColor(edgeH.EdgeType);
                        float offset = tileSizeV * thickness * 0.5f;

                        if (edgeH.EdgeType == FlowTilemapEdgeType.Door)
                        {
                            offset *= 3;
                        }

                        var ty0 = y0 - offset;
                        var ty1 = y0 + offset;
                        DrawTileEdge(x0, ty0, x1, ty1, edgeH, resources, edgeColor, materialDefault, quadsByMaterial);
                    }

                    if (edgeV.EdgeType != FlowTilemapEdgeType.Empty)
                    {
                        var edgeColor = GetEdgeColor(edgeV.EdgeType);
                        float offset = tileSizeU * thickness * 0.5f;

                        if (edgeV.EdgeType == FlowTilemapEdgeType.Door)
                        {
                            offset *= 3;
                        }

                        var tx0 = x0 - offset;
                        var tx1 = x0 + offset;
                        DrawTileEdge(tx0, y0, tx1, y1, edgeV, resources, edgeColor, materialDefault, quadsByMaterial);
                    }

                }
            }


            var layers = new List<TilemapLayerRenderData>();
            foreach (var entry in quadsByMaterial)
            {
                var layer = new TilemapLayerRenderData();
                layer.material = entry.Key;
                layer.quads = entry.Value.ToArray();
                layers.Add(layer);
            }

            return layers.ToArray();
        }

        static void DrawTileEdge(float x0, float y0, float x1, float y1, FlowTilemapEdge edge, FlowTilemapRenderResources resources, Color color, Material material, Dictionary<Material, List<TilemapRenderQuad>> quadsByMaterial)
        {
            var v0 = new TilemapRenderVert(new Vector2(x0, y0), new Vector2(0, 1));
            var v1 = new TilemapRenderVert(new Vector2(x0, y1), new Vector2(0, 0));
            var v2 = new TilemapRenderVert(new Vector2(x1, y1), new Vector2(1, 0));
            var v3 = new TilemapRenderVert(new Vector2(x1, y0), new Vector2(1, 1));
            var edgeQuad = new TilemapRenderQuad(v0, v1, v2, v3, color, 0);
            AddLayerQuad(quadsByMaterial, edgeQuad, material);

            if (edge != null && edge.EdgeType == FlowTilemapEdgeType.Door)
            {
                var doorMeta = edge.Userdata as FlowTilemapCellDoorInfo;
                if (doorMeta != null)
                {
                    if (doorMeta.oneWay)
                    {
                        var doorQuad = edgeQuad.Clone();
                        doorQuad.color = Color.white;
                        if (doorMeta.nodeA.x < doorMeta.nodeB.x) doorQuad.rotateUV = 1;
                        if (doorMeta.nodeA.x > doorMeta.nodeB.x) doorQuad.rotateUV = 3;
                        if (doorMeta.nodeA.y < doorMeta.nodeB.y) doorQuad.rotateUV = 2;
                        if (doorMeta.nodeA.y > doorMeta.nodeB.y) doorQuad.rotateUV = 0;
                        var materialOneWayDoor = resources.materials.GetMaterial(resources.iconOneWayDoor);

                        AddLayerQuad(quadsByMaterial, doorQuad, materialOneWayDoor);
                    }
                }
            }
        }

        private static void AddLayerQuad(Dictionary<Material, List<TilemapRenderQuad>> quadsByMaterial, TilemapRenderQuad quad, Material material)
        {
            if (!quadsByMaterial.ContainsKey(material))
            {
                quadsByMaterial.Add(material, new List<TilemapRenderQuad>());
            }

            quadsByMaterial[material].Add(quad);
        }

        static Color GetSelectedCellColor(Color color)
        {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            S = Mathf.Clamp01(S * 2);
            return Color.HSVToRGB(H, S, V);
        }

        static Color GetCellColor(FlowTilemapCell cell)
        {
            switch (cell.CellType)
            {
                case FlowTilemapCellType.Empty:
                    return Color.black;

                case FlowTilemapCellType.Floor:
                    return Color.white;

                case FlowTilemapCellType.Door:
                    return Color.blue;

                case FlowTilemapCellType.Wall:
                    return new Color(0.5f, 0.5f, 0.5f);


                default:
                    return Color.magenta;
            }
        }


        struct TilemapRenderVert
        {
            public TilemapRenderVert(Vector2 position, Vector2 uv)
            {
                this.position = position;
                this.uv = uv;
            }

            public Vector2 position;
            public Vector2 uv;

            public TilemapRenderVert Clone()
            {
                return new TilemapRenderVert(position, uv);
            }
        }

        struct TilemapRenderQuad
        {
            public TilemapRenderQuad(TilemapRenderVert v0, TilemapRenderVert v1, TilemapRenderVert v2, TilemapRenderVert v3, Color color, float z)
            {
                verts = new TilemapRenderVert[4];
                verts[0] = v0;
                verts[1] = v1;
                verts[2] = v2;
                verts[3] = v3;
                this.color = color;
                this.z = z;
                rotateUV = 0;
            }

            public TilemapRenderQuad Clone()
            {
                var newQuad = new TilemapRenderQuad(
                    verts[0].Clone(),
                    verts[1].Clone(),
                    verts[2].Clone(),
                    verts[3].Clone(),
                    color, z);

                newQuad.rotateUV = rotateUV;
                return newQuad;
            }

            public TilemapRenderVert[] verts;
            public Color color;
            public float z;
            public int rotateUV;
        }


        struct TilemapLayerRenderData
        {
            public Material material;
            public TilemapRenderQuad[] quads;
        }

    }
}
