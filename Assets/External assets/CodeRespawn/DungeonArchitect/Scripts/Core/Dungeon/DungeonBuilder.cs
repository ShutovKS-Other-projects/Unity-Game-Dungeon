//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect
{

    /// <summary>
    /// Builds the layout of the dungeon and emits markers around the layout
    /// Implement this class to create your own builder
    /// </summary>
	[ExecuteInEditMode]
    public abstract class DungeonBuilder : MonoBehaviour
    {
        protected DungeonConfig config;
        protected PMRandom nrandom;
        protected PMRandom random;
        protected DungeonModel model;
        protected LevelMarkerList markers = new LevelMarkerList();
        protected Blackboard blackboard = new Blackboard();

        public bool asyncBuild = false;
        public long maxBuildTimePerFrame = 32;
        public Transform asyncBuildStartPosition;

        private bool isLayoutBuilt = false;
        public bool IsLayoutBuilt
        {
            get
            {
                {
                    // Hot code reload sometimes invalidates the model.  
                    if (random == null) return false;
                }
                return isLayoutBuilt;
            }
        }

        public LevelMarkerList Markers
        {
            get { return markers; }
        }

        public DungeonModel Model
        {
            get { return model; }
        }

        public Blackboard Blackboard
        {
            get { return blackboard; }
        }

        /// <summary>
        /// Builds the dungeon layout
        /// </summary>
        /// <param name="config">The builder configuration</param>
        /// <param name="model">The dungeon model that the builder will populate</param>
        public virtual void BuildDungeon(DungeonConfig config, DungeonModel model)
        {
            this.config = config;
            this.model = model;

            nrandom = new PMRandom(config.Seed);
            random = new PMRandom(config.Seed);

            markers = CreateMarkerListObject(config);

            isLayoutBuilt = true;
        }
        
        protected virtual LevelMarkerList CreateMarkerListObject(DungeonConfig config)
        {
            return new SpatialPartionedLevelMarkerList(8);
        }

		public virtual void OnDestroyed() {
            ClearSockets();
            isLayoutBuilt = false;
        }


        public virtual bool IsThemingSupported() { return true; }

        public virtual bool DestroyDungeonOnRebuild() { return false; }
        
        // This is called by the builders that do not support theming
        public virtual void BuildNonThemedDungeon(DungeonSceneProvider sceneProvider, IDungeonSceneObjectInstantiator objectInstantiator) { }
        
        public virtual void DebugDraw()
        {
        }

        public virtual void DebugDrawGizmos()
        {
        }

        protected void ClearSockets()
        {
            markers.Clear();
        }

        /// <summary>
        /// Emit markers defined by this builder
        /// </summary>
        public virtual void EmitMarkers()
        {
            ClearSockets();
        }

        /// <summary>
        /// Emit markers defined by the user (implementation of DungeonMarkerEmitter)
        /// </summary>
        public void EmitCustomMarkers()
        {
            var emitters = GetComponents<DungeonMarkerEmitter>();
            foreach (var emitter in emitters)
            {
                emitter.EmitMarkers(this);
            }
        }

        public PropSocket EmitMarker(string SocketType, Matrix4x4 transform, IntVector gridPosition, int cellId)
        {
            return EmitMarker(SocketType, transform, gridPosition, cellId, null);
        }

        public PropSocket EmitMarker(string SocketType, Matrix4x4 transform, IntVector gridPosition, int cellId, object metadata)
        {
            return markers.EmitMarker(SocketType, transform, gridPosition, cellId, metadata);
        }

        public void EmitMarker(string SocketType, Matrix4x4 _transform, int count, Vector3 InterOffset, IntVector gridPosition, int cellId, Vector3 LogicalToWorldScale)
        {
            EmitMarker(SocketType, _transform, count, InterOffset, gridPosition, cellId, LogicalToWorldScale, null);
        }

        public void EmitMarker(string SocketType, Matrix4x4 _transform, int count, Vector3 InterOffset, IntVector gridPosition, int cellId, Vector3 LogicalToWorldScale, object metadata)
        {
            markers.EmitMarker(SocketType, _transform, count, InterOffset, gridPosition, cellId, LogicalToWorldScale, metadata);
        }

        /// <summary>
        /// Implementations should override this so that the new logical scale and position is set based on the volume's transformation
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="newPositionOnGrid"></param>
        /// <param name="newSizeOnGrid"></param>
        public virtual void OnVolumePositionModified(Volume volume, out IntVector newPositionOnGrid, out IntVector newSizeOnGrid)
        {
            newPositionOnGrid = MathUtils.ToIntVector(volume.transform.position);
            newSizeOnGrid = MathUtils.ToIntVector(volume.transform.localScale);
        }

        protected void ProcessMarkerOverrideVolumes()
        {
			var dungeon = GetComponent<Dungeon>();
            // Process the theme override volumes
            var replacementVolumes = GameObject.FindObjectsOfType<MarkerReplaceVolume>();
            foreach (var replacementVolume in replacementVolumes)
            {
				if (replacementVolume.dungeon == dungeon) {
					// Fill up the prop sockets with the defined mesh data 
					for (int i = 0; i < markers.Count; i++) {
						PropSocket socket = markers[i];
						var socketPosition = Matrix.GetTranslation (ref socket.Transform);
						if (replacementVolume.GetBounds ().Contains (socketPosition)) {
							foreach (var replacementEntry in replacementVolume.replacements) {
								if (socket.SocketType == replacementEntry.fromMarker) {
									socket.SocketType = replacementEntry.toMarker;
								}
							}
						}
					}
				}
            }
        }
    }
}
