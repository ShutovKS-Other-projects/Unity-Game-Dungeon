using System;
using Player;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;


namespace Skill
{
    public class SkillButtons : MonoBehaviour
    {
        private PlayerSkills _playerSkills;
        private Skills _skills;

        private void Start()
        {
            _playerSkills = FindObjectOfType<PlayerSkills>();
            _skills = _playerSkills.Skills;

            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var button = child.GetComponent<Button>();
                var i1 = i;
                button.onClick.AddListener(() =>
                {
                    _skills.TryUnlockSkill((SkillMagicType)i1);
                    Debug.Log($"Skill {(SkillMagicType)i1} unlocked");
                });
            }
        }

        public void SetPlayerSkills(Skills skills)
        {
            _skills = skills;
        }
    }
}