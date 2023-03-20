using System;
using System.Collections.Generic;
using Old.Skill.Characteristic;
using UnityEngine;

namespace Old.Skill.SkillTree
{
    public class SkillCharacteristic
    {
        public SkillCharacteristic() => _unlockedSkillsTypeList = new List<SkillCharacteristicType>();

        private readonly List<SkillCharacteristicType> _unlockedSkillsTypeList;
        private int _skillPoints;

        public event Action OnSkillPointsUpdate;
        public event EventHandler<SkillCharacteristicType> AssignmentsValue;
        public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;

        public class OnSkillUnlockedEventArgs : EventArgs
        {
            public SkillCharacteristicType SkillCharacteristicType { get; set; }
        }

        public bool TryUnlockSkill(SkillCharacteristicType skillCharacteristicType)
        {
            if (!IsSkillPointEnough() || !CanUnlockSkill(skillCharacteristicType)) return false;
            UnlockSkill(skillCharacteristicType);
            AssignmentsValue?.Invoke(this, skillCharacteristicType);
            return true;
        }

        public void AddSkillPoint()
        {
            _skillPoints++;
            OnSkillPointsUpdate?.Invoke();
        }

        public int GetSkillPoints() => _skillPoints;

        public static float GetSkillValue(SkillCharacteristicType skillCharacteristicType) => skillCharacteristicType switch
        {
            SkillCharacteristicType.Stamina1 => 100,
            SkillCharacteristicType.Stamina2 => 150,
            SkillCharacteristicType.Stamina3 => 200,
            SkillCharacteristicType.Stamina4 => 400,
            SkillCharacteristicType.Stamina5 => 450,
            SkillCharacteristicType.Stamina6 => 500,

            SkillCharacteristicType.Health1 => 100,
            SkillCharacteristicType.Health2 => 150,
            SkillCharacteristicType.Health3 => 200,
            SkillCharacteristicType.Health4 => 400,
            SkillCharacteristicType.Health5 => 450,
            SkillCharacteristicType.Health6 => 500,

            SkillCharacteristicType.Mana1 => 100,
            SkillCharacteristicType.Mana2 => 150,
            SkillCharacteristicType.Mana3 => 200,
            SkillCharacteristicType.Mana4 => 400,
            SkillCharacteristicType.Mana5 => 450,
            SkillCharacteristicType.Mana6 => 500,

            SkillCharacteristicType.Strength1 => 10,
            SkillCharacteristicType.Strength2 => 15,
            SkillCharacteristicType.Strength3 => 20,
            SkillCharacteristicType.Strength4 => 40,
            SkillCharacteristicType.Strength5 => 45,
            SkillCharacteristicType.Strength6 => 50,

            SkillCharacteristicType.Armor1 => 10,
            SkillCharacteristicType.Armor2 => 15,
            SkillCharacteristicType.Armor3 => 20,
            SkillCharacteristicType.Armor4 => 40,
            SkillCharacteristicType.Armor5 => 45,
            SkillCharacteristicType.Armor6 => 50,

            _ => throw new ArgumentOutOfRangeException(nameof(skillCharacteristicType), skillCharacteristicType, null)
        };

        public bool CanUnlockSkill(SkillCharacteristicType skillCharacteristicType)
        {
            if (IsSkillUnlocked(skillCharacteristicType)) return false;
            var skillRequired = GetSkillRequired(skillCharacteristicType);
            return skillRequired != null
                   && (skillRequired == SkillCharacteristicType.None
                       || IsSkillUnlocked(skillRequired.Value));
        }

        public bool IsSkillUnlocked(SkillCharacteristicType skillCharacteristicType)
        {
            return _unlockedSkillsTypeList.Contains(skillCharacteristicType);
        }

        private bool IsSkillPointEnough() => _skillPoints > 0;

        private void UnlockSkill(SkillCharacteristicType skillCharacteristicType)
        {
            _unlockedSkillsTypeList.Add(skillCharacteristicType);
            _skillPoints--;
            OnSkillPointsUpdate?.Invoke();
            OnSkillUnlocked?.Invoke(this,
                new OnSkillUnlockedEventArgs { SkillCharacteristicType = skillCharacteristicType });
            Debug.Log($"Skill {skillCharacteristicType} unlocked");
        }

        private SkillCharacteristicType? GetSkillRequired(SkillCharacteristicType skillCharacteristicType) =>
            skillCharacteristicType switch
            {
                SkillCharacteristicType.Armor2 => SkillCharacteristicType.Armor1,
                SkillCharacteristicType.Armor3 => SkillCharacteristicType.Armor2,
                SkillCharacteristicType.Armor4 => SkillCharacteristicType.Armor3,
                SkillCharacteristicType.Armor5 => SkillCharacteristicType.Armor4,
                SkillCharacteristicType.Armor6 => SkillCharacteristicType.Armor5,

                SkillCharacteristicType.Stamina2 => SkillCharacteristicType.Stamina1,
                SkillCharacteristicType.Stamina3 => SkillCharacteristicType.Stamina2,
                SkillCharacteristicType.Stamina4 => SkillCharacteristicType.Stamina3,
                SkillCharacteristicType.Stamina5 => SkillCharacteristicType.Stamina4,
                SkillCharacteristicType.Stamina6 => SkillCharacteristicType.Stamina5,
                
                SkillCharacteristicType.Health2 => SkillCharacteristicType.Health1,
                SkillCharacteristicType.Health3 => SkillCharacteristicType.Health2,
                SkillCharacteristicType.Health4 => SkillCharacteristicType.Health3,
                SkillCharacteristicType.Health5 => SkillCharacteristicType.Health4,
                SkillCharacteristicType.Health6 => SkillCharacteristicType.Health5,

                SkillCharacteristicType.Mana2 => SkillCharacteristicType.Mana1,
                SkillCharacteristicType.Mana3 => SkillCharacteristicType.Mana2,
                SkillCharacteristicType.Mana4 => SkillCharacteristicType.Mana3,
                SkillCharacteristicType.Mana5 => SkillCharacteristicType.Mana4,
                SkillCharacteristicType.Mana6 => SkillCharacteristicType.Mana5,

                SkillCharacteristicType.Strength2 => SkillCharacteristicType.Strength1,
                SkillCharacteristicType.Strength3 => SkillCharacteristicType.Strength2,
                SkillCharacteristicType.Strength4 => SkillCharacteristicType.Strength3,
                SkillCharacteristicType.Strength5 => SkillCharacteristicType.Strength4,
                SkillCharacteristicType.Strength6 => SkillCharacteristicType.Strength5,
                
                _ => SkillCharacteristicType.None
            };
    }
}