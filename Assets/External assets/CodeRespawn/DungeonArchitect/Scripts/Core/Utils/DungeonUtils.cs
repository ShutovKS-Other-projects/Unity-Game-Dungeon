//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect;

namespace DungeonArchitect
{
    public class DungeonUtils
    {

        public static List<GameObject> GetDungeonObjects(Dungeon dungeon)
        {
            var result = new List<GameObject>();

            var components = GameObject.FindObjectsOfType<DungeonSceneProviderData>();
            foreach (var component in components)
            {
                if (component.dungeon == dungeon)
                {
                    result.Add(component.gameObject);
                }
            }

            return result;
        }

        public static void DestroyObject(Object go)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(go);
            }
            else
            {
                Object.DestroyImmediate(go);
            }
        }

        public static Bounds GetDungeonBounds(Dungeon dungeon)
        {
            var dungeonObjects = GetDungeonObjects(dungeon);
            var bounds = new Bounds();
            bool first = true;
            foreach (var gameObject in dungeonObjects)
            {
                var renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    if (first)
                    {
                        bounds = renderer.bounds;
                        first = false;
                    }
                    else
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
            }

            return bounds;
        }
    }
}
