//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.UI.Impl.UnityEditor
{
    public class UnityEditorUIRenderer : UIRenderer
    {
        public void BeginGroup(Rect bounds)
        {
            GUI.BeginGroup(bounds);
        }
        public void EndGroup()
        {
            GUI.EndGroup();
        }
        public bool Button(Rect bounds, GUIContent content, GUIStyle style)
        {
            return GUI.Button(bounds, content, style);
        }

        public bool Button(Rect bounds, string text, GUIStyle style)
        {
            return GUI.Button(bounds, text, style);
        }

        public bool Button(Rect bounds, string text)
        {
            return GUI.Button(bounds, text);
        }

        public void Box(Rect bounds, string text)
        {
            GUI.Box(bounds, text);
        }

        public void Box(Rect bounds, GUIContent content)
        {
            GUI.Box(bounds, content);
        }

        public void Box(Rect bounds, GUIContent content, GUIStyle style)
        {
            GUI.Box(bounds, content, style);
        }

        public void Label(Rect bounds, string text, GUIStyle style)
        {
            GUI.Label(bounds, text, style);
        }

        public void Label(Rect bounds, GUIContent content, GUIStyle style)
        {
            GUI.Label(bounds, content, style);
        }

        public Vector2 BeginScrollView(Rect bounds, Vector2 scrollPosition, Rect viewRect)
        {
            return GUI.BeginScrollView(bounds, scrollPosition, viewRect);
        }

        public void DrawTexture(Rect bounds, Texture texture)
        {
            if (texture != null)
            {
                GUI.DrawTexture(bounds, texture);
            }
        }

        public void DrawTexture(Rect bounds, Texture texture, ScaleMode scaleMode, bool alphaBlend, Color color)
        {
            if (texture != null)
            {
                GUI.DrawTexture(bounds, texture, scaleMode, alphaBlend, 1.0f, color, 0, 0);
            }
        }

        public void EndScrollView(bool handleScrollWheel)
        {
            GUI.EndScrollView(handleScrollWheel);
        }

        public void BeginGUI()
        {
            Handles.BeginGUI();
        }

        public void EndGUI()
        {
            Handles.EndGUI();
        }

        public void DrawLine(Vector3 v0, Vector3 v1)
        {
            Handles.DrawLine(v0, v1);
        }

        public void DrawLine(Color color, Vector3 v0, Vector3 v1)
        {
            Handles.color = color;
            Handles.DrawLine(v0, v1);
        }

        public void DrawPolyLine(params Vector3[] points)
        {
            Handles.DrawPolyLine(points);
        }

        public void DrawPolyLine(Color color, params Vector3[] points)
        {
            Handles.color = color;
            Handles.DrawPolyLine(points);
        }

        public void DrawAAPolyLine(float thickness, params Vector3[] points)
        {
            Handles.DrawAAPolyLine(thickness, points);
        }

        public void DrawAAPolyLine(float thickness, Color color, params Vector3[] points)
        {
            Handles.color = color;
            Handles.DrawAAPolyLine(thickness, points);
        }

        public void DrawAAPolyLine(Texture2D texture, float thickness, params Vector3[] points)
        {
            Handles.DrawAAPolyLine(texture, thickness, points);
        }

        public void DrawAAPolyLine(Texture2D texture, float thickness, Color color, params Vector3[] points)
        {
            Handles.color = color;
            Handles.DrawAAPolyLine(texture, thickness, points);
        }

        public void DrawBezier(Vector3 startPos, Vector3 endPos, Vector3 startTangent, Vector3 endTangent, Color lineColor, Texture2D texture, float lineThickness)
        {
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, lineColor, texture, lineThickness);
        }

        public void DrawAAConvexPolygon(params Vector3[] points)
        {
            Handles.DrawAAConvexPolygon(points);
        }

        public void DrawAAConvexPolygon(Color color, params Vector3[] points)
        {
            Handles.color = color;
            Handles.DrawAAConvexPolygon(points);
        }

        public void DrawRect(Rect bounds, Color color)
        {
            EditorGUI.DrawRect(bounds, color);
        }

        public void AddCursorRect(Rect bounds, UICursorType cursorType)
        {
            MouseCursor cursor;
            if (cursorType == UICursorType.ResizeHorizontal) cursor = MouseCursor.ResizeHorizontal;
            else if (cursorType == UICursorType.ResizeVertical) cursor = MouseCursor.ResizeVertical;
            else if (cursorType == UICursorType.Link) cursor = MouseCursor.Link;
            else cursor = MouseCursor.Arrow;

            EditorGUIUtility.AddCursorRect(bounds, cursor);
        }

        public Color color
        {
            get
            {
                return GUI.color;
            }
            set
            {
                GUI.color = value;
            }
        }

        public Color backgroundColor
        {
            get
            {
                return GUI.backgroundColor;
            }
            set
            {
                GUI.backgroundColor = value;
            }
        }

        private UIStyleManager styleManager = new UnityEditorUIStyleManager();
        public UIStyleManager StyleManager
        {
            get { return styleManager; }
        }

        Dictionary<string, Object> resources = new Dictionary<string, Object>();
        public object GetResource<T>(string path)
        {
            if (resources.ContainsKey(path) && resources[path] == null)
            {
                resources.Remove(path);
            }

            if (!resources.ContainsKey(path))
            {
                Object resource = Resources.Load(path, typeof(T)) as Object;
                resources.Add(path, resource);
            }

            return resources[path];
        }
    }
}
