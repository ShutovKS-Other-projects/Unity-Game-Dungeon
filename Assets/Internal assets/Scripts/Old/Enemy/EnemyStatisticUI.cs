using Old.Enemy.FiniteStateMachine;
using TMPro;
using UnityEngine;

namespace Old.Enemy
{
    public class EnemyStatisticUI : MonoBehaviour
    {
        private EnemyStateController _stateController;
        private EnemyStatistic _statistic;
        private TextMeshProUGUI _healthText;
        private Transform _cameraTransform;

        private void Start()
        {
            _stateController = GetComponentInParent<EnemyStateController>();
            _statistic = _stateController.EnemyStatistic;
            _cameraTransform = UnityEngine.Camera.main!.transform;

            _healthText = transform.Find("Health").GetComponent<TextMeshProUGUI>();

            transform.Find("Name").GetComponent<TextMeshPro>().text = $"{_statistic.RaceName}";
            transform.Find("Level").GetComponent<TextMeshPro>().text = $"Level: {_statistic.Level}";
            _healthText.GetComponent<TextMeshPro>().text = $"XP: {_statistic.Health}";


            _stateController.UpdateStatistic += UpdateStatistic;
        }

        private void FixedUpdate()
        {
            transform.LookAt(_cameraTransform.transform.position);
        }

        private void UpdateStatistic() => _healthText.GetComponent<TextMeshPro>().text = $"XP: {_statistic.Health}";
    }
}