//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Landscape
{
    public class LandscapeTransformerBase : DungeonEventListener
    {
        public Terrain terrain;

        [HideInInspector]
        [SerializeField]
        bool cachedTerrainDataRestored = false;

        [HideInInspector]
        [SerializeField]
        Rect worldBounds;

        public DungeonLandscapeRestorationCache landscapeRestorationCache;

        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            if (!landscapeRestorationCache)
            {
                Debug.LogError("Landscape restoration cache asset is not specified. Landscape generation will not proceed");
                return;
            }

            // First restore the landscape before baking the new dungeon layout (this removes the old layout)
            if (!cachedTerrainDataRestored)
            {
                // Restore the data
                RestoreLandscapeData(model);
                cachedTerrainDataRestored = true;
            }

            // Save the new layout on the cache asset
            SaveLandscapeData(model);

            // Bake the new dungeon layout on to the terrain
            BuildTerrain(model);
            cachedTerrainDataRestored = false;
        }

        public override void OnDungeonDestroyed(Dungeon dungeon) {
            // Restore the landscape data (by removing the baked dungeon) when the dungeon is destroyed
            RestoreLandscapeData(dungeon.ActiveModel);
            cachedTerrainDataRestored = true;
        }

        protected virtual void BuildTerrain(DungeonModel model) { }
        protected virtual Rect GetDungeonBounds(DungeonModel model) { return Rect.zero; }

        void SaveLandscapeData(DungeonModel model)
        {
            if (landscapeRestorationCache)
            {
                worldBounds = GetDungeonBounds(model);
                landscapeRestorationCache.SaveLandscapeData(terrain, worldBounds);
            }
        }

        void RestoreLandscapeData(DungeonModel model)
        {
            if (landscapeRestorationCache)
            {
                landscapeRestorationCache.RestoreLandscapeData(terrain, worldBounds);
            }
        }
    }
}
