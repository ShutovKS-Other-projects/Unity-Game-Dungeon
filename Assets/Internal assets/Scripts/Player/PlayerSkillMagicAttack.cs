using System;
using Magic.Database;
using Magic.SubMagic;
using Magic.SuperMagic;
using Magic.Type;
using UnityEngine;
using Skill.SkillTree;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerSkillMagicAttack : MonoBehaviour
    {
        private PlayerStatistic _playerStatistic;
        private SkillMagic _skillMagic;
        private UISkillTree _uiSkillTree;

        private void Awake()
        {
            _playerStatistic = GetComponent<PlayerStatistic>();

            _skillMagic = new SkillMagic();
            _skillMagic.OnSkillSwitched += PlayerOnSwitched!;

            _uiSkillTree = FindObjectOfType<UISkillTree>();
            _uiSkillTree.SetMagicSkills(_skillMagic);
        }

        private void PlayerOnSwitched(object sender, MagicAttackType magicAttackType)
        {
            _playerStatistic.MagicAttackType = magicAttackType;
        } 
    }
}