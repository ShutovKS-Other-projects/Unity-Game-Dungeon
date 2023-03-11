//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;


namespace DungeonArchitect.Utils
{
	public class Matrix {
		public static Vector3 GetTranslation(ref Matrix4x4 matrix) {
			Vector3 translate;
			translate.x = matrix.m03;
			translate.y = matrix.m13;
			translate.z = matrix.m23;
			return translate;
		}
		
		public static Vector3 GetTranslationDivW(ref Matrix4x4 matrix)
		{
			float w = matrix.m33;
			Vector3 translate;
			translate.x = matrix.m03 / w;
			translate.y = matrix.m13 / w;
			translate.z = matrix.m23 / w;
			return translate;
		}

		public static void SetTranslation(ref Matrix4x4 matrix, Vector3 translate) {
			matrix.m03 = translate.x;
			matrix.m13 = translate.y;
			matrix.m23 = translate.z;
		}

        
		public static void SetTransform(out Matrix4x4 transform, Vector3 position, Quaternion rotation, Vector3 scale) {
			transform = Matrix4x4.TRS(position, rotation, scale);
		}
		
		public static Quaternion GetRotation(ref Matrix4x4 matrix) {
			Vector3 forward;
			forward.x = matrix.m02;
			forward.y = matrix.m12;
			forward.z = matrix.m22;
			
			Vector3 upwards;
			upwards.x = matrix.m01;
			upwards.y = matrix.m11;
			upwards.z = matrix.m21;

            if (forward == Vector3.zero)
            {
                return Quaternion.identity;
            }
			return Quaternion.LookRotation(forward, upwards);
		}

		public static Vector3 GetScale(ref Matrix4x4 matrix) {
            return matrix.lossyScale;
		}
		
		public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale) {
			localPosition = GetTranslation(ref matrix);
			localRotation = GetRotation(ref matrix);
			localScale = GetScale(ref matrix);
		}
		
		public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix) {
			transform.localPosition = GetTranslation(ref matrix);
			transform.localRotation = GetRotation(ref matrix);
			transform.localScale = GetScale(ref matrix);
		}

		public static Matrix4x4 Copy(Matrix4x4 In) {
			return In * Matrix4x4.identity;
		}

		public static Matrix4x4 FromGameTransform(Transform t) {
			return Matrix4x4.TRS(t.position, t.rotation, t.localScale);
		}
	}
}
