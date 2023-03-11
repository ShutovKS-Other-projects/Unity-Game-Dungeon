//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Utils;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.Editors.SpatialConstraints;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for visual nodes
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VisualNode))]
    public abstract class VisualNodeEditor : PlaceableNodeEditor
	{
        SerializedProperty IsStatic;
        SerializedProperty affectsNavigation;
        SerializedProperty useSpatialConstraint;
        InstanceCache instanceCache = new InstanceCache();
        bool foldoutSpatialAdvanced = false;

        public override void OnEnable()
        {
            base.OnEnable();
            drawOffset = true;
			drawAttachments = true;
            IsStatic = sobject.FindProperty("IsStatic");
            affectsNavigation = sobject.FindProperty("affectsNavigation");
            useSpatialConstraint = sobject.FindProperty("useSpatialConstraint");
        }

        protected override void DrawMiscInspectorGUI()
        {
	        base.DrawMiscInspectorGUI();

	        DrawHeader("Misc");
			using (new EditorGUI.IndentLevelScope())
			{
				EditorGUILayout.PropertyField(IsStatic, new GUIContent("Force Static (editor only)"));
				EditorGUILayout.PropertyField(affectsNavigation);
			}

        }
        
        protected override void DrawPostInspectorGUI()
        {
	        DrawHeader("Rules");

            GUILayout.BeginHorizontal();
            GUILayout.Space(INDENT_SPACING);
            GUILayout.BeginVertical();
            
            var meshNode = target as VisualNode;
            DrawRule<SelectorRule>(" Selection Rule", ref meshNode.selectionRuleEnabled, ref meshNode.selectionRuleClassName);
            DrawRule<TransformationRule>(" Transform Rule", ref meshNode.transformRuleEnabled, ref meshNode.transformRuleClassName);

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            GUI.enabled = true;

            DrawSpatialConstraintCategory();
        }

        void DrawSpatialConstraintCategory()
        {
	        DrawHeader("SpatialConstraint");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(useSpatialConstraint);

            var visualNode = target as VisualNode;
            if (visualNode.useSpatialConstraint && visualNode.spatialConstraint != null)
            {
                visualNode.spatialConstraint.rotateToFit = EditorGUILayout.Toggle("Rotate to Fit?", visualNode.spatialConstraint.rotateToFit);
                visualNode.spatialConstraint.applyFitRotation = EditorGUILayout.Toggle("Apply Fit Rotation?", visualNode.spatialConstraint.applyFitRotation);

                foldoutSpatialAdvanced = EditorGUILayout.Foldout(foldoutSpatialAdvanced, "Advanced");
                if (foldoutSpatialAdvanced)
                {
                    EditorGUI.indentLevel++;
                    visualNode.spatialConstraint.checkRelativeToMarkerRotation = EditorGUILayout.Toggle("Check relative to marker rotation?", visualNode.spatialConstraint.checkRelativeToMarkerRotation);
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel++;
                if (GUILayout.Button("Open Spatial Constraints Window"))
                {
	                SpatialConstraintsEditorWindow.ShowWindow();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        string GetTypeName(System.Type type)
        {
            var meta = System.Attribute.GetCustomAttribute(type, typeof(MetaAttribute)) as MetaAttribute;
            if (meta != null)
            {
                return meta.displayText;
            }
            return type.Name;
        }
        

        void DrawRule<T>(string caption, ref bool ruleEnabled, ref string ruleClassName) where T : ScriptableObject
        {
            GUI.enabled = true;
            EditorGUILayout.BeginHorizontal();
            ruleEnabled = EditorGUILayout.ToggleLeft(caption, ruleEnabled);
            GUI.enabled = ruleEnabled; 
            MonoScript script = null;
            if (ruleClassName != null)
            {
                var rule = instanceCache.GetInstance(ruleClassName) as ScriptableObject;
                if (rule != null)
                {
                    script = MonoScript.FromScriptableObject(rule);
                } 
            }
            var oldScript = script;
            script = EditorGUILayout.ObjectField(script, typeof(MonoScript), false) as MonoScript;
            if (oldScript != script && script != null)
            {
	            var ruleType = script.GetClass();
	            if (ruleType.IsSubclassOf(typeof(T)))
	            {
		            ruleClassName = script.GetClass().AssemblyQualifiedName;
	            }
	            else
	            {
		            ruleClassName = null;
	            }
            }
            else if (script == null)
            {
                ruleClassName = null;
            }

            EditorGUILayout.EndHorizontal();
            
            GUI.enabled = true;
        }
    }

    /// <summary>
    /// Renders a visual node
    /// </summary>
    public abstract class VisualNodeRenderer : GraphNodeRenderer
    {
		protected virtual void DrawFrameTexture(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera) {
			DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_GO_NODE_FRAME);
		}

		protected virtual void DrawBackgroundTexture(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera) {
			DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_GO_NODE_BG);
		}

		protected virtual void DrawThumbnail(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera) {
			var thumbObject = GetThumbObject(node);
			var visualNode = node as VisualNode;
			var thumbnailSize = 96 / camera.ZoomLevel;
			if (thumbObject != null)
			{
				Texture texture = AssetThumbnailCache.Instance.GetThumb(thumbObject);
				if (texture != null)
				{
					var positionWorld = new Vector2(12, 12) + visualNode.Position;
					var positionScreen = camera.WorldToScreen(positionWorld);
                    renderer.DrawTexture(new Rect(positionScreen.x, positionScreen.y, thumbnailSize, thumbnailSize), texture);
				}
			}
			else
			{
				DrawTextCentered(renderer, rendererContext, node, camera, "None");
			}
		}

        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            DrawBackgroundTexture(renderer, rendererContext, node, camera);

			DrawThumbnail(renderer, rendererContext, node, camera);

			DrawFrameTexture(renderer, rendererContext, node, camera);

            base.Draw(renderer, rendererContext, node, camera);

            if (node.Selected)
            {
                DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_GO_NODE_SELECTION);
            }
        }

        protected abstract Object GetThumbObject(GraphNode node);
    }
}
