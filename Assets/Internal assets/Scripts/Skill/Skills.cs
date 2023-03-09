using System;
using System.Collections.Generic;

namespace Skill
{
    public class Skills
    {
        public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;
        public event EventHandler<SkillMagicType> OnSkillSwitched;

        public class OnSkillUnlockedEventArgs : EventArgs
        {
            public SkillMagicType SkillMagicType { get; set; }
        }

        public List<SkillMagicType> UnlockedSkillsTypeList;

        public Skills()
        {
            UnlockedSkillsTypeList = new List<SkillMagicType>();
        }


        private void UnlockSkill(SkillMagicType skillMagicType)
        {
            if (!UnlockedSkillsTypeList.Contains(skillMagicType))
            {
                UnlockedSkillsTypeList.Add(skillMagicType);
                OnSkillUnlocked?.Invoke(this, new OnSkillUnlockedEventArgs { SkillMagicType = skillMagicType });
            }

            SwitchingSkill(skillMagicType);
        }

        public void SwitchingSkill(SkillMagicType skillMagicType)
        {
            OnSkillSwitched?.Invoke(this, skillMagicType);
        }

        public bool IsSkillUnlocked(SkillMagicType skillMagicType)
        {
            return UnlockedSkillsTypeList.Contains(skillMagicType);
        }

        public SkillMagicType? GetSkillRequired(SkillMagicType skillMagicType)
        {
            return skillMagicType switch
            {
                // SkillType.Skill2 => SkillType.Skill1,
                // SkillType.Skill3 => SkillType.Skill2,
                _ => SkillMagicType.None
            };
        }

        public bool TryUnlockSkill(SkillMagicType skillMagicType)
        {
            var skillRequired = GetSkillRequired(skillMagicType);
            if (skillRequired != null && (skillRequired == SkillMagicType.None || IsSkillUnlocked(skillRequired.Value)))
            {
                UnlockSkill(skillMagicType);
                return true;
            }

            return false;
        }
    }
}