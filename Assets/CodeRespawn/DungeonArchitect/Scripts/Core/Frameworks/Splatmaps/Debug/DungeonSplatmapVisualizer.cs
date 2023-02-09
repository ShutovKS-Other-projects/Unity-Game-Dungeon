//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Splatmap
{
    public class DungeonSplatmapVisualizer : DungeonEventListener
    {
        public Renderer debugRenderer;
        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model) {
            var splatmapComponent = GetComponent<DungeonSplatmap>();
            if (splatmapComponent != null)
            {
                var splatAsset = splatmapComponent.splatmap;
                if (splatAsset != null && splatAsset.splatTextures.Length > 0 && debugRenderer != null)
                {
                    var roadmap = splatAsset.splatTextures[0];
                    debugRenderer.material.mainTexture = roadmap;
                }
            }
        }
    }
}
