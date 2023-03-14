using System.Collections.Generic;
using Skill.SkillTree;
using TMPro;
using UnityEngine;

namespace Skill.Characteristic
{
    public class UISkillTreeCharacteristic : MonoBehaviour
    {
        private SkillCharacteristic _skillCharacteristic;
        private List<SkillButtonCharacteristic> _skillButtonList;
        [SerializeField] private GameObject skillPoints;

        public void SetCharacteristicSkills(SkillCharacteristic skillCharacteristic)
        {
            Debug.Log("UISkillTreeCharacteristic SetCharacteristicSkills");
            _skillCharacteristic = skillCharacteristic;
            _skillButtonList = new List<SkillButtonCharacteristic>();

            for (var i = 0; i < transform.childCount; i++)
            {
                _skillButtonList.Add(new SkillButtonCharacteristic(transform.GetChild(i), skillCharacteristic,
                    (SkillCharacteristicType)i));
            }

            _skillCharacteristic.OnSkillUnlocked += SkillCharacteristic_OnSkillUnlocked;
            _skillCharacteristic.OnSkillPointsUpdate += SkillCharacteristic_OnSkillPointsUpdate;
            UpdateVisuals();
        }

        private void SkillCharacteristic_OnSkillPointsUpdate()
        {
            SkillPointsUpdate();
        }

        private void SkillCharacteristic_OnSkillUnlocked(object sender, SkillCharacteristic.OnSkillUnlockedEventArgs e)
        {
            UpdateVisuals();
        }

        private void SkillPointsUpdate()
        {
            skillPoints.GetComponent<TextMeshProUGUI>().text = _skillCharacteristic.GetSkillPoints().ToString();
        }

        private void UpdateVisuals()
        {
        }
    }
}