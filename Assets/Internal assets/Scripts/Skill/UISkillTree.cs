using System.Collections.Generic;
using Magic.Type;
using Player;
using Skill.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Skill.SkillTree
{
    public class UISkillTree : MonoBehaviour
    {
        private SkillMagic _skillMagic;
        private List<SkillButton> _skillButtonList;

        /// <summary>
        /// Обновление визуальных элементов скилов
        /// </summary>
        /// <param name="skillMagic"> Скилы</param>
        public void SetMagicSkills(SkillMagic skillMagic)
        {
            _skillMagic = skillMagic;
            

            for (var i = 0; i < transform.childCount; i++)
            {
                _skillButtonList.Add(new SkillButton(transform.GetChild(i), skillMagic, (MagicAttackType)i));
            }

            _skillMagic.OnSkillUnlocked += SkillMagic_OnSkillUnlocked;
            UpdateVisuals();
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

        /// <summary>
        /// Обновление визуальных элементов
        /// </summary>
        private void UpdateVisuals() { }
    }
}