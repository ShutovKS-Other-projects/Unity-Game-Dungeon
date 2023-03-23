using System;
using UnityEngine;

namespace Mining
{
    public class ManagerMining : MonoBehaviour
    {
        
        #region Singleton
        
        public static ManagerMining Instance;
        public MiningObject miningObjectDefault;
        public MiningObject miningObjectTime;

        #endregion
        
        #region Mining
        
        public int Mining1 
        {
            get => miningObjectDefault.Mining1;
            set => miningObjectDefault.Mining1 = value;
        }
        public int Mining2 
        {
            get => miningObjectDefault.Mining2;
            set => miningObjectDefault.Mining2 = value;
        }
        public int Mining3 
        {
            get => miningObjectDefault.Mining3;
            set => miningObjectDefault.Mining3 = value;
        }
        public int MiningBose1 
        {
            get => miningObjectDefault.MiningBose1;
            set => miningObjectDefault.MiningBose1 = value;
        }
        public int MiningBose2 
        {
            get => miningObjectDefault.MiningBose2;
            set => miningObjectDefault.MiningBose2 = value;
        }
        public int MiningBose3 
        {
            get => miningObjectDefault.MiningBose3;
            set => miningObjectDefault.MiningBose3 = value;
        }
        
        #endregion
        
        #region Unity Methods
        
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
            OnMiningChanged();
        }
        
        #endregion
        
        public event Action MiningChanged;

        public virtual void OnMiningChanged()
        {
            MiningChanged?.Invoke();
        }
    }
}