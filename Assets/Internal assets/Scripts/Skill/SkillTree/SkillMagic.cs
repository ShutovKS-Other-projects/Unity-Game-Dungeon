using System;
using System.Collections.Generic;
using Magic.Type;
using Skill.Enum;
using UnityEngine;

namespace Skill.SkillTree
{
    public class SkillMagic
    {
        public SkillMagic() => _unlockedSkillsTypeList = new List<MagicAttackType>();

        #region List

        /// <summary>
        /// Список разблокированных скилов
        /// </summary>
        private readonly List<MagicAttackType> _unlockedSkillsTypeList;

        #endregion

        #region Events

        /// <summary>
        /// Вызывается при смене скила
        /// </summary>
        public event EventHandler<MagicAttackType> OnSkillSwitched;
        
        /// <summary>
        /// Вызывается при разблокировке нового скила
        /// </summary>
        public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;

        /// <summary>
        /// Аргументы для события OnSkillUnlocked
        /// </summary>
        public class OnSkillUnlockedEventArgs : EventArgs
        {
            public MagicAttackType MagicAttackType { get; set; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Попытка разблокировки скила
        /// </summary>
        /// <param name="magicAttackType"> Тип скила, который необходимо разблокировать</param>
        /// <returns> Возвращает true, если скил разблокирован, иначе false</returns>
        public bool TryUnlockSkill(MagicAttackType magicAttackType)
        {
            if (!CanUnlockSkill(magicAttackType)) return false;
            UnlockSkill(magicAttackType);
            return true;
        }

        /// <summary>
        /// Проверка на возможность разблокировки скила 
        /// </summary>
        /// <param name="magicAttackType"> Тип скила, который необходимо разблокировать</param>
        /// <returns> Возвращает true, если скил можно разблокировать, иначе false</returns>
        private bool CanUnlockSkill(MagicAttackType magicAttackType)
        {
            var skillRequired = GetSkillRequired(magicAttackType);
            if (skillRequired == MagicAttackType.None) return true;
            return skillRequired != null && IsSkillUnlocked(skillRequired.Value);
        }
        
        /// <summary>
        /// Проверка на разблокировку скила 
        /// </summary>
        /// <param name="magicAttackType"> Тип скила, который необходимо проверить</param>
        /// <returns> Возвращает true, если скил разблокирован, иначе false</returns>
        private bool IsSkillUnlocked(MagicAttackType magicAttackType)
        {
            if (!_unlockedSkillsTypeList.Contains(magicAttackType)) return false;
            SwitchingSkill(magicAttackType);
            return true;
        }

        /// <summary>
        /// Разблокировка скила
        /// </summary>
        /// <param name="magicAttackType">Тип разблокируемого скила</param>
        private void UnlockSkill(MagicAttackType magicAttackType)
        {
            _unlockedSkillsTypeList.Add(magicAttackType);
            OnSkillUnlocked?.Invoke(this, new OnSkillUnlockedEventArgs { MagicAttackType = magicAttackType });
            Debug.Log($"Skill {magicAttackType} unlocked");
            SwitchingSkill(magicAttackType);
        }

        /// <summary>
        /// Смена скила
        /// </summary>
        /// <param name="magicAttackType"> Тип скила, на который необходимо сменить</param>
        private void SwitchingSkill(MagicAttackType magicAttackType)
        {
            OnSkillSwitched?.Invoke(this, magicAttackType);
            Debug.Log($"Skill {magicAttackType} switched");
        }


        /// <summary>
        /// Получение скила, который необходим для разблокировки
        /// </summary>
        /// <param name="magicAttackType"> Тип скила, который необходимо разблокировать</param>
        /// <returns> Тип скила, который необходим для разблокировки выбраного скила, если такого скила нет, то возвращается None</returns>
        private static MagicAttackType? GetSkillRequired(MagicAttackType magicAttackType) => magicAttackType switch
        {
            //1 LVL
            MagicAttackType.FireIgnition => MagicAttackType.Default,
            MagicAttackType.IceFrostbite => MagicAttackType.Default,
            MagicAttackType.LightningStaticShock => MagicAttackType.Default,
            MagicAttackType.EarthRockslide => MagicAttackType.Default,
            MagicAttackType.WaterTorrent => MagicAttackType.Default,
            MagicAttackType.AirGust => MagicAttackType.Default,

            //2 LVL
            MagicAttackType.FireInferno => MagicAttackType.FireIgnition,
            MagicAttackType.IceGlacialChill => MagicAttackType.IceFrostbite,
            MagicAttackType.LightningThunderbolt => MagicAttackType.LightningStaticShock,
            MagicAttackType.EarthTremor => MagicAttackType.EarthRockslide,
            MagicAttackType.WaterTsunami => MagicAttackType.WaterTorrent,
            MagicAttackType.AirHurricane => MagicAttackType.AirGust,

            //3 LVL
            MagicAttackType.FireMeteor => MagicAttackType.FireInferno,
            MagicAttackType.IceAbsoluteZero => MagicAttackType.IceGlacialChill,
            MagicAttackType.LightningLightningStorm => MagicAttackType.LightningThunderbolt,
            MagicAttackType.EarthEarthquake => MagicAttackType.EarthTremor,
            MagicAttackType.WaterCyclone => MagicAttackType.WaterTsunami,
            MagicAttackType.AirTornado => MagicAttackType.AirHurricane,

            _ => MagicAttackType.None
        };

        #endregion
    }
}