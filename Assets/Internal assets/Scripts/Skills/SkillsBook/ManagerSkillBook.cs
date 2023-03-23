using System;
using UnityEngine;

namespace Skills.SkillsBook
{
    public class ManagerSkillBook : MonoBehaviour
    {
        public static ManagerSkillBook Instance;
        private void Awake()
        {
            Instance = this;
            UpdateCharacterStats();
            SkillUnlocked += UpdateCharacterStats;
        }

        public event Action SkillUnlocked;
        public void OnSkillUnlocked() => SkillUnlocked!.Invoke();
        
        public int ExtraLife;
        public int RestoringLife;
        public int HealthBoost;
        public int StrengthBoost;
        public int FirstStrikePowerUp;
        public int DefenseBoost;
        public int IncreasingDodgeChance;
        
        public int skillPoints = 30;
        
        private void UpdateCharacterStats()
        {
            ExtraLife = 0;
            RestoringLife = 0;
            HealthBoost = 0;
            StrengthBoost = 0;
            FirstStrikePowerUp = 0;
            DefenseBoost = 0;
            IncreasingDodgeChance = 0;
            
            foreach (var skill in FindObjectsOfType<Skill>())
            {
                var (skillType, buff) = skill.GetSkillTypeAndBuff();
                switch (skillType)
                {
                    case SkillType.ExtraLife:
                        ExtraLife += buff;
                        break;
                    case SkillType.RestoringLife:
                        RestoringLife += buff;
                        break;
                    case SkillType.HealthBoost:
                        HealthBoost += buff;
                        break;
                    case SkillType.StrengthBoost:
                        StrengthBoost += buff;
                        break;
                    case SkillType.FirstStrikePowerUp:
                        FirstStrikePowerUp += buff;
                        break;
                    case SkillType.DefenseBoost:
                        DefenseBoost += buff;
                        break;
                    case SkillType.IncreasingDodgeChance:
                        IncreasingDodgeChance += buff;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}