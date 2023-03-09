using System;
using UnityEngine;
using Skill;

namespace Player
{
    public class PlayerSkills : MonoBehaviour
    {
        private PlayerStatistic _playerStatistic;
        public Skills Skills { get; private set; }


        private void Awake()
        {
            _playerStatistic = GetComponent<PlayerStatistic>();
            Skills = new Skills();
            Skills.OnSkillSwitched += PlayerSkills_OnSkillSwitched;
        }

        private void PlayerSkills_OnSkillUnlocked(object sender, Skills.OnSkillUnlockedEventArgs e)
        {
            _playerStatistic.MagicAttackType = e.SkillMagicType switch
            {
                SkillMagicType.Air => SkillMagicType.Air,
                SkillMagicType.Fire => SkillMagicType.Fire,
                SkillMagicType.Water => SkillMagicType.Water,
                SkillMagicType.Earth => SkillMagicType.Earth,
                SkillMagicType.Light => SkillMagicType.Light,
                SkillMagicType.Dark => SkillMagicType.Dark,
                SkillMagicType.Default => SkillMagicType.Default,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void PlayerSkills_OnSkillSwitched(object sender, SkillMagicType e)
        {
            _playerStatistic.MagicAttackType = e switch
            {
                SkillMagicType.Air => SkillMagicType.Air,
                SkillMagicType.Fire => SkillMagicType.Fire,
                SkillMagicType.Water => SkillMagicType.Water,
                SkillMagicType.Earth => SkillMagicType.Earth,
                SkillMagicType.Light => SkillMagicType.Light,
                SkillMagicType.Dark => SkillMagicType.Dark,
                SkillMagicType.Default => SkillMagicType.Default,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public Skills GetSkills()
        {
            return Skills;
        }
    }
}