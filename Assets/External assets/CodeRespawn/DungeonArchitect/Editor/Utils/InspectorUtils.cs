//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Utility functions for drawing UI in the Inspector window
    /// </summary>
    public class InspectorUtils
    {
        /// <summary>
        /// Draws the translation / rotation / scale widgets for a Matrix4x4
        /// </summary>
        /// <param name="caption">The caption to display above the widget</param>
        /// <param name="matrix">The transform matrix to modify</param>
        public static void DrawMatrixProperty(string caption, ref Matrix4x4 matrix)
        {
            Vector3 position;
            Quaternion rotation;
            Vector3 scale;
            Matrix.DecomposeMatrix(ref matrix, out position, out rotation, out scale);
            Vector3 rotationEular = rotation.eulerAngles;

            int precision = 4;
            RoundVector(ref position, precision);
            RoundVector(ref rotationEular, precision);
            RoundVector(ref scale, precision);

            DrawVectorProperty("Position", ref position);
            DrawVectorProperty("Rotation", ref rotationEular);
            DrawVectorProperty("Scale", ref scale);

            if (GUI.changed)
            {
                rotation = Quaternion.Euler(rotationEular);
                matrix = Matrix4x4.TRS(position, rotation, scale);
            }

        }

        /// <summary>
        /// Rounds the Vector to the nearest precision
        /// </summary>
        /// <param name="vector">The vector to round</param>
        /// <param name="precision">The precision in digits</param>
        public static void RoundVector(ref Vector3 vector, int precision)
        {
            vector.x = Round(vector.x, precision);
            vector.y = Round(vector.y, precision);
            vector.z = Round(vector.z, precision);
        }

        /// <summary>
        /// rounds a float to the nearest precision
        /// </summary>
        /// <param name="f">The value to round</param>
        /// <param name="precision">The precision in digits</param>
        /// <returns></returns>
        public static float Round(float f, int precision)
        {
            var multiplier = Mathf.Pow(10, precision);
            return Mathf.Round(f * multiplier) / multiplier;
        }

        /// <summary>
        /// Draws XYZ components of a Vector3 in the inspector window within the same line for better usability and asthetics
        /// </summary>
        /// <param name="caption">The caption to use on the property</param>
        /// <param name="vector">The vector to modify</param>
        public static void DrawVectorProperty(string caption, ref Vector3 vector)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(caption, EditorStyles.label, GUILayout.Width(60));
            GUILayout.Label("X:", EditorStyles.label);
            vector.x = EditorGUILayout.FloatField(vector.x);

            GUILayout.Label("Y:", EditorStyles.label);
            vector.y = EditorGUILayout.FloatField(vector.y);

            GUILayout.Label("Z:", EditorStyles.label);
            vector.z = EditorGUILayout.FloatField(vector.z);
            EditorGUILayout.EndHorizontal();
        }
    }
}
