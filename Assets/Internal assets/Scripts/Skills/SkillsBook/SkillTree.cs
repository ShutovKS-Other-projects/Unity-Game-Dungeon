using System;
using UnityEngine;

namespace Skills.SkillsBook
{
    public class SkillTree : MonoBehaviour
    {
        public static SkillTree Instance;
        private void Awake() => Instance = this;
        
        public event Action SkillUnlocked;
        
        public int skillPoints = 30;

        public void OnSkillUnlocked() => SkillUnlocked!.Invoke();
    }
}