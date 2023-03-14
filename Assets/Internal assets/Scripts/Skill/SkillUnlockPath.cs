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
        [FormerlySerializedAs("magicAttackType")] [FormerlySerializedAs("SkillMagicType")] public MagicType magicType;
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