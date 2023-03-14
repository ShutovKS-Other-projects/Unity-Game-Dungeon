//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using DungeonArchitect.Splatmap;
using DungeonArchitect.UI;

namespace DungeonArchitect.Editors
{
    public class DungeonPropertyEditorHook
    {
        public delegate void OnDungeonBuilt(Dungeon dungeon);
        public event OnDungeonBuilt DungeonBuilt;

        private DungeonPropertyEditorHook() { }
        private static DungeonPropertyEditorHook instance;
        public static DungeonPropertyEditorHook Get()
        {
            if (instance == null)
            {
                instance = new DungeonPropertyEditorHook();
            }
            return instance;
        }
        public static void NotifyDungeonBuilt(Dungeon dungeon)
        {
            if (Get().DungeonBuilt != null)
            {
                Get().DungeonBuilt.Invoke(dungeon);
            }
        }
    }
    /// <summary>
    /// Custom property editor for the dungeon game object
    /// </summary>
    [CustomEditor(typeof(Dungeon))]
	public class DungeonPropertyEditor : Editor
    {
	    private Texture2D iconDiscord;
	    private Texture2D iconDocs;
	    
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			if (GUILayout.Button ("Build Dungeon")) {
				BuildDungeon();
			}
			if (GUILayout.Button ("Destroy Dungeon")) {
				DestroyDungeon();
			}
			
			EditorGUILayout.Separator();

			if (iconDiscord == null)
			{
				iconDiscord = Resources.Load(UIResourceLookup.ICON_DISCORD_16x, typeof(Texture2D)) as Texture2D;
			}

			if (iconDocs == null)
			{
				iconDocs = Resources.Load(UIResourceLookup.ICON_DOCS_16x, typeof(Texture2D)) as Texture2D;
			}

			GUILayout.Label("Help / Support", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent(" Discord Support", iconDiscord)))
			{
				ExternalLinks.LaunchUrl(ExternalLinks.DiscordInvite);
			}
			if (GUILayout.Button(new GUIContent("Documentation", iconDocs)))
			{
				ExternalLinks.LaunchUrl(ExternalLinks.Documentation);
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button(new GUIContent("Launch Pad")))
			{
				OpenLaunchPad();
			}
		}

		void OpenLaunchPad()
		{
			System.Type launchPadType = Type.GetType("DungeonArchitect.Editors.LaunchPad.LaunchPadWindow, DungeonArchitect.LaunchPad");
			if (launchPadType != null)
			{
				var window = EditorWindow.GetWindow(launchPadType);
				if (window != null)
				{
					// Call the init function
					foreach (var methodInfo in launchPadType.GetMethods())
					{
						if (methodInfo.Name == "Init")
						{
							methodInfo.Invoke(window, new object[] { });
						}
					}

					window.Show();
				}
			}
			/*
			// Search all assemblies
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				type = asm.GetType(typeName);
				if (type != null)
				{
					break;
				}
			}
			*/
		}
		
        protected virtual void OnEnable()
        {
            //EditorApplication.update += EditorUpdate;
        }
        
        protected virtual void OnDisable()
        {
            //EditorApplication.update -= EditorUpdate;
        }

        void EditorUpdate()
        {
            var dungeon = target as Dungeon;
            dungeon.Update();
        }
        
        void BuildDungeon() {
            // Make sure we have a theme defined
            bool valid = false;
			Dungeon dungeon = target as Dungeon;
			if (dungeon != null) {
				if (HasValidThemes(dungeon)) {
                    var config = dungeon.Config;
                    if (config != null)
                    {
                        string configErrorMessage = "";
                        if (config.HasValidConfig(ref configErrorMessage))
                        {
                            valid = true;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Dungeon Architect", configErrorMessage, "Ok");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Dungeon Architect", "Missing dungeon config component in dungeon game object", "Ok");
                    }
				} 
				else {
					//Highlighter.Highlight ("Inspector", "Dungeon Themes");

					// Notify the user that at least one theme needs to be set
					EditorUtility.DisplayDialog("Dungeon Architect", "Please assign at least one Dungeon Theme before building", "Ok");
				}
			}

            if (valid)
            {
                // Create the splat maps for this dungeon, if necessary
                var splatComponent = dungeon.GetComponent<DungeonSplatmap>();
                SplatmapPropertyEditor.CreateSplatMapAsset(splatComponent);

                // Build the dungeon
                //Undo.RecordObjects(new Object[] { dungeon, dungeon.ActiveModel }, "Dungeon Built");
                dungeon.Build(new EditorDungeonSceneObjectInstantiator());
                DungeonEditorHelper.MarkSceneDirty();
                DungeonPropertyEditorHook.NotifyDungeonBuilt(dungeon);

                // Mark the splatmaps as dirty
                if (splatComponent != null && splatComponent.splatmap != null)
                {
                    EditorUtility.SetDirty(splatComponent.splatmap);
                }
                
                // Notify the theme editor that the dungeon was rebuilt manually from the editor
                var themeEditorWindow = DungeonEditorHelper.GetWindowIfOpen<DungeonThemeEditorWindow>();
                if (themeEditorWindow != null)
                {
	                themeEditorWindow.OnDungeonBuiltByUser(dungeon);
                }
                
                var model = dungeon.GetComponent<DungeonModel>();
                if (model != null)
                {
	                EditorUtility.SetDirty(model);
                }
            }
		}

		IEnumerator StopHighlighter() {
			yield return new WaitForSeconds(2);
			Highlighter.Stop();
		}

		void DestroyDungeon() {
			Dungeon dungeon = target as Dungeon;
            if (dungeon != null)
            {
                //Undo.RecordObjects(new Object[] { dungeon, dungeon.ActiveModel }, "Dungeon Destroyed");
                dungeon.DestroyDungeon();
                EditorUtility.SetDirty(dungeon.gameObject);
                
                // Notify the theme editor that the dungeon was rebuilt manually from the editor
                var themeEditorWindow = DungeonEditorHelper.GetWindowIfOpen<DungeonThemeEditorWindow>();
                if (themeEditorWindow != null)
                {
	                themeEditorWindow.OnDungeonDestroyedByUser(dungeon);
                }
                
                var model = dungeon.GetComponent<DungeonModel>();
                if (model != null)
                {
	                EditorUtility.SetDirty(model);
                }
            }
		}

        bool HasValidThemes(Dungeon dungeon) {
            var builder = dungeon.gameObject.GetComponent<DungeonBuilder>();
            if (builder != null && !builder.IsThemingSupported())
            {
                // Theming is not supported in this builder. empty theme configuration would do
                return true;
            }

            if (dungeon.dungeonThemes == null) return false;
			foreach (var theme in dungeon.dungeonThemes) {
				if (theme != null) {
					return true;
				}
			}
			return false;
		}

	}
}
