using Skill.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace Skill
{
    [CreateAssetMenu(fileName = "New SkillButton", menuName = "Skill/SkillButton", order = 0)]
    public class SkillButtonObject : ScriptableObject
    {
        public SkillMagicType skillMagicType;
        
        public Sprite skillSprite;
        public Sprite skillBackgroundSprite;
        
        public string SkillName => skillMagicType.ToString();
        
        [TextArea(3, 10)]
        public string skillDescription;
    }
}