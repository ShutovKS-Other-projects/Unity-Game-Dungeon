using System;
using System.Diagnostics;
using Player.FiniteStateMachine;
using Skill.Characteristic;
using Skill.SkillTree;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Player
{
    public class PlayerSkillCharacteristic : MonoBehaviour
    {
        [NonSerialized] public float Health = 0f;
        [NonSerialized] public float Mana = 0f;
        [NonSerialized] public float Stamina = 0f;
        [NonSerialized] public float Strength = 0f;
        [NonSerialized] public float Armor = 0f;

        private SkillCharacteristic _skillCharacteristic;
        private UISkillTreeCharacteristic _uiSkillTreeCharacteristic;

        private void Awake()
        {
            Debug.Log("PlayerSkillCharacteristic Awake");

            _skillCharacteristic = new SkillCharacteristic();
            _skillCharacteristic.AssignmentsValue += AssignmentsValue!;

            _uiSkillTreeCharacteristic = FindObjectOfType<UISkillTreeCharacteristic>();
            _uiSkillTreeCharacteristic.SetCharacteristicSkills(_skillCharacteristic);
        }
        
        public void AddSkillPont() => _skillCharacteristic.AddSkillPoint();

        private void AssignmentsValue(object sender, SkillCharacteristicType skillCharacteristicType)
        {
            switch (skillCharacteristicType)
            {
                case SkillCharacteristicType.Stamina1:
                    Stamina = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health1:
                    Health = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana1:
                    Mana = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength1:
                    Strength = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor1:
                    Armor = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina2:
                    Stamina = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health2:
                    Health = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana2:
                    Mana = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength2:
                    Strength = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor2:
                    Armor = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina3:
                    Stamina = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health3:
                    Health = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana3:
                    Mana = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength3:
                    Strength = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor3:
                    Armor = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina4:
                    Stamina = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health4:
                    Health = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana4:
                    Mana = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength4:
                    Strength = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor4:
                    Armor = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina5:
                    Stamina = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health5:
                    Health = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana5:
                    Mana = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength5:
                    Strength = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor5:
                    Armor = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina6:
                    Stamina = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health6:
                    Health = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana6:
                    Mana = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength6:
                    Strength = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor6:
                    Armor = SkillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(skillCharacteristicType), skillCharacteristicType,
                        null);
            }
        }
    }
}