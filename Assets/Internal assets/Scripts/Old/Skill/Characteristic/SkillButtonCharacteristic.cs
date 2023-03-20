using Old.Skill.SkillTree;
using UnityEngine;
using UnityEngine.UI;

namespace Old.Skill.Characteristic
{
    public class SkillButtonCharacteristic
    {
        private Transform _transform;
        private readonly SkillCharacteristicType _skillCharacteristicType;
        
        private Image _image;
        private Image _backgroundImage;
        
        public SkillButtonCharacteristic(Transform transform, SkillCharacteristic skillCharacteristic, SkillCharacteristicType skillCharacteristicType)
        {
            _transform = transform;
            _skillCharacteristicType = skillCharacteristicType;
            
            transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                skillCharacteristic.TryUnlockSkill(_skillCharacteristicType);
            });
        }
        
        public void UpdateVisuals(bool isUnlocked)
        {
            
        }
    }
}