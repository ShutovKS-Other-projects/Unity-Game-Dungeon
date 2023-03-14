using System.Collections.Generic;
using Magic.Type;
using Skill.SkillTree;
using TMPro;
using UnityEngine;

namespace Skill.Magic
{
    public class UISkillTreeMagic : MonoBehaviour
    {
        private SkillMagic _skillMagic;
        private List<SkillButtonMagic> _skillButtonList;
        [SerializeField] private GameObject skillPoints;

        /// <summary>
        /// Обновление визуальных элементов скилов
        /// </summary>
        /// <param name="skillMagic"> Скилы</param>
        public void SetMagicSkills(SkillMagic skillMagic)
        {
            _skillMagic = skillMagic;
            _skillButtonList = new List<SkillButtonMagic>();

            for (var i = 0; i < transform.childCount; i++)
            {
                _skillButtonList.Add(new SkillButtonMagic(transform.GetChild(i), skillMagic, (MagicType)i));
            }

            _skillMagic.OnSkillUnlocked += SkillMagic_OnSkillUnlocked;
            _skillMagic.OnSkillPointsUpdate += SkillCharacteristic_OnSkillPointsUpdate;
            UpdateVisuals();
        }

        private void SkillCharacteristic_OnSkillPointsUpdate()
        {
            SkillPointsUpdate();
        }

        /// <summary>
        /// Вызывается при разблокировке скила
        /// </summary>
        /// <param name="sender"> Объект, который вызвал событие</param>
        /// <param name="e"> Аргументы события</param>
        private void SkillMagic_OnSkillUnlocked(object sender, SkillMagic.OnSkillUnlockedEventArgs e)
        {
            UpdateVisuals();
        }

        private void SkillPointsUpdate()
        {
            skillPoints.GetComponent<TextMeshProUGUI>().text = _skillMagic.GetSkillPoints().ToString();
        }

        /// <summary>
        /// Обновление визуальных элементов
        /// </summary>
        private void UpdateVisuals()
        {
        }
    }
}