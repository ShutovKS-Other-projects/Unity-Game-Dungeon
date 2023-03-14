//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.SxEngine;
using DungeonArchitect.SxEngine.Utils;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D
{
    public class SxLayout3DWorldBuilder
    {
        public class BuildSettings
        {
            public SxMaterial NodeMaterial;
            public SxMaterial SubNodeMaterial;
            public SxMaterial MergedNodeMaterial;
            public SxMaterial ItemMaterial;

            private BuildSettings()
            {
            }

            public static BuildSettings Create()
            {
                var settings = new BuildSettings();
                settings.NodeMaterial = SxMaterialRegistry.Get<SxFlowNodeMaterial>();
                settings.SubNodeMaterial = SxMaterialRegistry.Get<SxFlowSubNodeMaterial>();
                settings.MergedNodeMaterial = SxMaterialRegistry.Get<SxFlowMergedNodeMaterial>();
                settings.ItemMaterial = SxMaterialRegistry.Get<SxFlowItemMaterial>();
                return settings;
            }
        }

        public static void Build(SxWorld world, FlowLayoutGraph graph)
        {
            Build(world, graph, BuildSettings.Create(), new FlowLayout3DRenderSettings(0.5f));
        }
        
        public static void Build(SxWorld world, FlowLayoutGraph graph, BuildSettings buildSettings, FlowLayout3DRenderSettings renderSettings) 
        {
            world.Clear();
            
            // build a grid
            {
                var gridMesh = world.SpawnActor<SxMeshActor>(true);
                gridMesh.SetMesh(SxMeshUtils.CreateGridMesh(10, 1.0f));
                gridMesh.SetMaterial<SxGridMaterial>();
            }

            if (graph == null)
            {
                return;
            }

            var graphQuery = new FlowLayoutGraphQuery(graph);
            
            // Build the node actors
            var nodeActors = new Dictionary<DungeonUID, SxLayoutNodeActor>();
            foreach (var layoutNode in graph.Nodes)
            {
                var nodeActor = world.SpawnActor<SxLayoutNodeActor>(true);
                
                var settings = new SxLayoutNodeActor.NodeRenderSettings();
                settings.Position = layoutNode.position;
                settings.NodeRadius = layoutNode.active ? renderSettings.NodeRadius : renderSettings.InactiveNodeRadius;
                settings.Color = layoutNode.active ? layoutNode.color : FlowLayout3DConstants.InactiveNodeColor;
                settings.Material = buildSettings.NodeMaterial;
                nodeActor.Initialize(settings);
                nodeActor.LayoutNode = layoutNode;

                nodeActors[layoutNode.nodeId] = nodeActor;
            }
            
            // Draw the merged node visuals
            foreach (var node in graph.Nodes)
            {
                if (node.MergedCompositeNodes.Count <= 1) continue;

                var bounds = new Bounds(node.position, Vector3.zero);
                foreach (var subNode in node.MergedCompositeNodes)
                {
                    bounds.Encapsulate(subNode.position);
                    
                    // Draw the sub node
                    var subNodeActor = world.SpawnActor<SxLayoutNodeActor>(true);
                    var settings = new SxLayoutNodeActor.NodeRenderSettings();
                    settings.Position = subNode.position;
                    settings.NodeRadius = renderSettings.InactiveNodeRadius;
                    settings.Color = new Color(0, 0, 0, 0.75f);
                    settings.Material = buildSettings.SubNodeMaterial;
                    subNodeActor.Initialize(settings);
                    subNodeActor.LayoutNode = subNode;

                    nodeActors[subNode.nodeId] = subNodeActor;
                }

                bounds.extents += Vector3.one * renderSettings.NodeRadius;

                {
                    var cubeActor = world.SpawnActor<SxLayoutMergedNodeActor>(true);
                    var settings = new SxLayoutMergedNodeActor.RenderSettings();
                    settings.Color = node.color;
                    settings.Bounds = bounds;
                    settings.Material = buildSettings.MergedNodeMaterial;
                    cubeActor.Initialize(settings);
                }
            }

            // Build the link actors
            var linkActors = new Dictionary<DungeonUID, SxLayoutLinkActor>();
            foreach (var link in graph.Links)
            {
                if (link.state.type == FlowLayoutGraphLinkType.Unconnected)
                {
                    continue;
                }

                var startNode = link.sourceSubNode.IsValid() ? graphQuery.GetSubNode(link.sourceSubNode) : graphQuery.GetNode(link.source);
                var endNode = link.destinationSubNode.IsValid() ? graphQuery.GetSubNode(link.destinationSubNode) : graphQuery.GetNode(link.destination);
                if (startNode == null || endNode == null)
                {
                    continue;
                }
                
                SxLayoutNodeActor startNodeActor = nodeActors.ContainsKey(startNode.nodeId) ? nodeActors[startNode.nodeId] : null;
                SxLayoutNodeActor endNodeActor = nodeActors.ContainsKey(endNode.nodeId) ? nodeActors[endNode.nodeId] : null;
                if (startNodeActor != null && endNodeActor != null)
                {
                    var linkActor = world.SpawnActor<SxLayoutLinkActor>(true);
                    bool oneWay = (link.state.type == FlowLayoutGraphLinkType.OneWay);
                    var settings = new SxLayoutLinkActor.LinkRenderSettings();
                    settings.StartNode = startNodeActor;
                    settings.EndNode = endNodeActor;
                    settings.StartRadius = renderSettings.NodeRadius;
                    settings.EndRadius = renderSettings.NodeRadius;
                    settings.OneWay = oneWay;
                    settings.Color = oneWay ? FlowLayout3DConstants.LinkOneWayColor : FlowLayout3DConstants.LinkColor;
                    settings.Thickness = renderSettings.LinkThickness;
                    linkActor.Initialize(settings);
                    linkActor.Link = link;

                    linkActors[link.linkId] = linkActor;
                }
            }
            
            var itemActors = new Dictionary<DungeonUID, SxLayoutItemActor>();
            // Build the node item actors
            foreach (var layoutNode in graph.Nodes)
            {
                if (!layoutNode.active || !nodeActors.ContainsKey(layoutNode.nodeId)) continue;
                
                var nodeActor = nodeActors[layoutNode.nodeId];
                if (nodeActor == null) continue;

                var nodeItems = layoutNode.items;
                float itemCount = nodeItems.Count;
                for (var i = 0; i < nodeItems.Count; i++)
                {
                    var item = nodeItems[i];
                    var angle = 2.0f * Mathf.PI / itemCount * i;
                    var offset = 1 - FlowLayout3DConstants.ItemNodeScaleMultiplier - 0.05f;
                    var x = Mathf.Cos(angle) * offset;
                    var y = Mathf.Sin(angle) * offset;
                    var itemActor = world.SpawnActor<SxLayoutItemActor>(false);

                    Color colorBackground;
                    Color colorText;
                    
                    FlowItemUtils.GetFlowItemColor(item, out colorBackground, out colorText);
                    var itemText = FlowItemUtils.GetFlowItemText(item);
                    
                    var offsetZSign = -1;
                    
                    var settings = new SxLayoutNodeActor.NodeRenderSettings();
                    settings.Position = new Vector3(x, y, -0.05f * offsetZSign);
                    settings.NodeRadius = FlowLayout3DConstants.ItemNodeScaleMultiplier;
                    settings.Color = colorBackground;
                    settings.Material = buildSettings.ItemMaterial;
                    settings.Text = itemText;
                    settings.TextColor = colorText;
                    settings.TextScale = 1.5f;
                    settings.TextDepthBias = -1;
                    
                    itemActor.Initialize(settings);
                    itemActor.Item = item;
                    itemActor.AlignToCamera = false;
                    
                    nodeActor.AddChild(itemActor);
                    
                    itemActors[item.itemId] = itemActor;
                }
            }
            
            // Build the link item actors
            foreach (var link in graph.Links)
            {
                if (link.state.type == FlowLayoutGraphLinkType.Unconnected)
                {
                    continue;
                }

                if (!linkActors.ContainsKey(link.linkId))
                {
                    continue;
                }

                var linkActor = linkActors[link.linkId];
                if (linkActor == null || linkActor.StartActor == null || linkActor.EndActor == null)
                {
                    continue;
                }

                var startPosition = linkActor.StartActor.Position;
                var endPosition = linkActor.EndActor.Position;
                var center = (startPosition + endPosition) * 0.5f;
                var direction = (endPosition - startPosition).normalized;

                var linkItems = link.state.items;
                for (var i = 0; i < linkItems.Count; i++)
                {
                    var item = linkItems[i];
                    
                    var angle = Mathf.PI * 0.5f + 2.0f * Mathf.PI / linkItems.Count * i;
                    var distance = renderSettings.ItemRadius * 1.5f;
                    var x = Mathf.Cos(angle) * distance;
                    var z = Mathf.Sin(angle) * distance;

                    var rotation = Quaternion.FromToRotation(Vector3.up, direction);
                    var offset = rotation * new Vector3(x, 0, z);
                    var position = center + offset;
                 
                    var itemActor = world.SpawnActor<SxLayoutItemActor>(true);

                    Color colorBackground;
                    Color colorText;
                    
                    FlowItemUtils.GetFlowItemColor(item, out colorBackground, out colorText);
                    var itemText = FlowItemUtils.GetFlowItemText(item);
                    
                    var settings = new SxLayoutNodeActor.NodeRenderSettings();
                    settings.Position = position;
                    settings.NodeRadius = renderSettings.NodeRadius * FlowLayout3DConstants.ItemNodeScaleMultiplier;
                    settings.Color = colorBackground;
                    settings.Material = buildSettings.ItemMaterial;
                    settings.Text = itemText;
                    settings.TextColor = colorText;
                    settings.TextScale = 1.5f;
                    settings.TextDepthBias = -2f;
                    
                    itemActor.Initialize(settings);
                    itemActor.Item = item;
                    
                    itemActors[item.itemId] = itemActor;
                }
            }
            
            // Link the referenced items
            var allItems = new List<FlowItem>();
            foreach (var layoutNode in graph.Nodes)
            {
                allItems.AddRange(layoutNode.items);
            }
            foreach (var item in allItems)
            {
                if (!itemActors.ContainsKey(item.itemId)) continue;
                var itemActor = itemActors[item.itemId];
                foreach (var referencedItemId in item.referencedItemIds)
                {
                    if (!itemActors.ContainsKey(referencedItemId)) continue;
                    var referencedItem = itemActors[referencedItemId];

                    SxLayoutItemActor startNodeActor = itemActor;
                    SxLayoutItemActor endNodeActor = referencedItem;
                    
                    var linkActor = world.SpawnActor<SxLayoutLinkActor>(true);
                    var settings = new SxLayoutLinkActor.LinkRenderSettings();
                    settings.StartNode = startNodeActor;
                    settings.EndNode = endNodeActor;
                    settings.StartRadius = renderSettings.ItemRadius;
                    settings.EndRadius = renderSettings.ItemRadius;
                    settings.OneWay = false;
                    settings.Color = FlowLayout3DConstants.LinkItemRefColor;
                    settings.Thickness = renderSettings.LinkThickness * 0.5f;
                    linkActor.Initialize(settings);
                    linkActor.FixPositionEveryFrame = true;
                }
            }
        }
    }
    
    public class SxLayoutLinkActor : SxMeshActor
    {
        public FlowLayoutGraphLink Link;
        public bool FixPositionEveryFrame = false;
        private LinkRenderSettings settings = new LinkRenderSettings();
        private SxMeshActor headActor;

        public SxActor StartActor
        {
            get => settings.StartNode;
        }
        
        public SxActor EndActor
        {
            get => settings.EndNode;
        }
        
        public LinkRenderSettings Settings
        {
            get => settings;
        }

        public struct LinkRenderSettings
        {
            public SxActor StartNode;
            public float StartRadius;
            public SxActor EndNode;
            public float EndRadius;
            public bool OneWay;
            public Color Color;
            public float Thickness;
        }

        public void Initialize(LinkRenderSettings settings)
        {
            this.settings = settings;
            
            RemoveAllChildren();
            SetMesh(new SxQuadMesh(settings.Color));
            SetMaterial<SxFlowLinkMaterial>();
            
            headActor = World.SpawnActor<SxMeshActor>(false);
            AddChild(headActor);

            if (settings.OneWay)
            {
                headActor.SetMaterial<SxFlowLinkOneWayHeadMaterial>();
            }
            else
            {
                headActor.SetMaterial<SxFlowLinkHeadMaterial>();
            }
            headActor.SetMesh(new SxQuadMesh(settings.Color));

            OrientLinkToNodes();
        }
        
        public override void Tick(SxRenderContext context, float deltaTime)
        {
            base.Tick(context, deltaTime);

            if (FixPositionEveryFrame)
            {
                OrientLinkToNodes();
            }
            
            OrientLinkToCamera(context.CameraPosition);
        }

        void OrientLinkToNodes()
        {
            var headThickness = settings.Thickness * FlowLayout3DConstants.LinkHeadThicknessMultiplier;
            float headLength = headThickness;
            float headWidth = headThickness;
            if (settings.OneWay)
            {
                headLength *= 2;
            }

            var startTransform = SxSceneGraphUtils.AccumulateTransforms(settings.StartNode);
            var endTransform = SxSceneGraphUtils.AccumulateTransforms(settings.EndNode);

            var start = Matrix.GetTranslation(ref startTransform);
            var end = Matrix.GetTranslation(ref endTransform);

            var direction = (end - start).normalized;
            start = start + direction * settings.StartRadius;
            end = end - direction * (settings.EndRadius + headLength);

            var length = (end - start).magnitude;
            
            Rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), direction);
            Position = start + (end - start) * 0.5f;
            Scale = new Vector3(length / 2.0f, settings.Thickness / 2.0f, 1);

            {
                var headScale = Vector3.Scale(Vector3.one / 2.0f, new Vector3(headLength, headWidth, 1));
                headScale = MathUtils.Divide(headScale, Scale);
                headActor.Scale = headScale;
                headActor.Position = MathUtils.Divide(new Vector3(length * 0.5f + headLength * 0.5f, 0, 0), Scale);
            }
        }

        void OrientLinkToCamera(Vector3 cameraPosition)
        {
            if (settings.StartNode != null && settings.EndNode != null)
            {
                var startTransform = SxSceneGraphUtils.AccumulateTransforms(settings.StartNode);
                var endTransform = SxSceneGraphUtils.AccumulateTransforms(settings.EndNode);

                var start = Matrix.GetTranslation(ref startTransform);
                var end = Matrix.GetTranslation(ref endTransform);
                
                var length = (end - start).magnitude;
                var axisX = (end - start) / length;
                var axisZ = (cameraPosition - Position).normalized;
                var axisY = Vector3.Cross(axisZ, axisX);
                axisZ = Vector3.Cross(axisX, axisY);

                var rotationMatrix = new Matrix4x4(axisX, axisY, axisZ, new Vector4(0, 0, 0, 1));
                Rotation = rotationMatrix.rotation;
            }
        }
    }

    public class SxLayoutNodeActor : SxLayoutNodeActorBase
    {
        public FlowLayoutGraphNode LayoutNode;
    }
    
    public class SxLayoutItemActor : SxLayoutNodeActorBase
    {
        public FlowItem Item;
    }

    public class SxLayoutMergedNodePlaneActor : SxMeshActor
    {
        public void Initialize(Color color, SxMaterial material)
        {
            SetMesh(new SxQuadMesh(color));
            SetMaterial(material);
        }
    }
    
    public class SxLayoutMergedNodeActor : SxActor
    {
        public struct RenderSettings
        {
            public Bounds Bounds;
            public Color Color;
            public SxMaterial Material;
        }

        struct PlaneTransform
        {
            public Vector3 position;
            public Quaternion rotation;

            public PlaneTransform(Vector3 position, Quaternion rotation)
            {
                this.position = position;
                this.rotation = rotation;
            }
        }

        public void Initialize(RenderSettings settings)
        {
            var planeOffsets = new Vector3[]
            {
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0)
            };

            var quadForward = new Vector3(0, 0, -1);
            foreach (var offset in planeOffsets)
            {
                var side = World.SpawnActor<SxLayoutMergedNodePlaneActor>(false);
                side.Initialize(settings.Color, settings.Material);
                side.Position = offset;
                side.Rotation = Quaternion.FromToRotation(quadForward, offset);
                AddChild(side);    
            }
            
            Scale = settings.Bounds.extents;
            Position = settings.Bounds.center;
        }
    }
    
    public abstract class SxLayoutNodeActorBase : SxMeshActor
    {
        public struct NodeRenderSettings
        {
            public float NodeRadius;
            public Vector3 Position;
            public Color Color;
            public SxMaterial Material;
            public string Text;
            public Color TextColor;
            public float TextScale;
            public float TextDepthBias;
        }
        
        public bool AlignToCamera { get; set; } = true;
        private NodeRenderSettings settings;

        public void Initialize(NodeRenderSettings settings)
        {
            this.settings = settings;
            Position = settings.Position;
            var color = settings.Color;
            var scaleF = settings.NodeRadius;
            
            Scale = new Vector3(scaleF, scaleF, scaleF);
            SetMesh(new SxQuadMesh(color));
            SetMaterial(settings.Material);

            if (settings.Text != null)
            {
                var textActor = World.SpawnActor<SxTextActor>(false);
                var textSettings = new SxTextComponentSettings()
                {
                    Font = Layout3DGraphRenderingResource.GetFont(),
                    Color = settings.TextColor,
                    Scale = settings.TextScale,
                    HAlign = SxTextHAlign.Center,
                    VAlign = SxTextVAlign.Center,
                    DepthBias = settings.TextDepthBias,
                };
                
                textActor.TextComponent.Initialize(textSettings);
                textActor.TextComponent.Text = settings.Text;

                textActor.Position = new Vector3(0, 0, 0.1f);
                textActor.Rotation = Quaternion.AngleAxis(180, Vector3.up);
                AddChild(textActor);
            }
        }

        public override void Tick(SxRenderContext context, float deltaTime)
        {
            base.Tick(context, deltaTime);
            if (AlignToCamera)
            {
                // Align the quad to camera
                var axisZ = (context.CameraPosition - Position).normalized;
                var axisX = Vector3.Cross(Vector3.up, axisZ);
                var axisY = Vector3.Cross(axisZ, axisX);

                var rotationMatrix = new Matrix4x4(axisX, axisY, axisZ, new Vector4(0, 0, 0, 1));
                Rotation = rotationMatrix.rotation;
            }
        }
    }
    
    class Layout3DGraphRenderingResource
    {
        private static Font font;

        public static Font GetFont()
        {
            if (font == null)
            {
                font = Resources.Load<Font>("ConsolasBold");
            }
            
            return font;
        }
    }
    
}