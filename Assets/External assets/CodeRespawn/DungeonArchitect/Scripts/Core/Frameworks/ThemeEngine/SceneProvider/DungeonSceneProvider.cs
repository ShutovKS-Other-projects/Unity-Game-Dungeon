//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Utils;
using DungeonArchitect.Graphs;
using DungeonArchitect.Themeing;

namespace DungeonArchitect
{
    /// <summary>
    /// A scene provider instantiates game objects into the scene.  
    /// Implementations can customize the instantiation process if needed (e.g. object pooling etc)
    /// </summary>
	public class DungeonSceneProvider : MonoBehaviour {
        /// <summary>
        /// Called when build is started
        /// </summary>
        public virtual void OnDungeonBuildStart()
        {
            Initialize();
        }

        /// <summary>
        /// Called after build has ended
        /// </summary>
		public virtual void OnDungeonBuildStop() {}

        /// <summary>
        /// Request the creation of a game object
        /// </summary>
        /// <param name="gameObjectProp">The template to use for instantiation</param>
        /// <param name="transform">The transform of the instantiated game object</param>
		public virtual GameObject AddGameObject(GameObjectDungeonThemeItem gameObjectProp, Matrix4x4 transform, IDungeonSceneObjectInstantiator objectInstantiator) { return null; }

		/// <summary>
		/// Requests the creation of a game object from the provided list
		/// </summary>
		/// <returns>The game object from array.</returns>
		/// <param name="gameObjectArrayProp">Game object array property.</param>
		/// <param name="index">Index.</param>
		/// <param name="transform">Transform.</param>
		public virtual GameObject AddGameObjectFromArray(GameObjectArrayDungeonThemeItem gameObjectArrayProp, int index, Matrix4x4 transform, IDungeonSceneObjectInstantiator objectInstantiator) { return null; }

        /// <summary>
        /// Request the creation of a sprite object
        /// </summary>
        /// <param name="spriteProp">The sprite game object template reference</param>
        /// <param name="transform">The transform of the prop</param>
		public virtual GameObject AddSprite(SpriteDungeonThemeItem spriteProp, Matrix4x4 transform, IDungeonSceneObjectInstantiator objectInstantiator) { return null; }
        //public virtual void AddLight(GameObject Template, Matrix4x4 transform, string NodeId) {}

        /// <summary>
        /// Dungeon config used by the builder
        /// </summary>
		protected DungeonConfig config;

        /// <summary>
        /// The owning dungeon actor reference
        /// </summary>
        protected Dungeon dungeon;

        /// <summary>
        /// The parent for all spawned game objects.  Assign this to create all spawned objects
        /// underneath it to avoid cluttering up the hierarchy
        /// </summary>
		public GameObject itemParent;

		void Awake() {
            Initialize();
		}

        protected void Initialize()
        {
            config = GetComponent<DungeonConfig>();
            dungeon = GetComponent<Dungeon>();

            if (itemParent != null && !itemParent.isStatic)
            {
                // The parent should be static for performance reasons, or all child generated items would be considered as non-static (? TODO: confirm)
                Debug.LogWarning("Dungeon Scene Items parent is not marked static (" + itemParent.name + ").  Please mark as static to improve performance");
            }
        }

		protected GameObject BuildGameObject(GameObjectDungeonThemeItem gameObjectProp, Matrix4x4 transform, IDungeonSceneObjectInstantiator objectInstantiator) {
			return BuildGameObject (gameObjectProp.Template, gameObjectProp.NodeId, gameObjectProp.affectsNavigation, transform, gameObjectProp.externallyManaged, objectInstantiator);
		}

		protected GameObject BuildGameObjectFromArray(GameObjectArrayDungeonThemeItem gameObjectArrayProp, int index, Matrix4x4 transform, IDungeonSceneObjectInstantiator objectInstantiator) {
			if (index < 0 || index >= gameObjectArrayProp.Templates.Length) {
				// Invalid index
				return null;
			}
            GameObject template = gameObjectArrayProp.Templates[index];
            if (template == null)
            {
                return null;
            }

			return BuildGameObject(template, gameObjectArrayProp.NodeId, gameObjectArrayProp.affectsNavigation, transform, gameObjectArrayProp.externallyManaged, objectInstantiator);
		}

		protected GameObject BuildGameObject(GameObject template, string nodeId, bool affectsNavigation, Matrix4x4 transform, bool externallyManaged, IDungeonSceneObjectInstantiator objectInstantiator) {
			Matrix.DecomposeMatrix(ref transform, out _position, out _rotation, out _scale);
			
			var MeshTemplate = template;
			string NodeId = nodeId;

            var parentTransform = (itemParent != null) ? itemParent.transform : null;
            var gameObj = objectInstantiator.Instantiate(MeshTemplate, _position, _rotation, _scale, parentTransform);
            
			var data = gameObj.AddComponent<DungeonSceneProviderData> ();
			data.NodeId = NodeId;
			data.dungeon = dungeon;
			data.affectsNavigation = affectsNavigation;
			data.externallyManaged = externallyManaged;
			
			return gameObj;
		}

		protected void FlipSpriteTransform(ref Matrix4x4 transform, Sprite sprite) {
			Matrix.DecomposeMatrix(ref transform, out _position, out _rotation, out _scale);

			FlipSpritePosition(ref _position);

			//var basePixelScale = 1.0f / sprite.pixelsPerUnit;	// TODO: Verify this
			//var spriteScale = new Vector3(sprite.rect.width * basePixelScale, sprite.rect.height * basePixelScale, 1);
			//_scale = Vector3.Scale(_scale, spriteScale);

			// flip the rotation
			var angles = _rotation.eulerAngles;
			var t = angles.z;
			angles.z = -angles.y;
			angles.y = t;
			_rotation = Quaternion.Euler(angles);

			transform.SetTRS(_position, _rotation, _scale);
		}
		
		protected void FlipSpritePosition(ref Matrix4x4 transform) {
			var position = Matrix.GetTranslation(ref transform);
			
			FlipSpritePosition(ref _position);
			
			Matrix.SetTranslation(ref transform, position);
		}

		protected void FlipSpritePosition(ref Vector3 position) {
			var z = position.z;
			position.z = position.y;
			position.y = z;
		}

        public virtual void InvalidateNodeCache(string NodeId) {  }

		protected GameObject BuildSpriteObject(SpriteDungeonThemeItem spriteData, Matrix4x4 transform, string NodeId) {
			if (spriteData.sprite == null) return null;
			var gameObj = new GameObject(spriteData.sprite.name);
			
			// Setup the sprite
			var spriteRenderer = gameObj.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = spriteData.sprite;
			spriteRenderer.color = spriteData.color;
			spriteRenderer.sortingOrder = spriteData.orderInLayer;
			
			if (spriteData.materialOverride != null) {
				spriteRenderer.material = spriteData.materialOverride;
			}
			if (spriteData.sortingLayerName != null && spriteData.sortingLayerName.Length > 0) {
				spriteRenderer.sortingLayerName = spriteData.sortingLayerName;
			}

			// Setup the sprite collision
			var collisionType = spriteData.collisionType;
			if (collisionType != DungeonSpriteCollisionType.None) {
				Vector2 baseScaleMultiplier = Vector2.one;
				var sprite = spriteData.sprite;
				var pixelsPerUnit = sprite.pixelsPerUnit;
				baseScaleMultiplier.x = sprite.rect.width / pixelsPerUnit;
				baseScaleMultiplier.y = sprite.rect.height / pixelsPerUnit;

				Collider2D collider = null;
				if (collisionType == DungeonSpriteCollisionType.Box) {
					var boxCollider = gameObj.AddComponent<BoxCollider2D>();
					boxCollider.size = Vector3.Scale(spriteData.physicsSize, baseScaleMultiplier);
					collider = boxCollider;
				}
				else if (collisionType == DungeonSpriteCollisionType.Circle) {
					var circleCollider = gameObj.AddComponent<CircleCollider2D>();
					circleCollider.radius = spriteData.physicsRadius * baseScaleMultiplier.x;
					collider = circleCollider;
				}
				else if (collisionType == DungeonSpriteCollisionType.Polygon) {
					collider = gameObj.AddComponent<PolygonCollider2D>();
				}

				if (collider != null) {
					collider.sharedMaterial = spriteData.physicsMaterial;
					collider.offset = Vector3.Scale(spriteData.physicsOffset, baseScaleMultiplier);
				}
			}


			// Set the transform
			Matrix.DecomposeMatrix(ref transform, out _position, out _rotation, out _scale);
			gameObj.transform.position = _position;
			gameObj.transform.rotation = _rotation;
			gameObj.transform.localScale = _scale;
			
			// Setup dungeon related parameters
			if (itemParent != null) {
				gameObj.transform.SetParent(itemParent.transform, true);
			}
			
			var data = gameObj.AddComponent<DungeonSceneProviderData> ();
			data.NodeId = NodeId;
            data.dungeon = dungeon;
            data.affectsNavigation = spriteData.affectsNavigation;
			
			return gameObj;
		}
		
		protected Vector3 _position = new Vector3();
		protected Quaternion _rotation = new Quaternion();
		protected Vector3 _scale = new Vector3();
		protected void SetTransform(Transform transform, Matrix4x4 matrix) {
			Matrix.DecomposeMatrix(ref matrix, out _position, out _rotation, out _scale);
			transform.position = _position;
			transform.rotation = _rotation;
			transform.localScale = _scale;
		}
	}
}