using Old.Magic.Type;
using Old.Skill.SkillTree;
using UnityEngine;
using UnityEngine.UI;

namespace Old.Skill.Magic
{
    public class SkillButtonMagic
    {
        private Transform _transform;
        private readonly MagicType _magicType;

        private Image _image;
        private Image _backgroundImage;

        public SkillButtonMagic(Transform transform, SkillMagic skillMagic, MagicType magicType)
        {
            _transform = transform;
            _magicType = magicType;

            transform.GetComponent<Button>().onClick.AddListener(() => { skillMagic.TryUnlockSkill(_magicType); });
        }

        public void UpdateVisuals()
        {
            Debug.Log($"Update visuals {_magicType}");
            // _image.sprite = _skillMagic.GetSkillSprite(_skillMagicType);
            // _backgroundImage.sprite = _skillMagic.GetSkillBackgroundSprite(_skillMagicType);
        }
    }
}