using Inventory;
using Player;
using TMPro;
using UnityEngine;
namespace UI.PlayerInfo
{
    public class Characteristics : MonoBehaviour
    {
        private PlayerStatistic _playerStatistic;
        
        TextMeshProUGUI _healthText;
        TextMeshProUGUI _manaText;
        TextMeshProUGUI _staminaText;
        TextMeshProUGUI _strengthText;
        TextMeshProUGUI _armorText;
        TextMeshProUGUI _agilityText;

        private void OnEnable()
        {
            _playerStatistic = GameObject.FindWithTag("Player").GetComponent<PlayerStatistic>();
            
            _healthText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _manaText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            _staminaText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            _strengthText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            _armorText = transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            _agilityText = transform.GetChild(5).GetComponent<TextMeshProUGUI>();
        }

        private void FixedUpdate()
        {
            UpdatePlayerInfo();
        }

        private void UpdatePlayerInfo()
        {
            UpdateTextHealth($"Здоровья: {_playerStatistic.HealthMax}");
            UpdateTextMana($"Маны: {_playerStatistic.ManaMax}");
            UpdateTextStamina($"Выносливости: {_playerStatistic.StaminaMax}");
            UpdateTextDamage($"Сила: {_playerStatistic.Strength}");
            UpdateTextArmor($"Броня: {_playerStatistic.Armor}");
            UpdateTextAgility($"Ловкость: {_playerStatistic.Agility}");
        }

        void UpdateTextHealth(string text) => _healthText.text = text;
        void UpdateTextMana(string text) => _manaText.text = text;
        void UpdateTextStamina(string text) => _staminaText.text = text;
        void UpdateTextDamage(string text) => _strengthText.text = text;
        void UpdateTextArmor(string text) => _armorText.text = text;
        void UpdateTextAgility(string text) => _agilityText.text = text;

    }
}
