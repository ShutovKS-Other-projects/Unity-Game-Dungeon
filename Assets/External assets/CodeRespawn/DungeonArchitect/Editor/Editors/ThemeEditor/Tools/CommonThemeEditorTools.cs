//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using DungeonArchitect.Graphs;
using DungeonArchitect.Builders;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Builders.Isaac;
using DungeonArchitect.Builders.SimpleCity;
using DungeonArchitect.Builders.CircularCity;
using DungeonArchitect.Builders.FloorPlan;
using DungeonArchitect.Builders.Mario;
using DungeonArchitect.Builders.Maze;
using DungeonArchitect.Builders.BSP;
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.UI;
using DungeonArchitect.Builders.Infinity.Caves;
using DungeonArchitect.UI.Impl.UnityEditor;

namespace DungeonArchitect.Editors.ThemeEditorTools
{
    public class CommonThemeEditorTools
    {
        [ThemeEditorTool("Refresh Thumbnails", 100)]
        public static void RefreshThumbnails(DungeonThemeEditorWindow editor)
        {
            if (editor.GraphEditor != null)
            {
                AssetThumbnailCache.Instance.Reset();
            }
        }

        [ThemeEditorTool("Center On Graph", 200)]
        public static void CenterOnGraph(DungeonThemeEditorWindow editor)
        {
            if (editor.GraphEditor != null)
            {
                editor.GraphEditor.FocusCameraOnBestFit(editor.position);
            }
        }

        [ThemeEditorTool("Advanced/Recreate Node Ids", 100100)]
        public static void RecreateNodeIds(DungeonThemeEditorWindow editor)
        {
            var confirm = EditorUtility.DisplayDialog("Recreate Node Ids?",
                    "Are you sure you want to recreate node Ids?  You should do this after cloning a theme file", "Yes", "Cancel");
            if (confirm)
            {
                DungeonEditorHelper._Advanced_RecreateGraphNodeIds();
            }
        }
    }


    public class BuilderThemeEditorTools
    {
        [ThemeEditorTool("Create Default Markers For/Grid Builder", 10100)]
        public static void CreateDefaultMarkersForGrid(DungeonThemeEditorWindow editor)
        {
            CreateDefaultMarkersFor(editor.GraphEditor, typeof(GridDungeonBuilder), editor.uiSystem);
        }

        [ThemeEditorTool("Create Default Markers For/Grid Flow Builder", 10105)]
        public static void CreateDefaultMarkersForGridFlow(DungeonThemeEditorWindow editor)
        {
            CreateDefaultMarkersFor(editor.GraphEditor, typeof(GridFlowDungeonBuilder), editor.uiSystem);
        }

        [ThemeEditorTool("Create Default Markers For/Simple City Builder", 10200)]
        public static void CreateDefaultMarkersForSimpleCity(DungeonThemeEditorWindow editor)
        {
            CreateDefaultMarkersFor(editor.GraphEditor, typeof(SimpleCityDungeonBuilder), editor.uiSystem);
        }

        [ThemeEditorTool("Create Default Markers For/Circular City Builder", 10200)]
        public static void CreateDefaultMarkersForCircularCity(DungeonThemeEditorWindow editor)
        {
            CreateDefaultMarkersFor(editor.GraphEditor, typeof(CircularCityDungeonBuilder), editor.uiSystem);
        }

        [ThemeEditorTool("Create Default Markers For/Floor Plan Builder", 10300)]
        public static void CreateDefaultMarkersForFloorPlan(DungeonThemeEditorWindow editor)
        {
            CreateDefaultMarkersFor(editor.GraphEditor, typeof(FloorPlanBuilder), editor.uiSystem);
        }

		[ThemeEditorTool("Create Default Markers For/Isaac Builder", 10400)]
		public static void CreateDefaultMarkersForIsaac(DungeonThemeEditorWindow editor)
		{
			CreateDefaultMarkersFor(editor.GraphEditor, typeof(IsaacDungeonBuilder), editor.uiSystem);
		}

        [ThemeEditorTool("Create Default Markers For/Mario Builder", 10500)]
        public static void CreateDefaultMarkersForMario(DungeonThemeEditorWindow editor)
        {
            CreateDefaultMarkersFor(editor.GraphEditor, typeof(MarioDungeonBuilder), editor.uiSystem);
        }

        [ThemeEditorTool("Create Default Markers For/Maze Builder", 10600)]
        public static void CreateDefaultMarkersForMaze(DungeonThemeEditorWindow editor)
        {
            CreateDefaultMarkersFor(editor.GraphEditor, typeof(MazeDungeonBuilder), editor.uiSystem);
        }

        [ThemeEditorTool("Create Default Markers For/Spatial Builder", 10700)]
        public static void CreateDefaultMarkersForBSP(DungeonThemeEditorWindow editor)
        {
            CreateDefaultMarkersFor(editor.GraphEditor, typeof(BSPDungeonBuilder), editor.uiSystem);
        }

        [ThemeEditorTool("Create Default Markers For/Infinity Cave Builder", 10800)]
        public static void CreateDefaultMarkersForInfinityCaves(DungeonThemeEditorWindow editor)
        {
            CreateDefaultMarkersFor(editor.GraphEditor, typeof(InfinityCaveChunkBuilder), editor.uiSystem);
        }

        static void CreateDefaultMarkersFor(GraphEditor graphEditor, Type builderType, UISystem uiSystem)
        {
            // Remove unused nodes
            // Grab the names of all the markers nodes in the graph
            var markerNames = new List<string>();
            foreach (var node in graphEditor.Graph.Nodes)
            {
                if (node is MarkerNode)
                {
                    var markerNode = node as MarkerNode;
                    markerNames.Add(markerNode.Caption);
                }
            }

            var unusedMarkers = new List<string>(markerNames.ToArray());


            // Remove markers from the unused list that have child nodes attached to it
            foreach (var node in graphEditor.Graph.Nodes)
            {
                if (node is VisualNode)
                {
                    var visualNode = node as VisualNode;
                    foreach (var parentNode in visualNode.GetParentNodes())
                    {
                        if (parentNode is MarkerNode)
                        {
                            var markerNode = parentNode as MarkerNode;
                            unusedMarkers.Remove(markerNode.Caption);
                        }
                    }
                }
            }
            
            // Remove markers from the unused list that are referenced by other marker emitters
            foreach (var node in graphEditor.Graph.Nodes)
            {
                if (node is MarkerEmitterNode)
                {
                    var emitterNode = node as MarkerEmitterNode;
                    string markerName = emitterNode.Caption;
                    // this marker is referenced by an emitter.  Remove it from the unused list
                    unusedMarkers.Remove(markerName);
                }
            }

            // Remove markers from the unused list that are required by the new builder type
            var builderMarkers = DungeonBuilderDefaultMarkers.GetDefaultMarkers(builderType);
            foreach (var builderMarker in builderMarkers)
            {
                unusedMarkers.Remove(builderMarker);
            }

            // Remove all the unused markers
            var markerNodesToDelete = new List<MarkerNode>();
            foreach (var node in graphEditor.Graph.Nodes)
            {
                if (node is MarkerNode)
                {
                    var markerNode = node as MarkerNode;
                    if (unusedMarkers.Contains(markerNode.Caption)) {
                        markerNodesToDelete.Add(markerNode);
                    }
                }
            }

            graphEditor.DeleteNodes(markerNodesToDelete.ToArray(), uiSystem);


            // Grab the names of all the markers nodes in the graph
            markerNames.Clear();
            foreach (var node in graphEditor.Graph.Nodes)
            {
                if (node is MarkerNode)
                {
                    var markerNode = node as MarkerNode;
                    markerNames.Add(markerNode.Caption);
                }
            }

            var markersToCreate = new List<string>();
            foreach (var builderMarker in builderMarkers)
            {
                if (!markerNames.Contains(builderMarker))
                {
                    markersToCreate.Add(builderMarker);
                }
            }

            var existingBounds = new List<Rect>();
            foreach (var node in graphEditor.Graph.Nodes)
            {
                existingBounds.Add(node.Bounds);
            }
            
            // Add the new nodes
            const int INTER_NODE_X = 200;
            const int INTER_NODE_Y = 300;
            int itemsPerRow = 5;
            int positionIndex = 0;
            int ix, iy, x, y;
            var markerNodeSize = new Vector2(120, 50);
            for (int i = 0; i < markersToCreate.Count; i++)
            {
                bool overlaps;
                int numOverlapTries = 0;
                int MAX_OVERLAP_TRIES = 100;
                do
                {
                    ix = positionIndex % itemsPerRow;
                    iy = positionIndex / itemsPerRow;
                    x = ix * INTER_NODE_X;
                    y = iy * INTER_NODE_Y;
                    positionIndex++;

                    overlaps = false;
                    var newNodeBounds = new Rect(x, y, markerNodeSize.x, markerNodeSize.y);
                    foreach (var existingBound in existingBounds)
                    {
                        if (newNodeBounds.Overlaps(existingBound))
                        {
                            overlaps = true;
                            break;
                        }
                    }
                    numOverlapTries++;
                } while (overlaps && numOverlapTries < MAX_OVERLAP_TRIES);

                var newNode = GraphOperations.CreateNode<MarkerNode>(graphEditor.Graph, uiSystem.Undo);
                GraphEditorUtils.AddToAsset(new UnityEditorUIPlatform(), graphEditor.Graph, newNode);
                newNode.Position = new Vector2(x, y);
                newNode.Caption = markersToCreate[i];
                
            }
        }
    }
}
