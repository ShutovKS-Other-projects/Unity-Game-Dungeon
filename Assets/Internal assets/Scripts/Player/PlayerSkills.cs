using System;
using Magic;
using Magic.Object;
using Magic.Type;
using UnityEngine;
using Skill;
using Skill.Enum;
using Skill.SkillTree;

namespace Player
{
    public class PlayerSkills : MonoBehaviour
    {
        [SerializeField] public MagicAttackObject MagicAttackObject;
        private SkillMagic _skillMagic;
        private PlayerStatistic _playerStatistic;
        private UISkillTree _uiSkillTree;

        private void Awake()
        {
            _playerStatistic = GetComponent<PlayerStatistic>();

            _skillMagic = new SkillMagic();
            _skillMagic.OnSkillSwitched += PlayerOnSwitched;

            _uiSkillTree = FindObjectOfType<UISkillTree>();
            _uiSkillTree.SetMagicSkills(_skillMagic);
        }

        private void PlayerOnSwitched(object sender, MagicAttackType magicAttackType)
        {
            _playerStatistic.MagicAttackType = magicAttackType;
        }
    }
}