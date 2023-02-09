//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    public interface IDungeonSceneObjectInstantiator
    {
        GameObject Instantiate(GameObject template, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent);
    }
    
    public class RuntimeDungeonSceneObjectInstantiator : IDungeonSceneObjectInstantiator
    {
        public GameObject Instantiate(GameObject template, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent)
        {
            var gameObj = MonoBehaviour.Instantiate(template) as GameObject;
            gameObj.transform.SetParent(parent);
            gameObj.transform.position = position;
            gameObj.transform.rotation = rotation;
            gameObj.transform.localScale = scale;
            return gameObj;
        }
    }
}
