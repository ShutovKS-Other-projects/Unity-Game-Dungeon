//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using DungeonArchitect.Editors.Utils;
using DungeonArchitect.Editors.Visualization;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Impl.UnityEditor;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// The main editor window for the Theme graph editor.  This hosts the graph editor for managing the theme graph
    /// </summary>
    public class DungeonThemeEditorWindow : EditorWindow
    {
        [SerializeField]
        DungeonThemeGraphEditor graphEditor;
        
        public DungeonThemeGraphEditor GraphEditor
        {
            get { return graphEditor; }
        }

        public UISystem uiSystem { get; private set; }
        UIRenderer renderer;

        public static void ShowEditor()
        {
            EditorWindow.GetWindow<DungeonThemeEditorWindow>();
        }

        public void Init(Graph graph)
        {
            CreateUISystem();
            this.titleContent = new GUIContent("Dungeon Theme");
            if (graphEditor != null)
            {
                graphEditor.Init(graph, position, graph, uiSystem);
                Repaint();
            }

            // Grab the list of tools that we would be using
            toolFunctions = FetchToolFunctions();
        }

        void CreateUISystem()
        {
            uiSystem = new UnityEditorUISystem();
            renderer = new UnityEditorUIRenderer();
        }

        ThemeEditorToolFunctionInfo[] toolFunctions;

        static ThemeEditorToolFunctionInfo[] FetchToolFunctions()
        {
            var functions = new List<ThemeEditorToolFunctionInfo>();
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(ThemeEditorToolFunctionDelegate));
            var methods = assembly.GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(ThemeEditorToolAttribute), false).Length > 0)
                      .ToArray();
            foreach (var method in methods)
            {
                if (method.IsStatic)
                {
                    var function = Delegate.CreateDelegate(typeof(ThemeEditorToolFunctionDelegate), method) as ThemeEditorToolFunctionDelegate;
                    if (function != null)
                    {
                        var functionInfo = new ThemeEditorToolFunctionInfo();
                        functionInfo.function = function;
                        functionInfo.attribute = method.GetCustomAttributes(typeof(ThemeEditorToolAttribute), false)[0] as ThemeEditorToolAttribute;
                        functions.Add(functionInfo);
                    }
                }
            }

            // Sort based no priority
            return functions.OrderBy(o => o.attribute.Priority).ToArray();
        }
        
        void OnEnable()
        {
            if (graphEditor == null)
            {
                graphEditor = CreateInstance<DungeonThemeGraphEditor>();
            }
            this.wantsMouseMove = true;

            graphEditor.OnEnable();
        }

        void OnDisable()
        {
            if (graphEditor != null)
            {
                graphEditor.OnDisable();
            }
        }

        void OnDestroy()
        {
            if (graphEditor != null)
            {
                graphEditor.OnDisable();
                graphEditor.OnDestroy();
                graphEditor = null;
            }
        }

        void Update()
        {
            if (graphEditor != null)
            {
                graphEditor.Update();
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        string[] GetMarkerNames()
        {
            var markerNames = new List<string>();
            if (graphEditor != null && graphEditor.Graph != null)
            {
                var graph = graphEditor.Graph;
                foreach (var node in graph.Nodes)
                {
                    if (node is MarkerNode)
                    {
                        var markerNode = node as MarkerNode;
                        markerNames.Add(markerNode.Caption);
                    }
                }
            }
            var markerArray = markerNames.ToArray();
            System.Array.Sort(markerArray);
            return markerArray;
        }

        void HandleVisualizeMarkerButtonPressed()
        {
            graphEditor.UpdateMarkerVisualizer();
        }

        void DrawToolbar()
        {
            var graphValid = (graphEditor != null && graphEditor.Graph != null);
            if (toolFunctions == null)
            {
                toolFunctions = FetchToolFunctions();
            }

            if (graphValid)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

                if (GUILayout.Button("Navigate To", EditorStyles.toolbarDropDown))
                {
                    GenericMenu markerMenu = new GenericMenu();
                    var markerNames = GetMarkerNames();
                    if (markerNames.Length > 0)
                    {
                        foreach (var markerName in markerNames)
                        {
							markerMenu.AddItem(new GUIContent(markerName), false, OnJumpTo_MarkerName, markerName);
                        }
                    }

                    // Offset menu from right of editor window
					markerMenu.DropDown(new Rect(0, 0, 0, 20));
                    EditorGUIUtility.ExitGUI();
                }

				if (GUILayout.Button("Tools", EditorStyles.toolbarDropDown)) {
					GenericMenu toolsMenu = new GenericMenu();
                    foreach (var functionInfo in toolFunctions)
                    {
                        toolsMenu.AddItem(new GUIContent(functionInfo.attribute.Path), false, ToolFunctionInvoker, functionInfo.function);
                    }

                    // Offset menu from right of editor window
                    toolsMenu.DropDown(new Rect(80, 0, 0, 20));
					EditorGUIUtility.ExitGUI();
                }

                var themeGraphEditor = graphEditor as DungeonThemeGraphEditor;
                themeGraphEditor.realtimeUpdate = GUILayout.Toggle(themeGraphEditor.realtimeUpdate, "Realtime Update", EditorStyles.toolbarButton);

                var oldVisualizeMarkers = themeGraphEditor.visualizeMarkers;
                themeGraphEditor.visualizeMarkers = GUILayout.Toggle(themeGraphEditor.visualizeMarkers, "Visualize Markers", EditorStyles.toolbarButton);
                if (oldVisualizeMarkers != themeGraphEditor.visualizeMarkers)
                {
                    HandleVisualizeMarkerButtonPressed();
                }

                GUILayout.FlexibleSpace();

                {
                    renderer.backgroundColor = new Color(1, 0.25f, 0.25f, 1);
                    renderer.color = Color.white;
                
                    var iconDiscord = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_DISCORD_16x) as Texture2D;
                    if (GUILayout.Button(new GUIContent(" Discord Support", iconDiscord), DungeonEditorStyles.discordToolButtonStyle))
                    {
                        ExternalLinks.LaunchUrl(ExternalLinks.DiscordInvite);
                    }

                    renderer.backgroundColor = new Color(0.25f, 0.25f, 1, 1);
                    renderer.color = Color.white;
                    var iconDocs = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_DOCS_16x) as Texture2D;
                    if (GUILayout.Button(new GUIContent("Documentation", iconDocs), DungeonEditorStyles.discordToolButtonStyle))
                    {
                        ExternalLinks.LaunchUrl(ExternalLinks.Documentation);
                    }

                }

                EditorGUILayout.EndHorizontal();
            }

        }

        void ToolFunctionInvoker(object userData)
        {
            var toolFunction = userData as ThemeEditorToolFunctionDelegate;
            toolFunction(this);
        }

		void Advanced_OnCreateNodeIds() {
			var confirm = EditorUtility.DisplayDialog("Recreate Node Ids?",
				"Are you sure you want to recreate node Ids?  You should do this after cloning a theme file", "Yes", "Cancel");
			if (confirm) {
				DungeonEditorHelper._Advanced_RecreateGraphNodeIds();
			}
		}

		void OnRefreshThumbnail() {
			AssetThumbnailCache.Instance.Reset();
		}

        void OnJumpTo_MarkerName(object userdata)
        {
            var markerName = userdata as string;
            if (markerName != null && graphEditor != null)
            {
                graphEditor.FocusCameraOnMarker(markerName, position);
            }
        }

        void OnJumpTo_CenterGraph()
        {
            if (graphEditor != null)
            {
                graphEditor.FocusCameraOnBestFit(position);
            }
        }

        void OnGUI()
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            var guiState = new GUIState(renderer);

            var bounds = new Rect(Vector2.zero, position.size);
            graphEditor.UpdateWidget(uiSystem, bounds);
            graphEditor.Draw(uiSystem, renderer);

            Event e = Event.current;
            switch (e.type)
            {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

					var gameObjects = new List<GameObject>();
					var sprites = new List<Sprite>();

					foreach (var draggedObject in DragAndDrop.objectReferences) {
						if (draggedObject is GameObject) {
							gameObjects.Add(draggedObject as GameObject);
						} 
						else if (draggedObject is Sprite) {
							sprites.Add(draggedObject as Sprite);
						}
					}

					// Build the sprite nodes
					foreach (var sprite in sprites) {
						var node = graphEditor.CreateNode<SpriteNode>(e.mousePosition, uiSystem);
						node.sprite = sprite;
						graphEditor.SelectNode(node, uiSystem);
					}

					// Build the game object nodes
					if (gameObjects.Count > 0) {
						if (gameObjects.Count == 1) {
							// Build a game object node
							var node = graphEditor.CreateNode<GameObjectNode>(e.mousePosition, uiSystem);
							node.Template = gameObjects[0];

							var originalTransform = node.Template.transform;
							node.offset = Matrix4x4.TRS(Vector3.zero, originalTransform.rotation, originalTransform.localScale);

							graphEditor.SelectNode(node, uiSystem);
						}
						else {
							// Build a game object array node
							var node = graphEditor.CreateNode<GameObjectArrayNode>(e.mousePosition, uiSystem);
							node.Templates = gameObjects.ToArray();
							graphEditor.SelectNode(node, uiSystem);
						}
					}
                }
                break;
            }

            guiState.Restore();
            DrawToolbar();

            HandleInput(Event.current);
        }

        void HandleInput(Event e)
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            graphEditor.HandleInput(e, uiSystem);
            if (e.isScrollWheel)
            {
                Repaint();
            }

            switch (e.type)
            {
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                    Repaint();
                    break;
            }
        }

        public void OnDungeonBuiltByUser(Dungeon dungeon)
        {
            if (dungeon == graphEditor.TrackedDungeon)
            {
                graphEditor.UpdateMarkerVisualizer();
            }
        }
        
        public void OnDungeonDestroyedByUser(Dungeon dungeon)
        {
            if (dungeon == graphEditor.TrackedDungeon)
            {
                graphEditor.ClearMarkerVisualizer();
            }
        }
    }
}
