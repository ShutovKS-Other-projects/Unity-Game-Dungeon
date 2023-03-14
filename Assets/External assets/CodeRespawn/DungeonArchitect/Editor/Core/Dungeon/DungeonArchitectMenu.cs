//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.SnapFlow;
using DungeonArchitect.Editors.Flow.Impl;
using DungeonArchitect.Editors.SpatialConstraints;
using UnityEditor;

namespace DungeonArchitect.Editors
{
    public class DungeonArchitectMenu
    {

        //------------------- Create Menu -------------------
        [MenuItem("Dungeon Architect/Create/Dungeon Theme", priority = 100)]
        public static void CreateThemeAssetInBrowser()
        {
            DungeonEditorHelper.CreateThemeAssetInBrowser();
        }

        [MenuItem("Dungeon Architect/Create/Snap Builder/Snap Graph", priority = 1000)]
        public static void CreateDungeonSnapFlowAssetInBrowser()
        {
            DungeonEditorHelper.CreateSnapAssetInBrowser();
        }

        [MenuItem("Dungeon Architect/Create/Snap Builder/Snap Connection", priority = 1000)]
        public static void CreateDungeonSnapFlowConnectionAssetInBrowser()
        {
            DungeonEditorHelper.CreateSnapAssetInBrowser();
        }

        
        [MenuItem("Dungeon Architect/Create/Grid Flow Builder/Grid Flow Graph", priority = 1000)]
        public static void CreateDungeonGridFlowAssetInBrowser()
        {
            DungeonEditorHelper.CreateGridFlowAssetInBrowser();
        }

        [MenuItem("Dungeon Architect/Create/Snap Grid Flow Builder/Snap Grid Flow Graph", priority = 1000)]
        public static void CreateSnapGridFlowGraphAssetInBrowser()
        {
            DungeonEditorHelper.CreateSnapGridFlowGraphAssetInBrowser();
        }
        
        [MenuItem("Dungeon Architect/Create/Snap Grid Flow Builder/Module Bounds", priority = 1000)]
        public static void CreateSnapGridFlowModuleBoundsAssetInBrowser()
        {
            DungeonEditorHelper.CreateSnapGridFlowModuleBoundsAssetInBrowser();
        }
        
        [MenuItem("Dungeon Architect/Create/Snap Grid Flow Builder/Module Database", priority = 1000)]
        public static void CreateSnapGridFlowModuleDatabaseAssetInBrowser()
        {
            DungeonEditorHelper.CreateSnapGridFlowModuleDatabaseAssetInBrowser();
        }

        [MenuItem("Dungeon Architect/Create/Snap Grid Flow Builder/Placeable Marker", priority = 1000)]
        public static void CreateSnapGridFlowPlaceableMarkerAssetInBrowser()
        {
            DungeonEditorHelper.CreateSnapGridFlowPlaceableMarkerAssetInBrowser();
        }

        [MenuItem("Dungeon Architect/Create/Snap Grid Flow Builder/Snap Connection", priority = 1000)]
        public static void CreateSnapGridFlowConnectionAssetInBrowser()
        {
            DungeonEditorHelper.CreateSnapGridFlowConnectionAssetInBrowser();
        }
        
        [MenuItem("Dungeon Architect/Create/Landscape/Landscape Restoration Cache", priority = 1000)]
        public static void CreateDungeonLandscapeRestCacheInBrowser()
        {
            DungeonEditorHelper.CreateDungeonLandscapeRestCacheInBrowser();
        }

        /*
        //------------------- Windows Menu -------------------
        [MenuItem("Dungeon Architect/Windows/Theme Editor", priority = 111)]
        public static void OpenWindow_ThemeEditor()
        {
            DungeonThemeEditorWindow.ShowEditor();
        }

        [MenuItem("Dungeon Architect/Windows/Grid Flow", priority = 112)]
        public static void OpenWindow_GridFlow()
        {
            GridFlowEditorWindow.ShowWindow();
        }

        // TODO: Hide this from the menu
        [MenuItem("Dungeon Architect/Windows/Snap Grid Flow", priority = 113)]
        public static void OpenWindow_SnapGridFlow()
        {
            SnapGridFlowEditorWindow.ShowWindow();
        }

        [MenuItem("Dungeon Architect/Windows/SnapFlow", priority = 114)]
        public static void OpenWindow_SnapFlow()
        {
            SnapEditorWindow.ShowEditor();
        }
        [MenuItem("Dungeon Architect/Windows/Spatial Constraints", priority = 115)]
        public static void OpenWindow_SpatialConstraints()
        {
            SpatialConstraintsEditorWindow.ShowWindow();
        }
        */
    }
}
