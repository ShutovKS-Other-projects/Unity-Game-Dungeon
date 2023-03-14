using System.Collections.Generic;
using Magic.Type;
using Skill.SkillTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

            _skillMagic.OnSkillUnlocked += UpdateVisuals;
            _skillMagic.OnSkillPointsUpdate += SkillPointsUpdate;
            UpdateVisuals(null, null);
        }

        private void SkillPointsUpdate()
        {
            skillPoints.GetComponent<TextMeshProUGUI>().text = _skillMagic.GetSkillPoints().ToString();
        }

        /// <summary>
        /// Вызывается при разблокировке скила
        /// </summary>
        /// <param name="sender"> Объект, который вызвал событие</param>
        /// <param name="e"> Аргументы события</param>
        private void UpdateVisuals(object sender, SkillMagic.OnSkillUnlockedEventArgs e)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var skillType = (MagicType)i;
                if (_skillMagic.IsSkillUnlocked(skillType))
                    transform.GetChild(i).GetComponent<Image>().color = Color.white;
                else if (_skillMagic.CanUnlockSkill(skillType))
                    transform.GetChild(i).GetComponent<Image>().color = Color.gray;
                else
                    transform.GetChild(i).GetComponent<Image>().color = Color.black;
            }
        }
    }
}