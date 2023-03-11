//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect {

    [System.Serializable]
    public struct DebugTextItem {
        public string message;
        public Vector3 position;
        public Color color;
    }

    public class DebugText3D : MonoBehaviour {
        [HideInInspector]
        public DebugTextItem[] items;
    }
}