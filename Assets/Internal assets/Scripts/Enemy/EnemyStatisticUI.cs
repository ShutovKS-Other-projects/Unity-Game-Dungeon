using Enemy.FiniteStateMachine;
using TMPro;
using UnityEngine;
namespace Enemy
{
    public class EnemyStatisticUI : MonoBehaviour
    {
        private EnemyStatistic _statistic;
        private GameObject _nameText;
        private GameObject _levelText;
        private GameObject _healthText;
        private Transform _cameraTransform;

        private void Start()
        {
            _statistic = GetComponentInParent<EnemyStateController>().EnemyStatistic;
            
            _nameText = transform.Find("Name").gameObject;
            _levelText = transform.Find("Level").gameObject;
            _healthText = transform.Find("Health").gameObject;
            
            _nameText.GetComponent<TextMeshPro>().text = $"{_statistic.RaceName}";
            _levelText.GetComponent<TextMeshPro>().text = $"Level: {_statistic.Level}";
            
            _cameraTransform = UnityEngine.Camera.main!.transform;
        }

        private void FixedUpdate()
        {
            if (_statistic.Health > 0)
                Display();
            else
                NoDisplay();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Display()
        {
            _nameText.SetActive(true);
            _healthText.SetActive(true);
            transform.LookAt(_cameraTransform.transform.position);
            _healthText.GetComponent<TextMeshPro>().text = $"XP: {_statistic.Health}";
        }
        private void NoDisplay()
        {
            _nameText.SetActive(false);
            _healthText.SetActive(false);
        }
    }
}
