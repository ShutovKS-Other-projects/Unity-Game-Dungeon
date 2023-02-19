using Player;
using UnityEngine;
namespace UIGame
{
    public class UIGame : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;

        [SerializeField] private GameObject _healthText;
        [SerializeField] private GameObject _staminaText;
        [SerializeField] private GameObject _dialogText;

        private void FixedUpdate()
        {
            UpdateGameStatistics();
        }

        public void UpdateGameStatistics()
        {
            _healthText.GetComponent<UnityEngine.UI.Text>().text = $"Здоровья: {_playerData.health}/{_playerData.maxHealth}";
            _staminaText.GetComponent<UnityEngine.UI.Text>().text = $"Выносливости: {_playerData.stamina}/{_playerData.maxStamina}";
            //_dialogText.GetComponent<UnityEngine.UI.Text>().text = ;
        }
    }
}
