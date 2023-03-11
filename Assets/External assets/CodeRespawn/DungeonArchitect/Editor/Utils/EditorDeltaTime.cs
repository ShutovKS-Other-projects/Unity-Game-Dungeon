//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;

namespace DungeonArchitect.Editors
{
    public class EditorDeltaTime
    {
        private double lastUpdateTimestamp = EditorApplication.timeSinceStartup;
        public float DeltaTime { get; set; } = 0;
        
        public void Tick()
        {
            double currentTime = EditorApplication.timeSinceStartup;
            DeltaTime = (float)(currentTime - lastUpdateTimestamp);
            lastUpdateTimestamp = currentTime;
        }
    }
}