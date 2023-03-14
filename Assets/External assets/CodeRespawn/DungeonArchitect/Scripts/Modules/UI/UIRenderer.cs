//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
namespace DungeonArchitect.UI
{
    public interface UIRenderer
    {
        void BeginGroup(Rect bounds);
        void EndGroup();
        bool Button(Rect bounds, GUIContent content, GUIStyle style);
        bool Button(Rect bounds, string text, GUIStyle style);
        bool Button(Rect bounds, string text);
        void Box(Rect bounds, string text);
        void Box(Rect bounds, GUIContent content);
        void Box(Rect bounds, GUIContent content, GUIStyle style);
        void Label(Rect bounds, string text, GUIStyle style);
        void Label(Rect bounds, GUIContent content, GUIStyle style);
        Vector2 BeginScrollView(Rect bounds, Vector2 scrollPosition, Rect viewRect);
        void DrawTexture(Rect bounds, Texture texture);
        void DrawTexture(Rect bounds, Texture texture, ScaleMode scaleMode, bool alphaBlend, Color color);
        void EndScrollView(bool handleScrollWheel);
        void BeginGUI();
        void EndGUI();
        void DrawLine(Vector3 v0, Vector3 v1);
        void DrawLine(Color color, Vector3 v0, Vector3 v1);
        void DrawPolyLine(params Vector3[] points);
        void DrawPolyLine(Color color, params Vector3[] points);
        void DrawAAPolyLine(float thickness, params Vector3[] points);
        void DrawAAPolyLine(float thickness, Color color, params Vector3[] points);
        void DrawAAPolyLine(Texture2D texture, float thickness, params Vector3[] points);
        void DrawAAPolyLine(Texture2D texture, float thickness, Color color, params Vector3[] points);
        void DrawBezier(Vector3 startPos, Vector3 endPos, Vector3 startTangent, Vector3 endTangent, Color lineColor, Texture2D texture, float lineThickness);
        void DrawAAConvexPolygon(params Vector3[] points);
        void DrawAAConvexPolygon(Color color, params Vector3[] points);
        void DrawRect(Rect bounds, Color color);
        void AddCursorRect(Rect bounds, UICursorType cursorType);
        object GetResource<T>(string path);
        Color color { get; set; }
        Color backgroundColor { get; set; }

        UIStyleManager StyleManager { get; }
    }
}
