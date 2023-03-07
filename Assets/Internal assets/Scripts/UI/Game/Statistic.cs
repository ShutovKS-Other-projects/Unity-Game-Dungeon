using Level;
using Player;
using TMPro;
using UnityEngine;

namespace UI.Game
{
    public class Statistic : MonoBehaviour
    {
        private PlayerStatistic _playerStatistic;
        private LevelSystem _levelSystem;

        private TextMeshProUGUI _levelText;
        private TextMeshProUGUI _healthText;
        private TextMeshProUGUI _staminaText;

        private void Start()
        {
            _playerStatistic = GameObject.FindWithTag("Player").GetComponent<PlayerStatistic>();
            _levelSystem = _playerStatistic.LevelSystem;

            _levelText = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
            _healthText = transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
            _staminaText = transform.Find("StaminaText").GetComponent<TextMeshProUGUI>();

            UpdateTextLevel();
            _levelSystem.OnLevelUp += UpdateTextLevel;
        }

        private void FixedUpdate()
        {
            UpdateTextHealth();
            UpdateTextStamina();
        }


        private void UpdateTextLevel() => _levelText.text = $"Уровень: {_playerStatistic.Level}";

        private void UpdateTextHealth() => _healthText.text =
            $"Здоровья: {Mathf.Round(_playerStatistic.Health)}/{_playerStatistic.HealthMax}";

        private void UpdateTextStamina() => _staminaText.text =
            $"Выносливости: {Mathf.Round(_playerStatistic.Stamina)}/{_playerStatistic.StaminaMax}";
    }
}