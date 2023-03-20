using System;
using Old.Magic.Type;
using UnityEngine.UI;

namespace Old.Skill
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