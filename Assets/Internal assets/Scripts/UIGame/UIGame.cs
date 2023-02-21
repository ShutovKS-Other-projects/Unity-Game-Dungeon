using Player;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
namespace UIGame
{
    public class UIGame : MonoBehaviour
    {
        private PlayerStatistic _playerStatistic;

        private Text _healthText;
        private Text _staminaText;
        private Text _dialogText;

        private void Start()
        {
            _playerStatistic = GameObject.FindWithTag("Player").GetComponent<PlayerStatistic>();

            _healthText = transform.Find("Health").GetComponent<Text>();
            _staminaText = transform.Find("Stamina").GetComponent<Text>();
            _dialogText = transform.Find("Dialog").GetComponent<Text>();
        }
        
        private void FixedUpdate()
        {
            UpdateGameStatistics();
        }

        private void UpdateGameStatistics()
        {
            _healthText.GetComponent<Text>().text = $"Здоровья: {_playerStatistic.Health}/{_playerStatistic.HealthMax}";
            _staminaText.GetComponent<Text>().text = $"Выносливости: {_playerStatistic.Stamina}/{_playerStatistic.StaminaMax}";
            //_dialogText.GetComponent<Text>().text = ;
        }
    }
}
