using System;
using Player.FiniteStateMachine;
using Skill.Characteristic;
using Skill.SkillTree;
using UnityEngine;

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

        private void AssignmentsValue(object sender, SkillCharacteristicType skillCharacteristicType)
        {
            switch (skillCharacteristicType)
            {
                case SkillCharacteristicType.Stamina1:
                    Stamina = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health1:
                    Health = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana1:
                    Mana = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength1:
                    Strength = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor1:
                    Armor = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina2:
                    Stamina = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health2:
                    Health = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana2:
                    Mana = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength2:
                    Strength = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor2:
                    Armor = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina3:
                    Stamina = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health3:
                    Health = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana3:
                    Mana = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength3:
                    Strength = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor3:
                    Armor = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina4:
                    Stamina = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health4:
                    Health = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana4:
                    Mana = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength4:
                    Strength = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor4:
                    Armor = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina5:
                    Stamina = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health5:
                    Health = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana5:
                    Mana = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength5:
                    Strength = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor5:
                    Armor = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Stamina6:
                    Stamina = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Health6:
                    Health = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Mana6:
                    Mana = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Strength6:
                    Strength = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
                    break;
                case SkillCharacteristicType.Armor6:
                    Armor = _skillCharacteristic.GetSkillValue(skillCharacteristicType);
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