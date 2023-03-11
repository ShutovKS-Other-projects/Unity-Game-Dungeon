//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using DungeonArchitect.Themeing;

namespace DungeonArchitect
{
    /// <summary>
    /// Implementation of the Scene provider that adds object pooling over the existing functionality.
    /// This is useful for quick rebuilding and better performance, as object in the scene are reused
    /// while rebuilding, instead of destroying everything and rebuilding
    /// </summary>
	public class PooledDungeonSceneProvider : DungeonSceneProvider {
		// Pools list of game objects by their node ids
		Dictionary<string, Queue<GameObject>> pooledObjects = new Dictionary<string, Queue<GameObject>>();

		public override void OnDungeonBuildStart() {
            base.OnDungeonBuildStart();
			pooledObjects.Clear ();
			var items = GameObject.FindObjectsOfType<DungeonSceneProviderData>();
			foreach (var item in items) {
                if (item == null || item.externallyManaged) continue;
                if (item.dungeon != this.dungeon) continue;
                if (item.NodeId == null) continue;

				if (!pooledObjects.ContainsKey(item.NodeId)) {
					pooledObjects.Add(item.NodeId, new Queue<GameObject>());
				}
				pooledObjects[item.NodeId].Enqueue(item.gameObject);
			}
		}

		public override void OnDungeonBuildStop() {
			// Destroy all unused objects from the pool
			foreach (var objects in pooledObjects.Values) {
				foreach (var obj in objects) {
					if (Application.isPlaying) {
						Destroy(obj);
					} else {
						DestroyImmediate(obj);
					}
				}
			}

			pooledObjects.Clear ();
		}

		public override GameObject AddSprite(SpriteDungeonThemeItem spriteProp, Matrix4x4 transform, IDungeonSceneObjectInstantiator objectInstantiator) {
			if (spriteProp == null) return null;
			string NodeId = spriteProp.NodeId;
			
			if (spriteProp.sprite == null) {
				return null;
			}

			FlipSpriteTransform(ref transform, spriteProp.sprite);

			GameObject item = null;
			// Try to reuse an object from the pool
			if (pooledObjects.ContainsKey (NodeId) && pooledObjects [NodeId].Count > 0) {
				item = pooledObjects [NodeId].Dequeue ();
				SetTransform (item.transform, transform);
			} else {
				// Pool is exhausted for this object
				item = BuildSpriteObject(spriteProp, transform, NodeId);
			}
			
			SetStaticState(item, spriteProp.StaticState);

            return item;
		}
        
        public override void InvalidateNodeCache(string NodeId) {
            if (pooledObjects.ContainsKey(NodeId))
            {
                foreach (var obj in pooledObjects[NodeId])
                {
                    if (Application.isPlaying)
                    {
                        Destroy(obj);
                    }
                    else
                    {
                        DestroyImmediate(obj);
                    }
                }
                pooledObjects[NodeId].Clear();
            }
        }
		
        public override GameObject AddGameObject(GameObjectDungeonThemeItem gameObjectProp, Matrix4x4 transform, IDungeonSceneObjectInstantiator objectInstantiator)
        {
			if (gameObjectProp == null) return null;
			var MeshTemplate = gameObjectProp.Template;
			string NodeId = gameObjectProp.NodeId;

			if (MeshTemplate == null) {
                return null;
			}
			
			// If we are in 2D mode, then flip the YZ axis
			{
				var mode2D = false;
				if (config != null) {
					mode2D = config.IsMode2D();
				}
				if (mode2D) {
					var position = Matrix.GetTranslation(ref transform);
					FlipSpritePosition(ref position);
					Matrix.SetTranslation(ref transform, position);
				}
			}

			GameObject item = null;
			// Try to reuse an object from the pool
			if (pooledObjects.ContainsKey (NodeId) && pooledObjects [NodeId].Count > 0) {
				item = pooledObjects [NodeId].Dequeue ();
                if (item != null)
                {
                    SetTransform(item.transform, transform);
                }
            } 

            if (item == null) { 
				// Pool is exhausted for this object
				item = BuildGameObject(gameObjectProp, transform, objectInstantiator);
            }
            
            SetStaticState(item, gameObjectProp.StaticState);

            return item;
		}

        void SetStaticState(GameObject item, DungeonThemeItemStaticMode staticState)
        {
	        if (Application.isPlaying)
	        {
		        // Changing static state at runtime has no affect
		        return;
	        }
	        if (staticState == DungeonThemeItemStaticMode.Unchanged) return;
	        if (item == null) return;

	        var isStatic = staticState == DungeonThemeItemStaticMode.ForceStatic;
	        
	        item.isStatic = isStatic;
	        if (isStatic)
	        {
		        RecursivelySetStatic(item.transform);
	        }
        }

		public override GameObject AddGameObjectFromArray(GameObjectArrayDungeonThemeItem gameObjectArrayProp, int index, Matrix4x4 transform, IDungeonSceneObjectInstantiator objectInstantiator)
		{
			if (gameObjectArrayProp == null) return null;
			string NodeId = gameObjectArrayProp.NodeId + "_" + index.ToString();

			// If we are in 2D mode, then flip the YZ axis
			{
				var mode2D = false;
				if (config != null) {
					mode2D = config.IsMode2D();
				}
				if (mode2D) {
					var position = Matrix.GetTranslation(ref transform);
					FlipSpritePosition(ref position);
					Matrix.SetTranslation(ref transform, position);
				}
			}

			GameObject item = null;
			// Try to reuse an object from the pool
			if (pooledObjects.ContainsKey (NodeId) && pooledObjects [NodeId].Count > 0) {
				item = pooledObjects [NodeId].Dequeue ();
				SetTransform (item.transform, transform);
			} else {
				// Pool is exhausted for this object
				item = BuildGameObjectFromArray(gameObjectArrayProp, index, transform, objectInstantiator);
			}

			SetStaticState(item, gameObjectArrayProp.StaticState);
			
			return item;
		}


        void RecursivelySetStatic(Transform trans)
        {
            var obj = trans.gameObject;
            obj.isStatic = true;
            for (int i = 0; i < trans.childCount; i++)
            {
                var child = trans.GetChild(i);
                RecursivelySetStatic(child);
            }
        }

	}
}
