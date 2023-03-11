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
        [FormerlySerializedAs("SkillMagicType")] public MagicAttackType magicAttackType;
        public Image[] SkillUnlockPathImages;

        public void UpdateVisuals()
        {
            foreach (var skillUnlockPathImage in SkillUnlockPathImages)
            {
                
                // skillUnlockPathImage.sprite = _skillMagic.GetSkillUnlockPathSprite(SkillMagicType);
            }
        }
    }
}