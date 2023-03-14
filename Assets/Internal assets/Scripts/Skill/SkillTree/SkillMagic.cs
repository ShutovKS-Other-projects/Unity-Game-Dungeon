using System;
using System.Collections.Generic;
using Magic.Type;
using Skill.Enum;
using UnityEngine;

namespace Skill.SkillTree
{
    public class SkillMagic
    {
        public SkillMagic() => _unlockedSkillsTypeList = new List<MagicType>();
        private MagicType _currentMagicType;

        #region List

        /// <summary>
        /// Список разблокированных скилов
        /// </summary>
        private readonly List<MagicType> _unlockedSkillsTypeList;

        private int _skillPoints;

        #endregion

        #region Events

        public event Action OnSkillPointsUpdate;

        /// <summary>
        /// Вызывается при смене скила
        /// </summary>
        public event EventHandler<MagicType> OnSkillSwitched;

        /// <summary>
        /// Вызывается при разблокировке нового скила
        /// </summary>
        public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;

        /// <summary>
        /// Аргументы для события OnSkillUnlocked
        /// </summary>
        public class OnSkillUnlockedEventArgs : EventArgs
        {
            public MagicType MagicType { get; set; }
        }

        #endregion

        #region Methods

        public void AddSkillPoint()
        {
            _skillPoints++;
        OnSkillPointsUpdate?.Invoke();
        }

        public int GetSkillPoints() => _skillPoints;

        /// <summary>
        /// Попытка разблокировки скила
        /// </summary>
        /// <param name="magicType"> Тип скила, который необходимо разблокировать</param>
        /// <returns> Возвращает true, если скил разблокирован, иначе false</returns>
        public bool TryUnlockSkill(MagicType magicType)
        {
            if (CanUnlockSkill(magicType))
            {
                UnlockSkill(magicType);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Проверка на возможность разблокировки скила 
        /// </summary>
        /// <param name="magicType"> Тип скила, который необходимо разблокировать</param>
        /// <returns> Возвращает true, если скил можно разблокировать, иначе false</returns>
        private bool CanUnlockSkill(MagicType magicType)
        {
            if (IsSkillUnlocked(magicType))
            {
                if (_currentMagicType != magicType)
                    SwitchingSkill(magicType);
                return false;
            }

            if (_skillPoints <= 0)
                return false;

            var skillRequired = GetSkillRequired(magicType);
            return skillRequired != null
                   && (skillRequired == MagicType.None
                       || IsSkillUnlocked(skillRequired.Value));
        }

        /// <summary>
        /// Проверка на разблокировку скила 
        /// </summary>
        /// <param name="magicType"> Тип скила, который необходимо проверить</param>
        /// <returns> Возвращает true, если скил разблокирован, иначе false</returns>
        private bool IsSkillUnlocked(MagicType magicType)
        {
            return _unlockedSkillsTypeList.Contains(magicType);
        }

        /// <summary>
        /// Разблокировка скила
        /// </summary>
        /// <param name="magicType">Тип разблокируемого скила</param>
        private void UnlockSkill(MagicType magicType)
        {
            _unlockedSkillsTypeList.Add(magicType);
            _skillPoints--;
            OnSkillPointsUpdate?.Invoke();
            OnSkillUnlocked?.Invoke(this, new OnSkillUnlockedEventArgs { MagicType = magicType });
            Debug.Log($"Skill {magicType} unlocked");
            SwitchingSkill(magicType);
        }

        /// <summary>
        /// Смена скила
        /// </summary>
        /// <param name="magicType"> Тип скила, на который необходимо сменить</param>
        private void SwitchingSkill(MagicType magicType)
        {
            OnSkillSwitched?.Invoke(this, magicType);
            _currentMagicType = magicType;
            Debug.Log($"Skill {magicType} switched");
        }


        /// <summary>
        /// Получение скила, который необходим для разблокировки
        /// </summary>
        /// <param name="magicType"> Тип скила, который необходимо разблокировать</param>
        /// <returns> Тип скила, который необходим для разблокировки выбраного скила, если такого скила нет, то возвращается None</returns>
        private static MagicType? GetSkillRequired(MagicType magicType) => magicType switch
        {
            MagicType.Fire1 => MagicType.Default,
            MagicType.Ice1 => MagicType.Default,
            MagicType.Lightning1 => MagicType.Default,
            MagicType.Buff1 => MagicType.Default,
            MagicType.Debuff1 => MagicType.Default,

            MagicType.Fire2 => MagicType.Fire1,
            MagicType.Ice2 => MagicType.Ice1,
            MagicType.Lightning2 => MagicType.Lightning1,
            MagicType.Buff2 => MagicType.Buff1,
            MagicType.Debuff2 => MagicType.Debuff1,

            MagicType.Fire3 => MagicType.Fire2,
            MagicType.Ice3 => MagicType.Ice2,
            MagicType.Lightning3 => MagicType.Lightning2,
            MagicType.Buff3 => MagicType.Buff2,
            MagicType.Debuff3 => MagicType.Debuff2,

            MagicType.Fire4 => MagicType.Fire3,
            MagicType.Ice4 => MagicType.Ice3,
            MagicType.Lightning4 => MagicType.Lightning3,
            MagicType.Buff4 => MagicType.Buff3,
            MagicType.Debuff4 => MagicType.Debuff3,

            MagicType.Fire5 => MagicType.Fire4,
            MagicType.Ice5 => MagicType.Ice4,
            MagicType.Lightning5 => MagicType.Lightning4,
            MagicType.Buff5 => MagicType.Buff4,
            MagicType.Debuff5 => MagicType.Debuff4,

            MagicType.Fire6 => MagicType.Fire5,
            MagicType.Ice6 => MagicType.Ice5,
            MagicType.Lightning6 => MagicType.Lightning5,
            MagicType.Buff6 => MagicType.Buff5,
            MagicType.Debuff6 => MagicType.Debuff5,

            _ => MagicType.None
        };

        #endregion
    }
}