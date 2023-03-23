using System;
using UnityEngine;

namespace Mining
{
    [Serializable]
    [CreateAssetMenu(fileName = "new Mining database", menuName = "Mining Database", order = 0)]
    public class MiningObject : ScriptableObject
    {
        public int Mining1;
        public int Mining2;
        public int Mining3;
        
        public int MiningBose1;
        public int MiningBose2;
        public int MiningBose3;
        
        public void Clear()
        {
            Mining1 = 0;
            Mining2 = 0;
            Mining3 = 0;
            MiningBose1 = 0;
            MiningBose2 = 0;
            MiningBose3 = 0;
        }
    }
}