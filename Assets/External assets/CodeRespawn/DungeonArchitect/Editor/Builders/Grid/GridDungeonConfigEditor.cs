//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;
using DungeonArchitect.Builders.Grid;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for the grid based dungeon configuration
    /// </summary>
	[CustomEditor(typeof(GridDungeonConfig))]
	public class GridDungeonConfigEditor : Editor {
		SerializedObject sobject;
		SerializedProperty Seed;
		SerializedProperty NumCells;
		SerializedProperty MinCellSize;
		SerializedProperty MaxCellSize;
		SerializedProperty RoomAreaThreshold;
		//SerializedProperty FloorHeight;
		SerializedProperty RoomAspectDelta;
		SerializedProperty CorridorWidth;
		SerializedProperty InitialRoomRadius;
		SerializedProperty SpanningTreeLoopProbability;
        SerializedProperty StairConnectionTollerance;
        SerializedProperty HeightVariationProbability;
		SerializedProperty NormalMean;
		SerializedProperty NormalStd;
		SerializedProperty GridCellSize;
		SerializedProperty MaxAllowedStairHeight;
		SerializedProperty Mode2D;
        SerializedProperty DoorProximitySteps;
        
        SerializedProperty UseFastCellDistribution;
        SerializedProperty CellDistributionWidth;
        SerializedProperty CellDistributionLength;
        SerializedProperty WallLayoutType;

        public void OnEnable() {
			sobject = new SerializedObject(target);
			Seed = sobject.FindProperty("Seed");
			NumCells = sobject.FindProperty("NumCells");
			MinCellSize = sobject.FindProperty("MinCellSize");
			MaxCellSize = sobject.FindProperty("MaxCellSize");
			RoomAreaThreshold = sobject.FindProperty("RoomAreaThreshold");
			//FloorHeight = sobject.FindProperty("FloorHeight");
			RoomAspectDelta = sobject.FindProperty("RoomAspectDelta");
            CorridorWidth = sobject.FindProperty("CorridorWidth");
			InitialRoomRadius = sobject.FindProperty("InitialRoomRadius");
			SpanningTreeLoopProbability = sobject.FindProperty("SpanningTreeLoopProbability");
			StairConnectionTollerance = sobject.FindProperty("StairConnectionTollerance");
			HeightVariationProbability = sobject.FindProperty("HeightVariationProbability");
			NormalMean = sobject.FindProperty("NormalMean");
			NormalStd = sobject.FindProperty("NormalStd");
			GridCellSize = sobject.FindProperty("GridCellSize");
			MaxAllowedStairHeight = sobject.FindProperty("MaxAllowedStairHeight");
            Mode2D = sobject.FindProperty("Mode2D");
            DoorProximitySteps = sobject.FindProperty("DoorProximitySteps");


            UseFastCellDistribution = sobject.FindProperty("UseFastCellDistribution");
            CellDistributionWidth = sobject.FindProperty("CellDistributionWidth");
            CellDistributionLength = sobject.FindProperty("CellDistributionLength");
            
            WallLayoutType = sobject.FindProperty("WallLayoutType");

        }

		public override void OnInspectorGUI()
		{
			sobject.Update();
			GUILayout.Label("Core Config", EditorStyles.boldLabel);
			// Core
			GUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(Seed);
			if (GUILayout.Button("R", GUILayout.Width(20), GUILayout.MaxHeight(15))) {
				RandomizeSeed();
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(NumCells);
			EditorGUILayout.PropertyField(GridCellSize);
            EditorGUILayout.Space();

            // Cell dimensions
            GUILayout.Label("Cell Dimensions", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(MinCellSize);
			EditorGUILayout.PropertyField(MaxCellSize);
			EditorGUILayout.PropertyField(RoomAreaThreshold);
			EditorGUILayout.PropertyField(RoomAspectDelta);
			EditorGUILayout.PropertyField(CorridorWidth);
            EditorGUILayout.Space();

            // Height variations
            GUILayout.Label("Height Variations", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(HeightVariationProbability);
			EditorGUILayout.PropertyField(MaxAllowedStairHeight);
			EditorGUILayout.PropertyField(StairConnectionTollerance);
			EditorGUILayout.PropertyField(SpanningTreeLoopProbability);
            EditorGUILayout.Space();

            // Misc
            GUILayout.Label("Misc", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(Mode2D);
			EditorGUILayout.PropertyField(NormalMean);
			EditorGUILayout.PropertyField(NormalStd);
			EditorGUILayout.PropertyField(InitialRoomRadius);
            EditorGUILayout.PropertyField(DoorProximitySteps);
            EditorGUILayout.Space();

            GUILayout.Label("Experimental", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(UseFastCellDistribution);
            EditorGUILayout.PropertyField(CellDistributionWidth);
            EditorGUILayout.PropertyField(CellDistributionLength);

            EditorGUILayout.PropertyField(WallLayoutType);

            EditorGUILayout.Space();

            //EditorGUILayout.PropertyField(FloorHeight);

            sobject.ApplyModifiedProperties();
		}

		void RandomizeSeed() {
			Seed.intValue = Mathf.RoundToInt(Random.value * int.MaxValue);
		}
	}

}
