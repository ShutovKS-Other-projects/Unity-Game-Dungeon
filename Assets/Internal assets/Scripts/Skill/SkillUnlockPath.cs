using System;
using Magic.Type;
using Skill.Enum;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Skill
{
    [Serializable]
    public class SkillUnlockPath
    {
        public MagicType magicType;
        public Image[] skillUnlockPathImages;

        public void UpdateVisuals()
        {
            foreach (var skillUnlockPathImage in skillUnlockPathImages)
            {
                // skillUnlockPathImage.sprite = _skillMagic.GetSkillUnlockPathSprite(SkillMagicType);
            }
        }
    }
}