using UnityEngine;

namespace Mining
{
    public class ManagerMining : MonoBehaviour
    {
        public static ManagerMining Instance;
        public MiningObject miningObjectDefault;
        public MiningObject miningObjectTime;
        
        public int Mining1 => miningObjectDefault.Mining1;
        public int Mining2 => miningObjectDefault.Mining2;
        public int Mining3 => miningObjectDefault.Mining3;
        public int MiningBose1 => miningObjectDefault.MiningBose1;
        public int MiningBose2 => miningObjectDefault.MiningBose2;
        public int MiningBose3 => miningObjectDefault.MiningBose3;

        private void Awake()
        {
            Instance = this;

            miningObjectDefault = Resources.Load<MiningObject>($"ScriptableObject/Mining/MiningObjectDefault");
            miningObjectTime = Resources.Load<MiningObject>($"ScriptableObject/Mining/MiningObjectTime");

            miningObjectDefault.Mining1 += miningObjectTime.Mining1;
            miningObjectDefault.Mining2 += miningObjectTime.Mining2;
            miningObjectDefault.Mining3 += miningObjectTime.Mining3;
            miningObjectDefault.MiningBose1 += miningObjectTime.MiningBose1;
            miningObjectDefault.MiningBose2 += miningObjectTime.MiningBose2;
            miningObjectDefault.MiningBose3 += miningObjectTime.MiningBose3;
            
            miningObjectTime.Clear();
        }
    }
}