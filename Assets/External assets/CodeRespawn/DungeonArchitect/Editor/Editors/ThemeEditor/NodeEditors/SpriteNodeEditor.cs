//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for a sprite node
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpriteNode))]
    public class SpriteNodeEditor : VisualNodeEditor
    {
        SerializedProperty sprite;
        SerializedProperty color;
        SerializedProperty materialOverride;
        SerializedProperty sortingLayerName;
        SerializedProperty orderInLayer;

        SerializedProperty collisionType;
        SerializedProperty physicsMaterial;
        SerializedProperty physicsOffset;
        SerializedProperty physicsSize;
        SerializedProperty physicsRadius;

        public override void OnEnable()
        {
            base.OnEnable();
            drawOffset = true;
            drawAttachments = true;
            sprite = sobject.FindProperty("sprite");
            color = sobject.FindProperty("color");
            materialOverride = sobject.FindProperty("materialOverride");
            sortingLayerName = sobject.FindProperty("sortingLayerName");
            orderInLayer = sobject.FindProperty("orderInLayer");

            collisionType = sobject.FindProperty("collisionType");
            physicsMaterial = sobject.FindProperty("physicsMaterial");
            physicsOffset = sobject.FindProperty("physicsOffset");
            physicsSize = sobject.FindProperty("physicsSize");
            physicsRadius = sobject.FindProperty("physicsRadius");
        }

        protected override void DrawPreInspectorGUI()
        {
            DrawHeader("Sprite");
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(sprite);
                EditorGUILayout.PropertyField(color);
                EditorGUILayout.PropertyField(materialOverride);
                EditorGUILayout.PropertyField(sortingLayerName);
                EditorGUILayout.PropertyField(orderInLayer);
            }

            base.DrawPreInspectorGUI();
        }


        protected override void DrawPostInspectorGUI()
        {
            DrawHeader("Physics");

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(collisionType);

                var collsionIndex = collisionType.enumValueIndex;
                var collisionTypeValue = DungeonSpriteCollisionType.None;
                if (System.Enum.IsDefined(typeof(DungeonSpriteCollisionType), collsionIndex))
                {
                    collisionTypeValue = (DungeonSpriteCollisionType) collsionIndex;
                }

                if (collisionTypeValue != DungeonSpriteCollisionType.None)
                {
                    EditorGUILayout.PropertyField(physicsMaterial, new GUIContent("Material"));
                    EditorGUILayout.PropertyField(physicsOffset, new GUIContent("Offset"));
                    if (collisionTypeValue == DungeonSpriteCollisionType.Box)
                    {
                        EditorGUILayout.PropertyField(physicsSize, new GUIContent("Size"));
                    }
                    else if (collisionTypeValue == DungeonSpriteCollisionType.Circle)
                    {
                        EditorGUILayout.PropertyField(physicsRadius, new GUIContent("Radius"));
                    }
                }
            }
            
            base.DrawPostInspectorGUI();
        }
        
        protected override string GetNodeTitle()
        {
            return "Sprite Node";
        }
    }

    /// <summary>
    /// Renders a sprite node
    /// </summary>
    public class SpriteNodeRenderer : VisualNodeRenderer
    {
        protected override Object GetThumbObject(GraphNode node)
        {
            var spriteNode = node as SpriteNode;
            if (spriteNode == null) return null;
            return spriteNode.sprite;
        }
    }

}
