//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Frameworks.Snap;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors
{
    public class AssetBuilder
    {
        public static void CreatePlaceableMarkerAsset(string path)
        {
            var gameObject = new GameObject("PlaceableMarker");
            gameObject.AddComponent<PlaceableMarker>();
            bool success;
            try
            {
                PrefabUtility.SaveAsPrefabAsset(gameObject, path, out success);
            }
            catch
            {
                success = false;
            }

            if (!success)
            {
                Debug.LogError("Failed to create Placeable Marker asset");
            }
            
            Object.DestroyImmediate(gameObject);
        }
        
        public static void CreateSnapConnection(string path)
        {
            // Create the main snap connection game object and attach the snap connection component to it
            var gameObject = new GameObject("SnapConnection");
            var connection = gameObject.AddComponent<SnapConnection>();
            
            // Create child game objects to hold the wall and door
            var wall = new GameObject("Wall");
            var door = new GameObject("Door");
            wall.transform.parent = gameObject.transform;
            door.transform.parent = gameObject.transform;
            
            // Link them up with the connection object
            connection.wallObject = wall;
            connection.doorObject = door;

            bool success;
            try
            {
                PrefabUtility.SaveAsPrefabAsset(gameObject, path, out success);
            }
            catch
            {
                success = false;
            }

            if (!success)
            {
                Debug.LogError("Failed to create Snap Connection asset");
            }

            Object.DestroyImmediate(gameObject);
        }
    }
}