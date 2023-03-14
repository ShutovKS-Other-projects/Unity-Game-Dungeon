//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Grammar;
using UnityEngine.Serialization;

namespace DungeonArchitect.Builders.Snap
{
    [System.Serializable]
    public class SnapModuleEntry
    {
        [SerializeField]
        public GameObject module = null;

        [SerializeField]
        public string category = "";
    }

    public class SnapConfig : DungeonConfig
    {
        /// <summary>
        /// Specify the list of modules here.  These modules would be stitched together to create your level
        /// </summary>
        [Tooltip(@"Specify the list of modules here.  These modules would be stitched together to create your level")]
        public SnapModuleEntry[] Modules;

        [FormerlySerializedAs("dungeonFlow")] [Tooltip(@"Dungeon flow assets allow you to design layouts for your dungeons using graph grammar")]
        public SnapFlowAsset snapFlow;

        /// <summary>
        /// Grammar production rule graphs can be built using user specified scripts.  Check this if they
        /// should be re-run (and hence regenerate the graph) while processing the graph grammar
        /// </summary>
        [Tooltip(@"Grammar production rule graphs can be built using user specified scripts.  Check this if they should be re-run (and hence regenerate the graph) while processing the graph grammar")]
        public bool runGraphGenerationScripts = false;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip(@"")]
        public bool RotateModulesToFit = true;

        /// <summary>
        /// When modules are stitched together, the builder makes sure they do not overlap.  This parameter is used to 
        /// control the tolerance level.  If set to 0, even the slightest overlap with a nearby module would not create an adjacent module
        /// Leaving to a small number like 100, would tolerate an overlap with nearby module by 100 unreal units.
        /// Adjust this depending on your art asset
        /// </summary>
        [Tooltip(@"When modules are stitched together, the builder makes sure they do not overlap.  This parameter is used to 
	 control the tolerance level.  If set to 0, even the slightest overlap with a nearby module would not create an adjacent module
	 Leaving to a small number like 100, would tolerate an overlap with nearby module by 100 unreal units.
	 Adjust this depending on your art asset")]
        public float CollisionTestContraction = 1;

        [Tooltip(@"When two modules connect, we'll have two copies of the door from each room.   Enable this flag to hide one of the doors.
	 Sometimes, you might not want to do this (e.g. in a 2D tilemap)")]
        public bool hideDuplicateDoors = true;
        
        /// <summary>
        /// Sometimes, the search space is too large (with billions of possibilities) and if a valid path cannot be easily found
        /// (e.g. due to existing occluded geometry) the search would take too long.  This value makes sure the build doesn't
        /// hang and bails out early with the best result it has found till that point.
        /// Increase the value to have better quality result in those cases. Decrease if you notice the build taking too long
        /// or if build speed is a priority (e.g. if you are building during runtime).   A good value is ~1000000
        /// </summary>
        [Tooltip(@"Sometimes, the search space is too large (with billions of possibilities) and if a valid path cannot be easily found
	(e.g. due to existing occluded geometry) the search would take too long.  This value makes sure the build doesn't
	hang and bails out early with the best result it has found till that point.
	Increase the value to have better quality result in those cases. Decrease if you notice the build taking too long
	or if build speed is a priority (e.g. if you are building during runtime).   A good value is ~1000000")]
        public int MaxProcessingPower = 1000000;
        
    }
}

