using System;
using Skill.Enum;
using UnityEngine.UI;

namespace Skill
{
    [Serializable]
    public class SkillUnlockPath
    {
        public SkillMagicType SkillMagicType;
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