using Player;
using TMPro;
using UnityEngine;
namespace UI.Game
{
    public class Statistic : MonoBehaviour
    {
        //Add event system
        
        PlayerStatistic _playerStatistic;

        TextMeshProUGUI _healthText;
        TextMeshProUGUI _staminaText;

        void OnEnable()
        {
            _playerStatistic = GameObject.FindWithTag("Player").GetComponent<PlayerStatistic>();

            _healthText = transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
            _staminaText = transform.Find("StaminaText").GetComponent<TextMeshProUGUI>();
        }
        
        void FixedUpdate()
        {
            UpdateGameStatistics();
        }
        
        void UpdateGameStatistics()
        {
            UpdateTextHealth($"Здоровья: {Mathf.Round(_playerStatistic.Health)}/{_playerStatistic.HealthMax}");
            UpdateTextStamina($"Выносливости: {Mathf.Round(_playerStatistic.Stamina)}/{_playerStatistic.StaminaMax}");
        }

        void UpdateTextHealth(string text) => _healthText.text = text;
        void UpdateTextStamina(string text) => _staminaText.text = text;
    }
}
