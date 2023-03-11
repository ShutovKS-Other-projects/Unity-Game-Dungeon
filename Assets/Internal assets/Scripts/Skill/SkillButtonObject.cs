using Magic.Type;
using Skill.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace Skill
{
    [CreateAssetMenu(fileName = "New SkillButton", menuName = "Skill/SkillButton", order = 0)]
    public class SkillButtonObject : ScriptableObject
    {
        [FormerlySerializedAs("skillMagicType")] public MagicAttackType magicAttackType;
        
        public Sprite skillSprite;
        public Sprite skillBackgroundSprite;
        
        public string SkillName => magicAttackType.ToString();
        
        [TextArea(3, 10)]
        public string skillDescription;
    }
}