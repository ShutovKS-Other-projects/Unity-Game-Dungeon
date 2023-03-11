//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.MiniMaps;
using DungeonArchitect.Utils;
using System.Linq;
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Tilemap;
using UnityEngine;

namespace DungeonArchitect.Builders.GridFlow
{
    [System.Serializable]
    public enum GridFlowMinimapInitMode
    {
        OnDungeonRebuild,
        OnPlay,
        Manual
    }

    [System.Serializable]
    public struct GridFlowMinimapIcons
    {
        public Texture2D iconOneWayDoor;
    }

    public class GridFlowMinimap : DungeonMiniMap
    {
        public Shader tileShader;
        public GridFlowMinimapInitMode initMode = GridFlowMinimapInitMode.Manual;
        public GridFlowMinimapIcons icons;
        public bool seeThroughWalls = false;

        FlowTilemapRenderResources resources;
        GridFlowDungeonModel model;
        GridFlowDungeonConfig config;
        int tileSize = 1;
        List<GridFlowMinimapTrackedObject> trackedObjects = new List<GridFlowMinimapTrackedObject>();
        Color[] fogMask;

        protected override bool SupportsFogOfWar { get { return true; } }

        private void Reset()
        {
            compositeShader = Shader.Find("DungeonArchitect/MiniMap/Composite");
            tileShader = Shader.Find("DungeonArchitect/MiniMap/Tile");
            initMode = GridFlowMinimapInitMode.OnDungeonRebuild;
            
        }

        private void Awake()
        {
        }

        private void Start()
        {
            if (initMode == GridFlowMinimapInitMode.OnPlay)
            {
                Initialize();
            }
        }

        public void AddTrackedObject(GridFlowMinimapTrackedObject trackedObject)
        {
            trackedObjects.Add(trackedObject);
        }

        protected override void CreateTextures(IntVector2 desiredSize, out Texture staticImage, out Texture fogOfWar, out Texture overlayImage, out IntVector2 targetTextureSize)
        {
            resources = new FlowTilemapRenderResources();
            resources.materials = new TexturedMaterialInstances(tileShader);
            resources.iconOneWayDoor = icons.iconOneWayDoor;

            if (model == null)
            {
                model = GetComponent<GridFlowDungeonModel>();
            }

            if (config == null)
            {
                config = GetComponent<GridFlowDungeonConfig>();
            }

            if (model == null || config == null)
            {
                Debug.LogError("Cannot find GridFlowDungeonModel component along side the minimap script");
                staticImage = null;
                fogOfWar = null;
                overlayImage = null;
                targetTextureSize = IntVector2.Zero;
                return;
            }

            var tilemap = model.Tilemap;
            if (tilemap == null)
            {
                staticImage = null;
                fogOfWar = null;
                overlayImage = null;
                targetTextureSize = IntVector2.Zero;
                return;
            }

            // Calculate the tile size
            {
                var desiredTileSize2D = new Vector2();
                desiredTileSize2D.x = (float)desiredSize.x / model.Tilemap.Width;
                desiredTileSize2D.y = (float)desiredSize.y / model.Tilemap.Height;
                var desiredTileSizeF = Mathf.Min(desiredTileSize2D.x, desiredTileSize2D.y);
                tileSize = Mathf.FloorToInt(desiredTileSizeF);
            }

            var texWidth = tilemap.Width * tileSize;
            var texHeight = tilemap.Height * tileSize;

            targetTextureSize = new IntVector2(texWidth, texHeight);
            staticImage = new RenderTexture(texWidth, texHeight, 0);
            overlayImage = new RenderTexture(texWidth, texHeight, 0);
            fogOfWar = new Texture2D(tilemap.Width, tilemap.Height, TextureFormat.R8, false, true);
            fogMask = new Color[tilemap.Width * tilemap.Height];
        }

        protected override void UpdateStaticTexture(Texture texture)
        {
            var rtt = texture as RenderTexture;
            if (rtt == null || model == null) return;

            var tilemap = model.Tilemap;
            if (tilemap != null)
            {
                FlowTilemapRenderer.Render(rtt, tilemap, tileSize, resources, cell => false);
            }
        }

        struct FogOfWarItem
        {
            public Vector2 position;
            public float radius;
            public float falloffStart;
        }


        IntVector2[] FogOfWarFilterVisibility(IntVector2[] tiles, IntVector2 player)
        {
            if (seeThroughWalls)
            {
                return tiles;
            }

            var tilemap = model.Tilemap;
            if (tilemap == null)
            {
                return tiles;
            }

            var result = new List<IntVector2>();
            var valid = new HashSet<IntVector2>(tiles);
            var visited = new HashSet<IntVector2>();
            var queue = new Queue<IntVector2>();
            queue.Enqueue(player);
            visited.Add(player);
            while (queue.Count > 0)
            {
                var coord = queue.Dequeue();
                result.Add(coord);

                var cell = tilemap.Cells.GetCell(coord.x, coord.y);
                if (cell == null) continue;
                if (cell.CellType == FlowTilemapCellType.Wall) continue;
                if (cell.CellType == FlowTilemapCellType.Door) continue;

                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        var cx = coord.x + dx;
                        var cy = coord.y + dy;

                        if (cx < 0 || cy < 0 || cx >= tilemap.Width || cy >= tilemap.Height) continue;
                        
                        var childCoord = new IntVector2(cx, cy);
                        if (visited.Contains(childCoord) || !valid.Contains(childCoord))
                        {
                            continue;
                        }
                        visited.Add(childCoord);
                        queue.Enqueue(childCoord);
                    }
                }
            }

            return result.ToArray();
        }

        protected override void UpdateFogOfWarTexture(Texture texture)
        {
            var fogTex = texture as Texture2D;
            var tilemap = model.Tilemap;
            if (fogTex == null || model == null || fogMask == null || tilemap == null) return;

            var items = new List<FogOfWarItem>();
            foreach (var trackedObject in trackedObjects)
            {
                if (trackedObject == null) continue;
                if (trackedObject.exploresFogOfWar)
                {
                    var item = new FogOfWarItem();
                    var localPosition = transform.InverseTransformPoint(trackedObject.transform.position);
                    var gridCoord = new Vector2(
                        localPosition.x / config.gridSize.x,
                        localPosition.z / config.gridSize.z);

                    item.position = gridCoord;
                    item.radius = trackedObject.fogOfWarNumTileRadius;
                    item.falloffStart = trackedObject.fogOfWarLightFalloffStart;
                    items.Add(item);
                }
            }

            foreach (var item in items)
            {
                int cx = Mathf.FloorToInt(item.position.x);
                int cy = Mathf.FloorToInt(item.position.y);

                int x0 = Mathf.FloorToInt(item.position.x - item.radius);
                int x1 = Mathf.FloorToInt(item.position.x + item.radius);
                int y0 = Mathf.FloorToInt(item.position.y - item.radius);
                int y1 = Mathf.FloorToInt(item.position.y + item.radius);

                cx = Mathf.Clamp(cx, 0, tilemap.Width - 1);
                cy = Mathf.Clamp(cy, 0, tilemap.Height - 1);

                x0 = Mathf.Clamp(x0, 0, tilemap.Width - 1);
                x1 = Mathf.Clamp(x1, 0, tilemap.Width - 1);
                y0 = Mathf.Clamp(y0, 0, tilemap.Height - 1);
                y1 = Mathf.Clamp(y1, 0, tilemap.Height - 1);
                var falloff = Mathf.Clamp01(1 - item.falloffStart);
                var tilesToProcess = new List<IntVector2>();
                for (int y = y0; y <= y1; y++)
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        tilesToProcess.Add(new IntVector2(x, y));
                    }
                }

                var visibleTiles = FogOfWarFilterVisibility(tilesToProcess.ToArray(), new IntVector2(cx, cy));
                foreach (var tile in visibleTiles)
                {
                    float distance = (new Vector2(tile.x, tile.y) - item.position).magnitude;
                    distance = Mathf.Clamp01(distance / item.radius);
                    var weight = 1 - distance;

                    weight = weight / falloff;

                    weight = Mathf.Clamp01(weight);

                    int index = tile.y * tilemap.Width + tile.x;
                    fogMask[index].r = Mathf.Max(weight, fogMask[index].r);
                }
            }


            fogTex.SetPixels(fogMask);
            fogTex.Apply(false);
        }

        protected override void UpdateOverlayTexture(Texture texture)
        {
            var rtt = texture as RenderTexture;
            if (rtt == null || model == null || model.Tilemap == null) return;

            trackedObjects = (from trackedObject in trackedObjects
                              where trackedObject != null
                              select trackedObject).ToList();

            var oldRTT = RenderTexture.active;
            RenderTexture.active = rtt;
            
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Clear(true, true, new Color(0, 0, 0, 0), 0);

            var tilemap = model.Tilemap;
            var tileSizeUV = tileSize / (float)texture.width;
            foreach (var trackedObject in trackedObjects)
            {
                if (trackedObject == null || trackedObject.icon == null) continue;
                var color = trackedObject.tint;

                var positionUV = WorldToUVCoord(trackedObject.transform.position, tilemap.Width, tilemap.Height, texture.width, texture.height);
                var iconSize = tileSizeUV * trackedObject.iconScale;
                var hs = iconSize * 0.5f;

                float cx = positionUV.x;
                float cy = positionUV.y;
                var x0 = -hs;
                var x1 = +hs;
                var y0 = -hs;
                var y1 = +hs;

                Quaternion rotation;
                if (trackedObject.rotateIcon)
                {
                    var forward = trackedObject.transform.forward;
                    forward.y = 0;
                    forward = forward.normalized;
                    rotation = Quaternion.LookRotation(forward);
                }
                else
                {
                    rotation = Quaternion.identity;
                }

                var material = resources.materials.GetMaterial(trackedObject.icon);
                material.SetPass(0);

                
                GL.Begin(GL.QUADS);
                EmitVertex(cx, cy, x0, y0, 0, 0, color, rotation);
                EmitVertex(cx, cy, x0, y1, 0, 1, color, rotation);
                EmitVertex(cx, cy, x1, y1, 1, 1, color, rotation);
                EmitVertex(cx, cy, x1, y0, 1, 0, color, rotation);
                GL.End();
            }

            GL.PopMatrix();

            RenderTexture.active = oldRTT;
        }

        Vector2 WorldToUVCoord(Vector3 position, int tilemapWidth, int tilemapHeight, int textureWidth, int textureHeight)
        {
            var minimapWorldPosition = transform.InverseTransformPoint(position);
            var minimapGridCoord = new Vector2(
                minimapWorldPosition.x / config.gridSize.x,
                minimapWorldPosition.z / config.gridSize.z);
            var minimapPixelCoord = minimapGridCoord * tileSize;
            var minimapUVCoord = new Vector2(
                minimapPixelCoord.x / textureWidth,
                minimapPixelCoord.y / textureHeight);

            return minimapUVCoord;
        }

        void EmitVertex(float cx, float cy, float x, float y, float u, float v, Color color, Quaternion rotation)
        {
            var vertex = new Vector3(x, 0, y);
            vertex = rotation * vertex;
            x = vertex.x;
            y = vertex.z;
            GL.Color(color);
            GL.TexCoord2(u, v);
            GL.Vertex3(cx + x, cy + y, 0);

        }

    }
}
