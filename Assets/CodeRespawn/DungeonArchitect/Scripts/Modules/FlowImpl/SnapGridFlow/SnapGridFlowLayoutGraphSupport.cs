 //$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Flow.Impl.SnapGridFlow.Tasks;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow
{	
	struct NodeGroupSettings
	{
		public float Weight;
		public Vector3Int GroupSize;

		public SgfModuleDatabaseItem Module;
		public int ModuleAssemblyIdx;
		public string Category;
            
		public Vector3Int[] LocalSurfaceCoords;
		public Vector3Int[] LocalVolumeCoords;
	}

	/*
	internal struct SGFGroupConnectionInfo
	{
		public Vector3Int EdgeNodeCoord;
		public Vector3Int RemoteNodeCoord;
		public string Category;
	}
	*/
	
	internal class SGFNodeGroupUserData : IFlowDomainData
	{
		public SgfModuleDatabaseItem Module;
		public int ModuleAssemblyIdx;
		//public SGFGroupConnectionInfo[] ModuleAssemblyConnections;

		public IFlowDomainData Clone()
		{
			var clone = new SGFNodeGroupUserData()
			{
				Module = Module,
				ModuleAssemblyIdx = ModuleAssemblyIdx,
				//ModuleAssemblyConnections = new SGFGroupConnectionInfo[ModuleAssemblyConnections.Length]
			};
			//Array.Copy(ModuleAssemblyConnections, clone.ModuleAssemblyConnections, ModuleAssemblyConnections.Length);
			
			return clone;
		}
	}
	
    public class SnapFlowLayoutNodeGroupGenerator : FlowLayoutNodeGroupGenerator
    {
        private NodeGroupSettings[] groupSettings;
        //private static FLocalCoordBuilder coordBuilder = new FLocalCoordBuilder();
        private int minGroupSize = 1;
        private ISGFLayoutTaskPathBuilder pathingTask;
        
        public SnapFlowLayoutNodeGroupGenerator(SnapGridFlowModuleDatabase moduleDatabase, ISGFLayoutTaskPathBuilder pathingTask)
        {
	        this.pathingTask = pathingTask;
            if (moduleDatabase != null)
            {
	            float maxSelectionWeight = 0.0f;
	            foreach (var module in moduleDatabase.Modules)
	            {
		            maxSelectionWeight = Mathf.Max(maxSelectionWeight, module.SelectionWeight);
	            }

	            if (maxSelectionWeight == 0)
	            {
		            maxSelectionWeight = 1.0f;
	            }
	            
	            {
		            var settings = new List<NodeGroupSettings>();
		            foreach (var module in moduleDatabase.Modules)
		            {
			            for (int AsmIdx = 0; AsmIdx < module.RotatedAssemblies.Length; AsmIdx++)
			            {
				            var assembly = module.RotatedAssemblies[AsmIdx];
				            var groupSize = assembly.numChunks;
				            var setting = new NodeGroupSettings()
				            {
					            Weight = module.SelectionWeight,
					            GroupSize = groupSize,
					            Module = module,
					            ModuleAssemblyIdx = AsmIdx,
					            Category = module.Category
				            };
				            settings.Add(setting);
			            }

			            groupSettings = settings.ToArray();
		            }
	            }

	            if (groupSettings != null)
	            {
		            minGroupSize = int.MaxValue;
		            foreach (var setting in groupSettings)
		            {
			            var groupSize = setting.GroupSize.x * setting.GroupSize.y * setting.GroupSize.z;
			            minGroupSize = Mathf.Min(minGroupSize, groupSize);
		            }
	            }
	            else
	            {
		            groupSettings = new NodeGroupSettings[0];
	            }

	            /*
	            // Build the group weights
                var groupWeights = new Dictionary<Vector3Int, float>();
                {
	                var groupCounts = new Dictionary<Vector3Int, int>();
	                foreach (var module in moduleDatabase.Modules)
	                {
		                var chunkSizes = new List<Vector3Int>();
		                chunkSizes.Add(module.NumChunks);
		                if (module.allowRotation)
		                {
			                var rotatedNumChunks = new Vector3Int(module.NumChunks.z, module.NumChunks.y, module.NumChunks.x);
			                chunkSizes.Add(rotatedNumChunks);
		                }
		                foreach (var chunkSize in chunkSizes)
		                {
			                if (!groupWeights.ContainsKey(chunkSize))
			                {
				                groupWeights[chunkSize] = 0;
			                }

			                groupWeights[chunkSize] += module.SelectionWeight;

			                if (!groupCounts.ContainsKey(chunkSize))
			                {
				                groupCounts[chunkSize] = 0;
			                }

			                groupCounts[chunkSize] = groupCounts[chunkSize] + 1;
		                }
	                }

	                // Average out the weights
	                var keys = groupWeights.Keys.ToArray();
	                foreach (var key in keys)
	                {
		                var value = groupWeights[key];
		                var count = groupCounts[key];
		                groupWeights[key] = value / count;
	                }
                }

                var settingList = new List<NodeGroupSettings>();
                foreach (var entry in groupWeights) {
                    
                    var setting = new NodeGroupSettings()
                    {
                        Weight = entry.Value,
                        GroupSize = entry.Key
                    };
                    settingList.Add(setting);
                }

                groupSettings = settingList.ToArray();
                */
            }
            else
            {
                var setting = new NodeGroupSettings()
                {
                    Weight = 1,
                    GroupSize = new Vector3Int(1, 1, 1)
                };
                groupSettings = new NodeGroupSettings[] {setting};
            }
            
            for (int i = 0; i < groupSettings.Length; i++)
            {
	            var setting = groupSettings[i];
	            var assembly = setting.Module.RotatedAssemblies[setting.ModuleAssemblyIdx];
	            FLocalCoordBuilder.GetCoords(assembly, out groupSettings[i].LocalVolumeCoords, out groupSettings[i].LocalSurfaceCoords);
            }
        }

        public override int GetMinNodeGroupSize()
        {
            return minGroupSize;
        }

        
        public override FlowLayoutPathNodeGroup[] Generate(FlowLayoutGraphQuery graphQuery, FlowLayoutGraphNode currentNode, int pathIndex, int pathLength, System.Random random, HashSet<DungeonUID> visited)
        {
	        if (currentNode == null) {
		        return new FlowLayoutPathNodeGroup[0];
	        }

	        if (groupSettings.Length == 0) {
		        var nullGenerator = new NullFlowLayoutNodeGroupGenerator();
		        return nullGenerator.Generate(graphQuery, currentNode, pathIndex, pathLength, random, visited);
	        }

	        var allowedCategories = new HashSet<string>(pathingTask.GetCategoriesAtNode(pathIndex, pathLength));
	        
	        var filteredGroupSettings = new List<NodeGroupSettings>();
            foreach (var groupSetting in groupSettings)
            {
	            if (allowedCategories.Contains(groupSetting.Category))
	            {
		            filteredGroupSettings.Add(groupSetting);
	            }
            }

            var currentNodeCoord = MathUtils.RoundToVector3Int(currentNode.coord);
            var result = new List<FlowLayoutPathNodeGroup>();
	        foreach (var groupSetting in filteredGroupSettings)
            {
		        foreach (var localSurfaceCoord in groupSetting.LocalSurfaceCoords) {
			        bool valid = true;
			        var baseCoord = currentNodeCoord - localSurfaceCoord;
			        foreach (var localVolumeCoord in groupSetting.LocalVolumeCoords) {
				        var groupNodeCoord = baseCoord + localVolumeCoord;
                        var testNode = graphQuery.GetNodeObjAtCoord(groupNodeCoord);
 				        if (testNode == null || visited.Contains(testNode.nodeId) || testNode.active) {
					        valid = false;
					        break;
				        }
			        }

			        if (valid) {
				        // Add this group
                        var newGroup = new FlowLayoutPathNodeGroup();
				        newGroup.IsGroup = true;
				        newGroup.Weight = groupSetting.Weight;
				        foreach (var localVolumeCoord in groupSetting.LocalVolumeCoords) {
					        var nodeCoord = baseCoord + localVolumeCoord;
                            var groupNode = graphQuery.GetNodeObjAtCoord(nodeCoord);
                            if (groupNode != null)
                            {
                                newGroup.GroupNodes.Add(groupNode.nodeId);
                            }
                        }
				        foreach (var surfCoord in groupSetting.LocalSurfaceCoords) {
					        var nodeCoord = baseCoord + surfCoord;
                            var groupNode = graphQuery.GetNodeObjAtCoord(nodeCoord);
					        newGroup.GroupEdgeNodes.Add(groupNode.nodeId);
				        }

				        var userdata = new SGFNodeGroupUserData();
				        userdata.Module = groupSetting.Module;
				        userdata.ModuleAssemblyIdx = groupSetting.ModuleAssemblyIdx;
				        newGroup.userdata = userdata;
				        
                        result.Add(newGroup);
			        }
		        }
	        }

            return result.ToArray();
        }

        class FLocalCoordBuilder {
            public static void GetCoords(SgfModuleAssembly assembly, out Vector3Int[] outVolumeCoords, out Vector3Int[] outSurfaceCoords) {
	            var numChunks = assembly.numChunks;
	            List<Vector3Int> volumeCoords;

	            // Process the volume coords
	            if (_volumeCoordsMap.ContainsKey(numChunks))
                {
	                volumeCoords = _volumeCoordsMap[numChunks];
                }
	            else
	            {
		            volumeCoords = new List<Vector3Int>();
		            for (int dz = 0; dz < numChunks.z; dz++) {
			            for (int dy = 0; dy < numChunks.y; dy++) {
				            for (int dx = 0; dx < numChunks.x; dx++) {
					            volumeCoords.Add(new Vector3Int(dx, dy, dz));
				            }
			            }
		            }
		            _volumeCoordsMap.Add(numChunks, volumeCoords);		            
	            }

	            // Process the surface coords
	            var surfaceCoords = new HashSet<Vector3Int>();
	            {
		            // Process along the X-axis
		            {
		                for (int x = 0; x < numChunks.x; x++)
		                {
		                    for (int y = 0; y < numChunks.y; y++)
		                    {
			                    var cellFront = assembly.front.Get(x, y);
			                    var cellBack = assembly.back.Get(numChunks.x - 1 - x, y);
			                    if (cellFront.HasConnection()) surfaceCoords.Add(new Vector3Int(x, y, 0));
			                    if (cellBack.HasConnection()) surfaceCoords.Add(new Vector3Int(x, y, numChunks.z - 1));
		                    }
		                }
		            }

		            // Process along the Z-axis
		            {
		                for (int z = 0; z < numChunks.z; z++)
		                {
		                    for (int y = 0; y < numChunks.y; y++)
		                    {
			                    var cellRight = assembly.right.Get(numChunks.z - 1 - z, y);
			                    var cellLeft = assembly.left.Get(z, y);
			                    if (cellRight.HasConnection()) surfaceCoords.Add(new Vector3Int(0, y, z));
			                    if (cellLeft.HasConnection()) surfaceCoords.Add(new Vector3Int(numChunks.x - 1, y, z));
		                    }
		                }
		            }

		            // Process along the Y-axis
		            {
		                for (int x = 0; x < numChunks.x; x++)
		                {
		                    for (int z = 0; z < numChunks.z; z++)
		                    {
			                    var cellDown = assembly.down.Get(x, z);
			                    var cellTop = assembly.top.Get(x, z);
			                    if (cellDown.HasConnection()) surfaceCoords.Add(new Vector3Int(x, 0, z));
			                    if (cellTop.HasConnection()) surfaceCoords.Add(new Vector3Int(x, numChunks.y - 1, z));
		                    }
		                }
		            }
	            }

	            outVolumeCoords = volumeCoords.ToArray();
                outSurfaceCoords = surfaceCoords.ToArray();
            }
	
            private static Dictionary<Vector3Int, List<Vector3Int>> _volumeCoordsMap = new Dictionary<Vector3Int, List<Vector3Int>>();
        };
    }

    public class SnapFlowLayoutGraphConstraints : IFlowLayoutGraphConstraints
    {
        private SnapGridFlowModuleDatabase moduleDatabase;
        private ISGFLayoutTaskPathBuilder pathingTask; 

        public SnapFlowLayoutGraphConstraints(SnapGridFlowModuleDatabase moduleDatabase, ISGFLayoutTaskPathBuilder pathingTask)
        {
            this.moduleDatabase = moduleDatabase;
            this.pathingTask = pathingTask;
        }
        
        public bool IsValid(FlowLayoutGraphQuery graphQuery, FlowLayoutGraphNode node, FlowLayoutGraphNode[] incomingNodes)
        {
            var graph = graphQuery.Graph;
            if (graph == null) return false;
            Debug.Assert(node != null && node.pathIndex != -1);


            var allIncomingNodes = new HashSet<FlowLayoutGraphNode>(incomingNodes);

            foreach (var link in graph.Links)
            {
	            if (link.state.type == FlowLayoutGraphLinkType.Unconnected)
	            {
		            continue;
	            }

	            if (link.destination == node.nodeId)
	            {
		            var sourceNode = link.sourceSubNode.IsValid() 
							? graphQuery.GetSubNode(link.sourceSubNode)
							: graphQuery.GetNode(link.source);
		            
		            Debug.Assert(sourceNode != null, "Invalid source node");
		            allIncomingNodes.Add(sourceNode);
	            }
	            if (link.source == node.nodeId)
	            {
		            var destNode = link.destinationSubNode.IsValid()
						? graphQuery.GetSubNode(link.destinationSubNode)
						: graphQuery.GetNode(link.destination);
		            
		            Debug.Assert(destNode != null, "Invalid dest node");
		            allIncomingNodes.Add(destNode);
	            }
            }

            FlowLayoutPathNodeGroup group;
            FFAGConstraintsLink[] constraintLinks;
            BuildNodeGroup(graphQuery, node, allIncomingNodes.ToArray(), out group, out constraintLinks);
            
            var nodeSnapData = node.GetDomainData<FlowLayoutNodeSnapDomainData>();
            if (nodeSnapData == null || nodeSnapData.Categories.Length == 0) {
                return false;
            }

            return IsValid(graphQuery, group, constraintLinks, nodeSnapData.Categories);
        }

        public bool IsValid(FlowLayoutGraphQuery graphQuery, FlowLayoutPathNodeGroup group, int pathIndex, int pathLength, FFAGConstraintsLink[] incomingNodes)
        {
            var allowedCategories = pathingTask.GetCategoriesAtNode(pathIndex, pathLength);
            return IsValid(graphQuery, group, incomingNodes.ToArray(), allowedCategories);
        }

        private bool IsValid(FlowLayoutGraphQuery graphQuery, FlowLayoutPathNodeGroup group, FFAGConstraintsLink[] incomingNodes, string[] allowedCategories)	// TODO: Get rid of allowedCategory
        {
            if (group == null || group.GroupEdgeNodes.Count == 0 || group.GroupNodes.Count == 0) return false;


            SGFNodeGroupUserData sgfUserData = group.userdata as SGFNodeGroupUserData;
            Debug.Assert(sgfUserData != null, "Invalid SGF user group data");
            
            var registeredAssembly = sgfUserData.Module.RotatedAssemblies[sgfUserData.ModuleAssemblyIdx];
            
            // Build the input node assembly
            SgfModuleAssembly assembly;
            SGFModuleAssemblyBuilder.Build(graphQuery, group, incomingNodes, out assembly);
            
            SgfModuleAssemblySideCell[] doorIndices;
            if (registeredAssembly.CanFit(assembly, out doorIndices)) {
	            return true;
            }
        
            return false;
        }
        
        public static void BuildNodeGroup(FlowLayoutGraphQuery graphQuery, FlowLayoutGraphNode node, FlowLayoutGraphNode[] incomingNodes, 
                out FlowLayoutPathNodeGroup outGroup, out FFAGConstraintsLink[] outConstraintLinks)
        {
	        var graph = graphQuery.Graph;
	        outGroup = new FlowLayoutPathNodeGroup();
	        if (graph == null)
	        {
		        outGroup = null;
		        outConstraintLinks = new FFAGConstraintsLink[0];
		        return;
	        }
	        

			Vector3Int minCoord = Vector3Int.zero;
			Vector3Int maxCoord = Vector3Int.zero;
			if (node.MergedCompositeNodes.Count <= 1) {
				outGroup.IsGroup = false;
				outGroup.GroupNodes.Add(node.nodeId);
				outGroup.GroupEdgeNodes.Add(node.nodeId);
				minCoord = maxCoord = MathUtils.RoundToVector3Int(node.coord);
			}
			else {
				outGroup.IsGroup = true;
				var minCoordF = node.MergedCompositeNodes[0].coord;
				var maxCoordF = minCoordF;

				foreach (var subNode in node.MergedCompositeNodes) {
					minCoordF = MathUtils.ComponentMin(minCoordF, subNode.coord);
					maxCoordF = MathUtils.ComponentMax(maxCoordF, subNode.coord);
					outGroup.GroupNodes.Add(subNode.nodeId);
				}
				minCoord = MathUtils.RoundToVector3Int(minCoordF);
				maxCoord = MathUtils.RoundToVector3Int(maxCoordF);

				foreach (var subNode in node.MergedCompositeNodes) {
					var coord = MathUtils.RoundToVector3Int(subNode.coord);
					if (coord.x == minCoord.x || coord.y == minCoord.y || coord.z == minCoord.z ||
						coord.x == maxCoord.x || coord.y == maxCoord.y || coord.z == maxCoord.z) {
						outGroup.GroupEdgeNodes.Add(subNode.nodeId);
					}
				}
			}
			
			var sgfUserData = node.GetDomainData<SGFNodeGroupUserData>();
			outGroup.userdata = sgfUserData;
			Debug.Assert(sgfUserData.Module != null, "SGF node group doesn't have correct module user data assigned");

			var constraintLinkList = new List<FFAGConstraintsLink>();
			
			foreach (var link in graph.Links) {
				if (link.state.type == FlowLayoutGraphLinkType.Unconnected) continue;

				var source = link.sourceSubNode.IsValid() ? link.sourceSubNode : link.source;
				var destination = link.destinationSubNode.IsValid() ? link.destinationSubNode : link.destination;

				var bHostsSource = outGroup.GroupNodes.Contains(source);
				var bHostsDest = outGroup.GroupNodes.Contains(destination);
				if (!bHostsSource && !bHostsDest) continue;
				if (bHostsSource && bHostsDest) continue;

				if (bHostsSource) {
					if (outGroup.GroupEdgeNodes.Contains(source)) {
						var sourceNode = graphQuery.GetNode(source);
						if (sourceNode == null) sourceNode = graphQuery.GetSubNode(source);
						var destinationNode = graphQuery.GetNode(destination);
						if (destinationNode == null) destinationNode = graphQuery.GetSubNode(destination);
						if (sourceNode != null && destinationNode != null) {
							constraintLinkList.Add(new FFAGConstraintsLink(sourceNode, destinationNode, link));
						}
					}
				}
				else if (bHostsDest) {
					if (outGroup.GroupEdgeNodes.Contains(destination)) {
						var sourceNode = graphQuery.GetNode(source);
						if (sourceNode == null) sourceNode = graphQuery.GetSubNode(source);
						var destinationNode = graphQuery.GetNode(destination);
						if (destinationNode == null) destinationNode = graphQuery.GetSubNode(destination);
						if (sourceNode != null && destinationNode != null) {
							constraintLinkList.Add(new FFAGConstraintsLink(destinationNode, sourceNode, link));
						}
					}
				}
			}

			var nodeByCoords = new Dictionary<Vector3Int, FlowLayoutGraphNode>();
			foreach (var graphNode in graph.Nodes) {
				if (graphNode.MergedCompositeNodes.Count > 0) {
					foreach (var subNode in graphNode.MergedCompositeNodes) {
						var coord = MathUtils.RoundToVector3Int(subNode.coord);
						nodeByCoords[coord] = subNode;
					}
				}
				else {
					var coord = MathUtils.RoundToVector3Int(graphNode.coord);
					nodeByCoords[coord] = graphNode;
				}
			}

			foreach (var incomingNode in incomingNodes) {
				if (incomingNode == null) continue;
				var innerCoord = MathUtils.RoundToVector3Int(incomingNode.coord);
				innerCoord.x = Mathf.Clamp(innerCoord.x, minCoord.x, maxCoord.x);
				innerCoord.y = Mathf.Clamp(innerCoord.y, minCoord.y, maxCoord.y);
				innerCoord.z = Mathf.Clamp(innerCoord.z, minCoord.z, maxCoord.z);
				if (nodeByCoords.ContainsKey(innerCoord))
				{
					var innerNode = nodeByCoords[innerCoord];
					var innerNodeLink = graphQuery.GetConnectedLink(incomingNode.nodeId, innerNode.nodeId);
					Debug.Assert(innerNodeLink != null, "BuildNodeGroup: Cannot find link");
					constraintLinkList.Add(new FFAGConstraintsLink(innerNode, incomingNode, innerNodeLink));
				}
			}

			outConstraintLinks = constraintLinkList.ToArray();
        }
    }
    
    public class SnapFlowLayoutNodeCreationConstraint : IFlowLayoutNodeCreationConstraint
    {
        public bool CanCreateNodeAt(FlowLayoutGraphNode node, int totalPathLength, int currentPathPosition)
        {
            return true;
        }
    }

    public class FlowLayoutNodeSnapDomainData : IFlowDomainData
    {
        public string[] Categories = new string[0];
        public IFlowDomainData Clone()
        {
            var clone = new FlowLayoutNodeSnapDomainData();
            clone.Categories = new string[Categories.Length];
            Array.Copy(Categories, clone.Categories, Categories.Length);
            return clone;
        }
    }
    
    
    public class SnapGridFlowDomainExtension : IFlowDomainExtension
    {
	    public SnapGridFlowModuleDatabase ModuleDatabase;
    }
}