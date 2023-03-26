using UnityEngine;

namespace Old.Level
{
    /// <summary>
    /// Система уровней
    /// </summary>
    public class LevelSystem
    {
        #region Events

        /// <summary>
        /// Событие повышения уровня
        /// </summary>
        public event System.Action OnLevelUp;

        /// <summary>
        /// Событие изменения количества очков опыта
        /// </summary>
        public event System.Action OnExperienceChanged;

        #endregion

        #region Singleton

        private int _experienceToNextLevel;

        #endregion

        #region Properties

        /// <summary>
        /// Уровень
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Очки опыта
        /// </summary>
        public int Experience { get; private set; }

        /// <summary>
        /// Очки опыта до следующего уровня
        /// </summary>
        public int ExperienceToNextLevel => _experienceToNextLevel;

        /// <summary>
        /// Очки опыта до следующего уровня в процентах
        /// </summary>
        public float ExperienceNormalized => (float)Experience / _experienceToNextLevel;

        #endregion

        #region Constructor

        public LevelSystem()
        {
            Level = 1;
            Experience = 0;
            _experienceToNextLevel = 100;
        }

        public LevelSystem(int level, int experience, int experienceToNextLevel)
        {
            Level = level;
            Experience = experience;
            _experienceToNextLevel = experienceToNextLevel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Добавить очков опыта
        /// </summary>
        /// <param name="experience">Количество очков опыта добавляемых</param>
        public void AddExperience(int experience)
        {
            Experience += experience;
            if (Experience >= _experienceToNextLevel)
            {
                LevelUp();
            }

            OnExperienceChanged?.Invoke();
        }

        /// <summary>
        /// Повысить уровень
        /// </summary>
        private void LevelUp()
        {
            Level++;
            Experience -= _experienceToNextLevel;
            _experienceToNextLevel = GetCountExperienceToNextLevel();
            OnLevelUp?.Invoke();
        }

        /// <summary>
        /// Подсчитать количество очков опыта до следующего уровня
        /// </summary>
        /// <returns></returns>
        private int GetCountExperienceToNextLevel()
        {
            var xp = 0.5 * Mathf.Log(Level, 1.1f) + (0.5 * Level + 1);
            xp = Mathf.Ceil((float)xp);
            return (int)xp;
        }

        #endregion
    }
}