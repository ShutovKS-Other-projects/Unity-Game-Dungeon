using System;
using Magic.Type;
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
        private readonly MagicAttackType _magicAttackType;

        private Image _image;
        private Image _backgroundImage;

        public SkillButton(Transform transform, SkillMagic skillMagic, MagicAttackType magicAttackType)
        {
            _transform = transform;
            _magicAttackType = magicAttackType;

            transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                skillMagic.TryUnlockSkill(_magicAttackType);
            });
        }
        
        public void UpdateVisuals()
        {
            Debug.Log($"Update visuals {_magicAttackType}");
            // _image.sprite = _skillMagic.GetSkillSprite(_skillMagicType);
            // _backgroundImage.sprite = _skillMagic.GetSkillBackgroundSprite(_skillMagicType);
        } 
    }
}