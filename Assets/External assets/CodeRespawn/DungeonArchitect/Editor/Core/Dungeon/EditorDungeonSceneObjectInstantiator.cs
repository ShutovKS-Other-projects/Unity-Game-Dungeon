//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors
{
    public class EditorDungeonSceneObjectInstantiator : IDungeonSceneObjectInstantiator
    {
        public GameObject Instantiate(GameObject template, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent)
        {
            var gameObj = PrefabUtility.InstantiatePrefab(template) as GameObject;
            gameObj.transform.SetParent(parent);
            gameObj.transform.position = position;
            gameObj.transform.rotation = rotation;
            gameObj.transform.localScale = scale;
            return gameObj;
        }
    }
}
