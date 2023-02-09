//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Splatmap;

namespace DungeonArchitect.Editors
{
    [CustomEditor(typeof(DungeonSplatmap))]
    public class SplatmapPropertyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                var component = target as DungeonSplatmap;
                if (component.splatmap == null)
                {
                    CreateSplatMapAsset(component);
                }
                else
                {
                    RegenerateSplatmaps(component);
                }
            }
        }

        public static void RegenerateSplatmaps(DungeonSplatmap splatComponent)
        {
            // Destroy all the existing splatmaps
            var asset = splatComponent.splatmap;
            foreach (var splatTexture in asset.splatTextures)
            {
                DestroyImmediate(splatTexture, true);
            }

            var targetTextures = new List<Texture2D>();
            
            foreach (var textureInfo in splatComponent.textures)
            {
                var texSize = textureInfo.textureSize;
                var texFormat = textureInfo.textureFormat;

                var splatTexture = new Texture2D(texSize, texSize, texFormat, false);
                splatTexture.name = textureInfo.id;
                AssetDatabase.AddObjectToAsset(splatTexture, asset);
                targetTextures.Add(splatTexture);
            }

            asset.splatTextures = targetTextures.ToArray();

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateSplatMapAsset(DungeonSplatmap splatComponent)
        {
            if (splatComponent == null)
            {
                // No splatmap attached to this dungeon configuration
                return;
            }

            // Check if the splatmap asset has been assigned
            if (splatComponent.splatmap == null)
            {
                // Create a new splatmap asset in the correct directory and assign it to the dungeon
                var defaultFileName = "DungeonSplatmap.asset";
                var scenePath = DungeonEditorHelper.GetActiveScenePath();
                splatComponent.splatmap = DungeonEditorHelper.CreateAssetInBrowser<DungeonSplatAsset>(scenePath, defaultFileName);
                
                RegenerateSplatmaps(splatComponent);
            }
        }
    }
}
