using System;
using UnityEngine;

namespace Skills.SkillsBook
{
    public class SkillBookController : MonoBehaviour
    {
        public static SkillBookController Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            UpdateCharacterStats();
            SkillUnlocked += UpdateCharacterStats;
        }

        public event Action SkillUnlocked;
        public void OnSkillUnlocked() => SkillUnlocked!.Invoke();

        [NonSerialized] public int ExtraLife;
        [NonSerialized] public int RestoringLife;
        [NonSerialized] public int HealthBoost;
        [NonSerialized] public int StrengthBoost;
        [NonSerialized] public int FirstStrikePowerUp;
        [NonSerialized] public int DefenseBoost;
        [NonSerialized] public int IncreasingDodgeChance;

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