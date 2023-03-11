using System;
using System.Collections.Generic;
using Skill.Enum;
using UnityEngine;

namespace Skill.SkillTree
{
    public class SkillMagic
    {
        public SkillMagic() => _unlockedSkillsTypeList = new List<SkillMagicType>();

        #region List

        /// <summary>
        /// Список разблокированных скилов
        /// </summary>
        private readonly List<SkillMagicType> _unlockedSkillsTypeList;

        #endregion

        #region Events

        /// <summary>
        /// Вызывается при разблокировке нового скила
        /// </summary>
        public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;

        /// <summary>
        /// Вызывается при смене скила
        /// </summary>
        public event EventHandler<SkillMagicType> OnSkillSwitched;

        /// <summary>
        /// Аргументы для события OnSkillUnlocked
        /// </summary>
        public class OnSkillUnlockedEventArgs : EventArgs
        {
            public SkillMagicType SkillMagicType { get; set; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Попытка разблокировки скила
        /// </summary>
        /// <param name="skillMagicType"> Тип скила, который необходимо разблокировать</param>
        /// <returns> Возвращает true, если скил разблокирован, иначе false</returns>
        public bool TryUnlockSkill(SkillMagicType skillMagicType)
        {
            if (!CanUnlockSkill(skillMagicType)) return false;
            UnlockSkill(skillMagicType);
            SwitchingSkill(skillMagicType);
            return true;
        }

        /// <summary>
        /// Проверка на разблокировку скила 
        /// </summary>
        /// <param name="skillMagicType"> Тип скила, который необходимо проверить</param>
        /// <returns> Возвращает true, если скил разблокирован, иначе false</returns>
        private bool IsSkillUnlocked(SkillMagicType skillMagicType)
        {
            if (!_unlockedSkillsTypeList.Contains(skillMagicType)) return false;
            SwitchingSkill(skillMagicType);
            return true;
        }

        /// <summary>
        /// Проверка на возможность разблокировки скила 
        /// </summary>
        /// <param name="skillMagicType"> Тип скила, который необходимо разблокировать</param>
        /// <returns> Возвращает true, если скил можно разблокировать, иначе false</returns>
        private bool CanUnlockSkill(SkillMagicType skillMagicType)
        {
            var skillRequired = GetSkillRequired(skillMagicType);
            if (skillRequired == SkillMagicType.None) return true;
            return skillRequired != null && IsSkillUnlocked(skillRequired.Value);
        }

        /// <summary>
        /// Разблокировка скила
        /// </summary>
        /// <param name="skillMagicType">Тип разблокируемого скила</param>
        private void UnlockSkill(SkillMagicType skillMagicType)
        {
            _unlockedSkillsTypeList.Add(skillMagicType);
            OnSkillUnlocked?.Invoke(this, new OnSkillUnlockedEventArgs { SkillMagicType = skillMagicType });
            Debug.Log($"Skill {skillMagicType} unlocked");
        }

        /// <summary>
        /// Смена скила
        /// </summary>
        /// <param name="skillMagicType"> Тип скила, на который необходимо сменить</param>
        private void SwitchingSkill(SkillMagicType skillMagicType)
        {
            OnSkillSwitched?.Invoke(this, skillMagicType);
            Debug.Log($"Skill {skillMagicType} switched");
        }


        /// <summary>
        /// Получение скила, который необходим для разблокировки
        /// </summary>
        /// <param name="skillMagicType"> Тип скила, который необходимо разблокировать</param>
        /// <returns> Тип скила, который необходим для разблокировки выбраного скила, если такого скила нет, то возвращается None</returns>
        private static SkillMagicType? GetSkillRequired(SkillMagicType skillMagicType) => skillMagicType switch
        {
            //1 LVL
            SkillMagicType.FireIgnition => SkillMagicType.Default,
            SkillMagicType.IceFrostbite => SkillMagicType.Default,
            SkillMagicType.LightningStaticShock => SkillMagicType.Default,
            SkillMagicType.EarthRockslide => SkillMagicType.Default,
            SkillMagicType.WaterTorrent => SkillMagicType.Default,
            SkillMagicType.AirGust => SkillMagicType.Default,

            //2 LVL
            SkillMagicType.FireInferno => SkillMagicType.FireIgnition,
            SkillMagicType.IceGlacialChill => SkillMagicType.IceFrostbite,
            SkillMagicType.LightningThunderbolt => SkillMagicType.LightningStaticShock,
            SkillMagicType.EarthTremor => SkillMagicType.EarthRockslide,
            SkillMagicType.WaterTsunami => SkillMagicType.WaterTorrent,
            SkillMagicType.AirHurricane => SkillMagicType.AirGust,

            //3 LVL
            SkillMagicType.FireMeteor => SkillMagicType.FireInferno,
            SkillMagicType.IceAbsoluteZero => SkillMagicType.IceGlacialChill,
            SkillMagicType.LightningLightningStorm => SkillMagicType.LightningThunderbolt,
            SkillMagicType.EarthEarthquake => SkillMagicType.EarthTremor,
            SkillMagicType.WaterCyclone => SkillMagicType.WaterTsunami,
            SkillMagicType.AirTornado => SkillMagicType.AirHurricane,

            _ => SkillMagicType.None
        };

        #endregion
    }
}