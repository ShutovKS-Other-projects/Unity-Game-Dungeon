using System.Net.Mime;
using Inventory;
using Player;
using UnityEngine;
using UnityEngine.UI;
namespace UIGame
{
    public class UIPlayerInfo : MonoBehaviour
    {
        private PlayerStatistic _playerStatistic;

        [SerializeField] private Text _classText;

        [SerializeField] private Text _healthText;
        [SerializeField] private Text _manaText;
        [SerializeField] private Text _staminaText;

        [SerializeField] private Text _damageText;
        [SerializeField] private Text _armorText;
        [SerializeField] private Text _agilityText;

        private void Start()
        {
            _playerStatistic = GameObject.FindWithTag("Player").GetComponent<PlayerStatistic>();
            // var listCharacter = transform.Find("ListCharacteristics");

            // _classText = transform.Find("Class").GetComponent<Text>().transform.GetChild(0).GetComponent<Text>();

            // _healthText = listCharacter.Find("Health").GetComponent<Text>();
            // _manaText = listCharacter.Find("Mana").GetComponent<Text>();
            // _staminaText = listCharacter.Find("Stamina").GetComponent<Text>();

            // _damageText = listCharacter.Find("Damage").GetComponent<Text>();
            // _armorText = listCharacter.Find("Armor").GetComponent<Text>();
            // _agilityText = listCharacter.Find("Agility").GetComponent<Text>();
        }

        private void FixedUpdate()
        {
            UpdatePlayerInfo();
        }

        private void UpdatePlayerInfo()
        {
            _classText.GetComponent<Text>().text = $"{_playerStatistic.ClassName}";

            _healthText.GetComponent<Text>().text = $"Здоровья: {_playerStatistic.HealthMax}";
            _manaText.GetComponent<Text>().text = $"Маны: {_playerStatistic.ManaMax}";
            _staminaText.GetComponent<Text>().text = $"Выносливости: {_playerStatistic.StaminaMax}";

            _damageText.GetComponent<Text>().text = $"Сила: {_playerStatistic.Strength}";
            _armorText.GetComponent<Text>().text = $"Броня: {_playerStatistic.Armor}";
            _agilityText.GetComponent<Text>().text = $"Ловкость: {_playerStatistic.Agility}";
        }

        public void InventoryUpdate()
        {
            transform.GetChild(0).GetComponent<StaticInterface>().AllSlotsInInventoryUpdate();
            transform.GetChild(1).GetComponent<DynamicInterface>().AllSlotsInInventoryUpdate();
        }
    }
}
