using System;
using UnityEngine;
using Skill;
using Skill.Enum;
using Skill.SkillTree;

namespace Player
{
    public class PlayerSkills : MonoBehaviour
    {
        private SkillMagic _skillMagic;
        private PlayerStatistic _playerStatistic;
        private UISkillTree _uiSkillTree;

        private void Awake()
        {
            _playerStatistic = GetComponent<PlayerStatistic>();

            _skillMagic = new SkillMagic();
            _skillMagic.OnSkillSwitched += PlayerSkills_OnSkillSwitched;

            _uiSkillTree = FindObjectOfType<UISkillTree>();
            _uiSkillTree.SetMagicSkills(_skillMagic);
        }

        private void PlayerSkills_OnSkillSwitched(object sender, SkillMagicType skillMagicType)
        {
            _playerStatistic.MagicAttackType = skillMagicType;
        }
    }
}