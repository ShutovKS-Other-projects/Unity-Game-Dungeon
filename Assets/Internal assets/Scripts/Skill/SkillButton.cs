using System;
using Player;
using Skill.Enum;
using Skill.SkillTree;
using UnityEngine;
using UnityEngine.UI;


namespace Skill
{
    public class SkillButton
    {
        private Transform _transform;
        private readonly SkillMagicType _skillMagicType;

        private Image _image;
        private Image _backgroundImage;

        public SkillButton(Transform transform, SkillMagic skillMagic, SkillMagicType skillMagicType)
        {
            _transform = transform;
            _skillMagicType = skillMagicType;

            transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                skillMagic.TryUnlockSkill(_skillMagicType);
            });
        }
        
        public void UpdateVisuals()
        {
            Debug.Log($"Update visuals {_skillMagicType}");
            // _image.sprite = _skillMagic.GetSkillSprite(_skillMagicType);
            // _backgroundImage.sprite = _skillMagic.GetSkillBackgroundSprite(_skillMagicType);
        } 
    }
}