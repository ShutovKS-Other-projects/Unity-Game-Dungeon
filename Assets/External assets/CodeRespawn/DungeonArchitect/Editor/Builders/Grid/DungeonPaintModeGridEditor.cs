//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;
using DMathUtils = DungeonArchitect.Utils.MathUtils;
using DungeonArchitect.Builders.Grid;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for the Paint model game object
    /// </summary>
    [CustomEditor(typeof(DungeonPaintModeGrid))]
    public class DungeonPaintModeGridEditor : DungeonPaintModeEditor
    {
        Vector3 cursorPosition;
        bool modeDelete = false;

        private readonly IDungeonSceneObjectInstantiator sceneObjectInstantiator = new EditorDungeonSceneObjectInstantiator();

        Vector3 SnapToGrid(Vector3 value)
        {
            var result = value;
            var mode = target as DungeonPaintModeGrid;
            var config = mode.GetDungeonConfig() as GridDungeonConfig;
            if (config == null)
            {
                return result;
            }
            var gridSize = config.GridCellSize;
            result.x = Mathf.FloorToInt(value.x / gridSize.x) * gridSize.x;
            result.y = mode.cursorLogicalHeight * gridSize.y;
            result.z = Mathf.FloorToInt(value.z / gridSize.z) * gridSize.z;
            return result;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Clear Paint Data"))
            {
                ClearPaintData();
            }
        }

		void UpdateCursorPosition(DungeonPaintModeGrid mode) {
			float distance;
			var e = Event.current;
			Plane plane;
			if (mode.mode2D) {
				var planePoint = Vector3.zero;
				plane = new Plane(new Vector3(0, 0, 1), planePoint);
			} else {
				var cursorHeight = mode.GetCursorHeight();
				var planePoint = new Vector3(0, cursorHeight, 0);
				plane = new Plane(Vector3.up, planePoint);
			}

			var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
			if (plane.Raycast(ray, out distance))
			{
				var hitpoint = ray.GetPoint(distance);
				if (mode.mode2D) {
					hitpoint = DMathUtils.FlipYZ(hitpoint);
				}
				hitpoint = SnapToGrid(hitpoint);
				cursorPosition = hitpoint;

				if (!mode.mode2D) {
					if (e.type == EventType.ScrollWheel)
					{
						var delta = (int)Mathf.Sign(e.delta.y);
						mode.SetElevationDelta(delta);
					}
				}
			}
		}


        protected override void SceneGUI(SceneView sceneview)
        {
            var e = Event.current;
            var mode = target as DungeonPaintModeGrid;
			UpdateCursorPosition(mode);
            modeDelete = e.shift;

            int buttonId = 0;
			if (e.type == EventType.MouseDown && e.button == buttonId)
            {
                ProcessPaintRequest(e);
                e.Use();
            }
            else if (e.type == EventType.MouseUp && e.button == buttonId)
            {
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && e.button == buttonId)
            {
                ProcessPaintRequest(e);
                e.Use();
            }
            else if (e.type == EventType.ScrollWheel && !mode.mode2D)
            {
                e.Use();
            }
            else if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            }

			var config = mode.GetDungeonConfig() as GridDungeonConfig;
			var model = mode.GetDungeonModelGrid() as GridDungeonModel;

			DrawOverlayData(config, model, mode.mode2D);
			DrawCursor(config, mode.mode2D, mode.brushSize);

            SceneView.RepaintAll();
        }

        void ClearPaintData()
        {
            var mode = target as DungeonPaintMode;
            if (mode != null)
            {
	            var toolData = mode.GetToolData() as GridDungeonToolData;
	            var dungeon = mode.GetDungeon();

	            if (toolData != null)
	            {
		            var modified = toolData.ClearToolOverlayData();
		            if (modified)
		            {
			            EditorUtility.SetDirty(toolData);
			            dungeon.Build(sceneObjectInstantiator);
		            }
	            }
            }
        }
        
        void ProcessPaintRequest(Event e)
        {
            var mode = target as DungeonPaintModeGrid;
            if (mode == null) return;
            
            var toolData = mode.GetToolData() as GridDungeonToolData;
            if (toolData == null) return;
            
            var config = mode.GetDungeonConfig() as GridDungeonConfig;
            if (config == null) return;
            
            var gridSize = config.GridCellSize;
            var cursorGridPosition = new IntVector(
                Mathf.RoundToInt(cursorPosition.x / gridSize.x),
                Mathf.RoundToInt(cursorPosition.y / gridSize.y),
                Mathf.RoundToInt(cursorPosition.z / gridSize.z));
            var dungeon = mode.GetDungeon();
			
			var cursorSize = Mathf.Max (1, mode.brushSize);
			for (int dx = 0; dx < cursorSize; dx++) {
				for (int dz = 0; dz < cursorSize; dz++) {
					var position = cursorGridPosition + new IntVector(dx, 0, dz);
					if (modeDelete)
					{
						var stateModified = toolData.RemovePaintCell(position);
                        if (stateModified) {
	                        dungeon.Build(sceneObjectInstantiator);
	                        EditorUtility.SetDirty(toolData);
                            //Undo.RecordObject(toolData, "Delete Painted Cell");
                        }
					}
					else
					{
                        var stateModified = toolData.AddPaintCell(position);
                        if (stateModified)
                        {
	                        dungeon.Build(sceneObjectInstantiator);
	                        EditorUtility.SetDirty(toolData);
                            //Undo.RecordObject(toolData, "Add Painted Cell");
                        }
					}
				}
			}

            EditorUtility.SetDirty(dungeon.gameObject);
        }

        void DrawCursor(GridDungeonConfig config, bool mode2D, int size)
        {
			size = Mathf.Max (1, size);
            var cursorSize = config.GridCellSize * size;
			DrawRect(cursorPosition, cursorSize, Color.red, 0.25f, 0.8f, mode2D);
        }

		void DrawRect(Vector3 position, Vector3 size, Color color, float faceOpacity, float outlineOpacity, bool mode2D)
        {
            var verts = new Vector3[] {
				position,
				position + new Vector3(size.x, 0, 0),
				position + new Vector3(size.x, 0, size.z),
				position + new Vector3(0, 0, size.z)
			};
			if (mode2D) {
				for(int i = 0; i < verts.Length; i++) {
					verts[i] = DMathUtils.FlipYZ(verts[i]);
				}
			}
            Color faceColor = new Color(color.r, color.g, color.b, faceOpacity);
            Color outlineColor = new Color(color.r, color.g, color.b, outlineOpacity);
            Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);
            
        }

		void DrawOverlayData(GridDungeonConfig config, GridDungeonModel model, bool mode2D)
        {
            var mode = target as DungeonPaintModeGrid;
            var opacity = mode.overlayOpacity;
            var gridSize = config.GridCellSize;
            var cellColorProcedural = Color.blue;
            var cellColorUserDefined = Color.cyan;

            // Visualize the user defined cells defined by the paint tool
            foreach (var cell in model.Cells)
            {
                var size = Vector3.Scale(DMathUtils.ToVector3(cell.Bounds.Size), gridSize);
                var location = Vector3.Scale(DMathUtils.ToVector3(cell.Bounds.Location), gridSize);
                var color = cell.UserDefined ? cellColorUserDefined : cellColorProcedural;
                DrawRect(location, size, color, opacity, 0.3f, mode2D);
            }
        }
    }
}