using Inventory;
using Player;
using UnityEngine;
using UnityEngine.UI;
namespace UIGame
{
    public class UIPlayerInfo : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;

        [SerializeField] private GameObject _classText;
        [SerializeField] private GameObject _healthText;
        [SerializeField] private GameObject _staminaText;
        [SerializeField] private GameObject _damageText;

        private void FixedUpdate()
        {
            UpdatePlayerInfo();
        }

        public void UpdatePlayerInfo()
        {
            _classText.GetComponent<Text>().text = $"{_playerData.className}";
            _healthText.GetComponent<Text>().text = $"Здоровья: {_playerData.health}";
            _staminaText.GetComponent<Text>().text = $"Выносливости: {_playerData.stamina}";
            _damageText.GetComponent<Text>().text = $"Урон: {_playerData.damage}";
        }

        public void UpdateInventory()
        {
            try
            {
                GetComponentInChildren<StaticInterface>().AllSlotsUpdate();
            }
            catch
            {
                Debug.Log("StaticInterface not found");
            }
            try
            {
                GetComponentInChildren<DynamicInterface>().AllSlotsUpdate();
            }
            catch
            {
                Debug.Log("DynamicInterface not found");
            }
        }
    }
}