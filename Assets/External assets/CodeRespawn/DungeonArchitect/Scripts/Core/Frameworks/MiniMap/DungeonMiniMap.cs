//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.MiniMaps
{
    public abstract class DungeonMiniMap : MonoBehaviour
    {
        public float updateFrequency = 0.05f;
        public bool enableFogOfWar = false;
        public RenderTexture minimapTexture;
        public Shader compositeShader;

        IntVector2 textureSize;
        Texture staticImage = null;
        Texture fogOfWar = null;
        Texture overlayImage = null;
        float timeSinceLastUpdate = 0;
        bool initialized = false;

        protected Vector4 uvTransform;
        protected abstract bool SupportsFogOfWar { get; }
        protected abstract void CreateTextures(IntVector2 desiredSize, out Texture staticImage, out Texture fogOfWar, out Texture overlayImage, out IntVector2 targetTextureSize);

        protected abstract void UpdateStaticTexture(Texture texture);
        protected abstract void UpdateFogOfWarTexture(Texture texture);
        protected abstract void UpdateOverlayTexture(Texture texture);

        public void Initialize()
        {
            if (minimapTexture != null)
            {
                IntVector2 desiredSize = new IntVector2(minimapTexture.width, minimapTexture.height);
                CreateTextures(desiredSize, out staticImage, out fogOfWar, out overlayImage, out textureSize);
                timeSinceLastUpdate = 0;

                UpdateStaticTexture(staticImage);
                UpdateDynamicTextures();
                RenderFinalImage();
                uvTransform = GetCompositeUVTransform();
                initialized = true;
            }
        }

        void UpdateDynamicTextures()
        {
            UpdateOverlayTexture(overlayImage);
            if (enableFogOfWar)
            {
                UpdateFogOfWarTexture(fogOfWar);
            }
        }

        private bool RequiresUpdate()
        {
            if (!initialized) return false;

            var currentTime = Time.time;
            return (currentTime - timeSinceLastUpdate > updateFrequency);
        }

        private void Update()
        {
            if (RequiresUpdate())
            {
                UpdateDynamicTextures();
                RenderFinalImage();
                timeSinceLastUpdate = Time.time;
            }
        }

        Vector4 GetCompositeUVTransform()
        {
            var maxSize = (float)Mathf.Max(textureSize.x, textureSize.y);
            var uvw = textureSize.x / maxSize;
            var uvh = textureSize.y / maxSize;
            var uvx = (1.0f - uvw) * 0.5f;
            var uvy = (1.0f - uvh) * 0.5f;
            return new Vector4(uvx, uvy, uvw, uvh);
        }

        void RenderFinalImage()
        {
            if (staticImage == null || minimapTexture == null) return;

            var oldRTT = RenderTexture.active;
            RenderTexture.active = minimapTexture;

            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Clear(true, true, new Color(0, 0, 0, 0), 0);

            var material = new Material(compositeShader);
            material.SetTexture("_LayoutTex", staticImage);
            material.SetTexture("_OverlayTex", overlayImage);
            material.SetTexture("_FowTex", fogOfWar);
            material.SetVector("_UVTransform", uvTransform);
            material.SetInt("_FowEnabled", enableFogOfWar ? 1 : 0);
            material.SetPass(0);

            GL.Begin(GL.QUADS);

            GL.Color(Color.white);
            GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, 0);

            GL.Color(Color.white);
            GL.TexCoord2(0, 1);
            GL.Vertex3(0, 1, 0);

            GL.Color(Color.white);
            GL.TexCoord2(1, 1);
            GL.Vertex3(1, 1, 0);

            GL.Color(Color.white);
            GL.TexCoord2(1, 0);
            GL.Vertex3(1, 0, 0);

            GL.End();
            GL.PopMatrix();

            RenderTexture.active = oldRTT;
        }

    }
}
