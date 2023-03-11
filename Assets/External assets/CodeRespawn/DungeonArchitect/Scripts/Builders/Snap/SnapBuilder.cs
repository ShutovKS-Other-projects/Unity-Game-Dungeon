//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Snap.Impl;
using DungeonArchitect.Frameworks.Snap;
using DungeonArchitect.Grammar;
using DungeonArchitect.Themeing;
using RNode = DungeonArchitect.RuntimeGraphs.RuntimeGraphNode<DungeonArchitect.Grammar.GrammarRuntimeGraphNodeData>;

namespace DungeonArchitect.Builders.Snap
{

    public class SnapBuilder : DungeonBuilder
    {
        SnapConfig snapConfig;
        SnapModel snapModel;
        
        new System.Random random;

        /// <summary>
        /// Builds the dungeon layout.  In this method, you should build your dungeon layout and save it in your model file
        /// No markers should be emitted here.   (EmitMarkers function will be called later by the engine to do that)
        /// </summary>
        /// <param name="config">The builder configuration</param>
        /// <param name="model">The dungeon model that the builder will populate</param>
        public override void BuildDungeon(DungeonConfig config, DungeonModel model)
        {
            base.BuildDungeon(config, model);

            markers.Clear();
        }

        /// <summary>
        /// Override the builder's emit marker function to emit our own markers based on the layout that we built
        /// You should emit your markers based on the layout you have saved in the model generated previously
        /// When the user is designing the theme interactively, this function will be called whenever the graph state changes,
        /// so the theme engine can populate the scene (BuildDungeon will not be called if there is no need to rebuild the layout again)
        /// </summary>
        public override void EmitMarkers()
        {
            base.EmitMarkers();

        }
        
        public override bool IsThemingSupported() { return false; }

        // This is called by the builders that do not support theming
        public override void BuildNonThemedDungeon(DungeonSceneProvider sceneProvider, IDungeonSceneObjectInstantiator objectInstantiator) {
            random = new System.Random((int)config.Seed);
            markers.Clear();

            // We know that the dungeon prefab would have the appropriate config and models attached to it
            // Cast and save it for future reference
            snapConfig = config as SnapConfig;
            snapModel = model as SnapModel;

            if (snapConfig == null)
            {
                Debug.LogError("No snap config script found in dungeon game object");
                return;
            }

            if (snapModel == null)
            {
                Debug.LogError("No snap model script found in dungeon game object");
                return;
            }

            if (snapConfig.snapFlow == null)
            {
                Debug.LogError("No dungeon flow asset defined in the config");
                return;
            }

            // Generate the module info list
            var ModuleInfos = new List<ModuleInfo>();
            {
                var RegisteredModules = new HashSet<SnapModuleEntry>(snapConfig.Modules);

                foreach (var RegisteredModule in RegisteredModules)
                {
                    if (RegisteredModule.module != null)
                    {
                        var moduleInfo = GenerateModuleInfo(RegisteredModule);
                        ModuleInfos.Add(moduleInfo);
                    }
                }
            }

            var dungeonBasePosition = (transform != null) ? transform.position : Vector3.zero;

            var StartNode = new ModuleGrowthNode();
            StartNode.IncomingModuleDoorIndex = -1;
            StartNode.startNode = true;
            StartNode.ModuleTransform = Matrix4x4.TRS(dungeonBasePosition, Quaternion.identity, Vector3.one);

            var OccupiedBounds = new HashSet<Bounds>();

            var LayoutBuildState = new SnapLayoutBuildState();
            LayoutBuildState.ModuleInfoList = ModuleInfos;

            // Build the dungeon flow graph

            var processorSettings = new GraphGrammarProcessorSettings();
            processorSettings.seed = (int)config.Seed;
            processorSettings.runGraphGenerationScripts = snapConfig.runGraphGenerationScripts;
            var processor = new GraphGrammarProcessor(snapConfig.snapFlow, processorSettings);
            processor.Build();
            var levelGraph = processor.Grammar.ResultGraph;
            var startGraphNode = GrammarRuntimeGraphUtils.FindStartNode(levelGraph);

            // Build the main branch
            ModuleBuildNode BuildNode = BuildLayoutRecursive(StartNode, ref OccupiedBounds, startGraphNode, LayoutBuildState);

            snapModel.ResetModel();

            sceneProvider.OnDungeonBuildStart();

            // Spawn the modules and register them in the model
            {
                var spawnedModuleList = new List<SnapModuleInstance>();
                TraverseTree(BuildNode, delegate (ModuleBuildNode Node)
                {
                    // Spawn a module at this location
                    ModuleInfo moduleInfo = Node.Module;

                    var templateInfo = new GameObjectDungeonThemeItem();
                    templateInfo.Template = moduleInfo.ModuleTemplate.module;
                    //templateInfo.NodeId = moduleInfo.ModuleGuid.ToString();
                    templateInfo.NodeId = Node.ModuleInstanceID;
                    templateInfo.Offset = Matrix4x4.identity;
                    templateInfo.StaticState = DungeonThemeItemStaticMode.Unchanged;

                    Node.spawnedModule = sceneProvider.AddGameObject(templateInfo, Node.AttachmentConfig.AttachedModuleTransform, objectInstantiator);

                    // Register this in the model
                    var snapModule = new SnapModuleInstance();
                    snapModule.InstanceID = Node.ModuleInstanceID;
                    snapModule.WorldTransform = Node.AttachmentConfig.AttachedModuleTransform;
                    snapModule.WorldBounds = Node.AttachmentConfig.AttachedModuleWorldBounds;
                    spawnedModuleList.Add(snapModule);
                });
                snapModel.modules = spawnedModuleList.ToArray();
            }

            // Generate the list of connections
            {
                var connectionList = new List<SnapModuleConnection>();
                TraverseTree(BuildNode, delegate (ModuleBuildNode Node)
                {
                    if (Node.Parent != null)
                    {
                        var Connection = new SnapModuleConnection();
                        Connection.ModuleAInstanceID = Node.ModuleInstanceID;
                        Connection.DoorAIndex = Node.AttachmentConfig.AttachedModuleDoorIndex;

                        Connection.ModuleBInstanceID = Node.Parent.ModuleInstanceID;
                        Connection.DoorBIndex = Node.IncomingDoorIndex;

                        connectionList.Add(Connection);
                    }
                });
                snapModel.connections = connectionList.ToArray();
            }
            
            sceneProvider.OnDungeonBuildStop();

            FixupDoorStates(BuildNode);
        }

        T GetArrayEntry<T>(int index, T[] array) where T : class
        {
            if (index < 0 || index >= array.Length)
            {
                return null;
            }

            return array[index];
        }

        void FixupDoorStates(ModuleBuildNode rootNode)
        {
            var moduleConnections = new Dictionary<GameObject, SnapConnection[]>();
            TraverseTree(rootNode, delegate (ModuleBuildNode node)
            {
                if (!moduleConnections.ContainsKey(node.spawnedModule))
                {
                    var connections = node.spawnedModule.GetComponentsInChildren<SnapConnection>();
                    moduleConnections.Add(node.spawnedModule, connections);
                }
            });

            // Set everything to wall
            foreach (var connections in moduleConnections.Values)
            {
                foreach (var connection in connections)
                {
                    connection.UpdateDoorState(SnapConnectionState.Wall);
                }
            }

            var stack = new Stack<ModuleBuildNode>();
            stack.Push(rootNode);
            while (stack.Count > 0)
            {
                ModuleBuildNode top = stack.Pop();
                if (top == null) continue;
                ModuleBuildNode parent = top.Parent;
                if (parent != null)
                {
                    if (top.spawnedModule != null && parent.spawnedModule != null)
                    {
                        int ParentDoorIndex = top.IncomingDoorIndex;
                        int TopDoorIndex = top.AttachmentConfig.AttachedModuleDoorIndex;
                        var parentConnection = GetArrayEntry<SnapConnection>(ParentDoorIndex, moduleConnections[parent.spawnedModule]);
                        var topConnection = GetArrayEntry<SnapConnection>(TopDoorIndex, moduleConnections[top.spawnedModule]);

                        if (parentConnection != null)
                        {
                             parentConnection.UpdateDoorState(SnapConnectionState.Door);
                        }
                        if (topConnection != null)
                        {
                            topConnection.UpdateDoorState(snapConfig.hideDuplicateDoors 
                                ? SnapConnectionState.None
                                : SnapConnectionState.Door);
                        }
                    }
                }

                foreach (var extension in top.Extensions)
                {
                    stack.Push(extension);
                }
            }
        }

        
        delegate void VisitTreeNodeDelegate(ModuleBuildNode Node);
        void TraverseTree(ModuleBuildNode RootNode, VisitTreeNodeDelegate VisitTreeNode)
        {
            var stack = new Stack<ModuleBuildNode>();
            stack.Push(RootNode);
            
            while (stack.Count > 0)
            {
                ModuleBuildNode Top = stack.Pop();
                if (Top == null) continue;

                VisitTreeNode(Top);

                // Add children
                foreach (ModuleBuildNode Extension in Top.Extensions)
                {
                    stack.Push(Extension);
                }
            }
        }


        static void CalculateOccupiedBounds(ModuleBuildNode Node, List<Bounds> OccupiedBounds)
        {
            if (Node == null) return;
            OccupiedBounds.Add(Node.AttachmentConfig.AttachedModuleWorldBounds);

            foreach (var ChildNode in Node.Extensions)
            {
                CalculateOccupiedBounds(ChildNode, OccupiedBounds);
            }
        }

        protected virtual Bounds GetBounds(GameObject target)
        {
            return SnapUtils.GetSnapModuleBounds(target);
        }
        
        ModuleInfo GenerateModuleInfo(SnapModuleEntry modulePrefab)
        {
            var moduleInfo = new ModuleInfo();
            moduleInfo.ModuleTemplate = modulePrefab;
            moduleInfo.ModuleGuid = System.Guid.NewGuid();
            moduleInfo.Bounds = GetBounds(modulePrefab.module);

            // Find the transform of the doors
            SnapUtils.FindConnectionTransforms(modulePrefab.module, out moduleInfo.ConnectionTransforms, out moduleInfo.ConnectionCategory);

            return moduleInfo;
        }

        void DebugLog(string name, ref Matrix4x4 Transform)
        {
            Debug.Log(string.Format(@"{0}: Pos:{1} | Rot:{2} | Scl:{3}", 
                name,
                Matrix.GetTranslation(ref Transform),
                Matrix.GetRotation(ref Transform).eulerAngles,
                Matrix.GetScale(ref Transform)));
        }

        protected virtual Matrix4x4[] FindAttachmentTransforms(ref Matrix4x4 ParentModuleTransform, ref Matrix4x4 IncomingDoorTransform, ref Matrix4x4 AttachmentDoorTransform)
        {
            return SnapUtils.FindAttachmentTransforms(ref ParentModuleTransform, ref IncomingDoorTransform, ref AttachmentDoorTransform);
        }

        Bounds GetModulePrefabBounds(GameObject prefab, Bounds bounds)
        {
            if (prefab == null)
            {
                return bounds;
            }

            return new Bounds( bounds.center - prefab.transform.position, bounds.extents * 2);
        }

        bool FindAttachmentConfiguration(ModuleInfo TargetModule, ModuleInfo IncomingModule, ref Matrix4x4 IncomingModuleTransform,
	            int IncomingDoorIndex, HashSet<Bounds> OccupiedBounds, ref SnapAttachmentConfiguration OutAttachmentConfig)
        {
            int TargetNumDoors = TargetModule.ConnectionTransforms.Length;
            //if (IncomingDoorIndex >= TargetNumDoors) return false;

            if (IncomingDoorIndex < 0 || IncomingModule == null)
            {
                OutAttachmentConfig.AttachedModule = TargetModule;
                OutAttachmentConfig.AttachedModuleDoorIndex = random.Range(0, TargetNumDoors - 1);
                OutAttachmentConfig.AttachedModuleWorldBounds = GetModulePrefabBounds(TargetModule.ModuleTemplate.module, TargetModule.Bounds);
                OutAttachmentConfig.AttachedModuleTransform = IncomingModuleTransform;
                return true;
            }

            //if (IncomingDoorIndex >= TargetNumDoors) return false;
            Matrix4x4 IncomingDoorTransform = IncomingModule.ConnectionTransforms[IncomingDoorIndex];
            string IncomingDoorCategory = IncomingModule.ConnectionCategory[IncomingDoorIndex];

            bool bFoundValid = false;
            int[] ShuffledIndices = MathUtils.GetShuffledIndices(TargetNumDoors, random);
            for (int si = 0; si < ShuffledIndices.Length; si++)
            {
                int Index = ShuffledIndices[si];
                string AttachmentDoorCategory = TargetModule.ConnectionCategory[Index];
                if (AttachmentDoorCategory != IncomingDoorCategory)
                {
                    // The doors do not fit
                    continue;
                }

                Matrix4x4 AttachmentDoorTransform = TargetModule.ConnectionTransforms[Index];

                // Align the module with a door that fits the incoming door
                Matrix4x4[] ModuleTransforms = FindAttachmentTransforms(ref IncomingModuleTransform, ref IncomingDoorTransform, ref AttachmentDoorTransform);

                foreach (var ModuleTransform in ModuleTransforms)
                {
                    if (!snapConfig.RotateModulesToFit)
                    {
                        // Rotation is not allowed. Check if we rotated the module
                        Matrix4x4 ModuleTransformCopy = ModuleTransform;
                        var moduleRotation = Matrix.GetRotation(ref ModuleTransformCopy);
                        if (Mathf.Abs(moduleRotation.eulerAngles.y) > 0.1f)
                        {
                            // Module was rotated
                            continue;
                        }
                    }

                    {
                        // Calculate the bounds of the module 
                        Bounds ModuleWorldBounds = GetModulePrefabBounds(TargetModule.ModuleTemplate.module, TargetModule.Bounds);
                        ModuleWorldBounds = MathUtils.TransformBounds(ModuleTransform, ModuleWorldBounds);
                        Bounds ContractedModuleWorldBounds = ExpandBounds(ModuleWorldBounds, -1 * (snapConfig.CollisionTestContraction + 1e-4f));

                        // Check if this module would intersect with any of the existing modules
                        bool bIntersects = false;
                        foreach (var OccupiedBound in OccupiedBounds)
                        {
                            if (ContractedModuleWorldBounds.Intersects(OccupiedBound))
                            {
                                // intersects. Do not spawn a module here
                                bIntersects = true;
                                break;
                            }
                        }
                        if (bIntersects)
                        {
                            continue;
                        }

                        // We found a valid module. Use this
                        OutAttachmentConfig.AttachedModule = TargetModule;
                        OutAttachmentConfig.AttachedModuleDoorIndex = Index;
                        OutAttachmentConfig.AttachedModuleWorldBounds = ModuleWorldBounds;
                        OutAttachmentConfig.AttachedModuleTransform = ModuleTransform;
                        bFoundValid = true;
                        break;
                    }
                }
            }

	        return bFoundValid;
        }

        int[] FindFilteredModuleList(List<ModuleInfo> ModuleInfoList, string category)
        {
            var indices = new List<int>();
            for (int i = 0; i < ModuleInfoList.Count; i++)
            {
                var moduleInfo = ModuleInfoList[i];
                if (moduleInfo.ModuleTemplate.category == category)
                {
                    indices.Add(i);
                }
            }

            return indices.ToArray();
        }

        public override void DebugDrawGizmos()
        {
            if (snapModel != null)
            {
                foreach (var module in snapModel.modules)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(module.WorldBounds.center, module.WorldBounds.size);
                }
            }
        }

        public override void DebugDraw()
        {
            if (snapModel == null)
            {
                snapModel = model as SnapModel;
            }
            if (snapModel == null)
            {
                return;
            }

            var moduleLookup = new Dictionary<string, SnapModuleInstance>();
            foreach (var module in snapModel.modules)
            {
                moduleLookup.Add(module.InstanceID, module);
            }

            foreach (var connection in snapModel.connections)
            {
                if (moduleLookup.ContainsKey(connection.ModuleAInstanceID) 
                    && moduleLookup.ContainsKey(connection.ModuleBInstanceID))
                {
                    var moduleA = moduleLookup[connection.ModuleAInstanceID];
                    var moduleB = moduleLookup[connection.ModuleBInstanceID];
                    if (moduleA != null && moduleB != null)
                    {
                        var start = moduleA.WorldBounds.center;
                        var end = moduleB.WorldBounds.center;

                        Debug.DrawLine(start, end, Color.red, 0, false);
                    }
                }
            }

        }

        Bounds ExpandBounds(Bounds bounds, float amount)
        {
            amount *= 0.5f;
            var extents = bounds.extents;
            extents += new Vector3(amount, amount, amount);
            extents.x = Mathf.Max(extents.x, 0.0f);
            extents.y = Mathf.Max(extents.y, 0.0f);
            extents.z = Mathf.Max(extents.z, 0.0f);

            bounds.extents = extents;
            return bounds;
        }
        
        ModuleBuildNode BuildLayoutRecursive(ModuleGrowthNode GrowthNode, ref HashSet<Bounds> _OccupiedBounds, RNode graphNode, SnapLayoutBuildState RecursiveState) 
        {
            RecursiveState.NumTries++;
            if (RecursiveState.NumTries >= snapConfig.MaxProcessingPower)
            {
                return null;
            }

            if (graphNode == null || graphNode.Payload == null || graphNode.Payload.nodeType == null)
            {
                return null;
            }

            ModuleGrowthNode Top = GrowthNode;

            // Pick a door from this module to extend
            ModuleBuildNode BestBuildNode = null;

            HashSet<Bounds> OccupiedBounds = new HashSet<Bounds>(_OccupiedBounds);

            int[] ModuleListIndices;
            ModuleListIndices = FindFilteredModuleList(RecursiveState.ModuleInfoList, graphNode.Payload.nodeType.nodeName);
            MathUtils.Shuffle(ModuleListIndices, random);

            for (int si = 0; si < ModuleListIndices.Length; si++)
            {
                int Index = ModuleListIndices[si];

                ModuleInfo Module = RecursiveState.ModuleInfoList[Index];

                var AttachmentConfig = new SnapAttachmentConfiguration();
                if (!FindAttachmentConfiguration(Module, Top.IncomingModule, ref Top.ModuleTransform, Top.IncomingModuleDoorIndex, OccupiedBounds, ref AttachmentConfig))
                {
                    continue;
                }

                var  BuildNode = new ModuleBuildNode();
                BuildNode.AttachmentConfig = AttachmentConfig;
                BuildNode.IncomingDoorIndex = Top.IncomingModuleDoorIndex;
                BuildNode.Module = Module;

                // We found a valid module. Use this
                {
                    Bounds contractedModuleWorldBounds = ExpandBounds(AttachmentConfig.AttachedModuleWorldBounds, -1 * snapConfig.CollisionTestContraction);
                    OccupiedBounds.Add(contractedModuleWorldBounds);
                }

                int AttachmentDoorIndex = AttachmentConfig.AttachedModuleDoorIndex;

                var childGraphNodes = graphNode.Outgoing.ToArray();

                bool allChildrenFound = true;
                foreach (var childGraphNode in childGraphNodes)
                {
                    ModuleBuildNode ChildBuildNode = null;
                    HashSet<Bounds> ChildOccupiedBounds = null;

                    // Extend from this door further
                    var ExtensionDoorIndices = MathUtils.GetShuffledIndices(Module.ConnectionTransforms.Length, random);
                    for (int ExtensionDoorIndexRef = 0; ExtensionDoorIndexRef < ExtensionDoorIndices.Length; ExtensionDoorIndexRef++)
                    {
                        int ExtensionDoorIndex = ExtensionDoorIndices[ExtensionDoorIndexRef];
                        if (ExtensionDoorIndex == AttachmentDoorIndex && Top.IncomingModuleDoorIndex != -1)
                        {
                            // Don't want to extend from the door we came in through
                            continue;
                        }

                        /////////////////////////// TODO; Handle child occupied bounds

                        // Grow this branch further
                        var NextNode = new ModuleGrowthNode();
                        NextNode.IncomingModuleDoorIndex = ExtensionDoorIndex;
                        NextNode.ModuleTransform = AttachmentConfig.AttachedModuleTransform;
                        NextNode.IncomingModule = Module;
                        var ExtensionOccupiedBounds = new HashSet<Bounds>(OccupiedBounds);
                        ModuleBuildNode ExtensionNode = BuildLayoutRecursive(NextNode, ref ExtensionOccupiedBounds, childGraphNode, RecursiveState);

                        if (ExtensionNode != null)
                        {
                            ChildBuildNode = ExtensionNode;
                            ChildOccupiedBounds = ExtensionOccupiedBounds;
                            break;
                        }
                    }

                    if (ChildBuildNode != null)
                    {
                        // We cannot grow the child branches
                        ChildBuildNode.Parent = BuildNode;
                        BuildNode.Extensions.Add(ChildBuildNode);

                        OccupiedBounds = ChildOccupiedBounds;
                    }
                    else
                    {
                        allChildrenFound = false;
                        break;
                    }
                }

                if (allChildrenFound)
                {
                    BestBuildNode = BuildNode;
                    _OccupiedBounds = OccupiedBounds;
                    break;
                }
            }

            return BestBuildNode;
        }
    }


    namespace Impl
    {
        class ModuleInfo
        {
            /// <summary>
            /// The prefab template of the module
            /// </summary>
            public SnapModuleEntry ModuleTemplate;

            /// <summary>
            ///  A unique ID assigned to this actor module (unique to the prefab). Will be different on each build
            /// </summary>
            public System.Guid ModuleGuid;

            /// <summary>
            /// The bounds of the prefab
            /// </summary>
            public Bounds Bounds;

            /// <summary>
            /// The local transforms of each SnapConnection child actor in the module actor
            /// </summary>
            public Matrix4x4[] ConnectionTransforms;


            public string[] ConnectionCategory;
        }

        class SnapAttachmentConfiguration
        {
            public ModuleInfo AttachedModule;
            public int AttachedModuleDoorIndex;
            public Bounds AttachedModuleWorldBounds;
            public Matrix4x4 AttachedModuleTransform;
        }

        class ModuleGrowthNode
        {
            public ModuleGrowthNode()
            {
                IncomingModuleDoorIndex = -1;
                startNode = false;
                ModuleTransform = Matrix4x4.identity;
            }

            public Matrix4x4 ModuleTransform;
            public ModuleInfo IncomingModule;
            public int IncomingModuleDoorIndex;
            public bool startNode;
        }

        class ModuleBuildNode
        {

                public static string GenerateModuleInstanceID(System.Guid ModuleGuid)
                {
                    return "NODE-SNAPMOD-" + ModuleGuid.ToString();
                }

            public ModuleBuildNode()
            {
                ModuleInstanceID = GenerateModuleInstanceID(System.Guid.NewGuid());
                IncomingDoorIndex = -1;
                DepthFromLeaf = 1;
            }

            public string ModuleInstanceID;
            public ModuleInfo Module;
            public int IncomingDoorIndex;
            public SnapAttachmentConfiguration AttachmentConfig;
            public int DepthFromLeaf;
            public List<ModuleBuildNode> Extensions = new List<ModuleBuildNode>();
            public ModuleBuildNode Parent;

            /// <summary>
            /// Reference to the spawned module. This will be set later
            /// </summary>
            public GameObject spawnedModule = null;     
        };

        class SnapLayoutBuildState
        {
            public SnapLayoutBuildState()
            {
                bSafetyBailOut = false;
                NumTries = 0;
                bFoundBestBuild = false;
            }

            /**
            Searching a dense tree can lead to billions of possibilities
            If this flag is set, the search bails out early to avoid a hang
            */
            public bool bSafetyBailOut;
            public int NumTries;
            public bool bFoundBestBuild;
            public List<ModuleInfo> ModuleInfoList = new List<ModuleInfo>();
        };
    }
}