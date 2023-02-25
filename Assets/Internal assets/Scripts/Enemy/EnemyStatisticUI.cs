using Enemy.FiniteStateMachine;
using TMPro;
using UnityEngine;
namespace Enemy
{
    public class EnemyStatisticUI : MonoBehaviour
    {
        EnemyStatistic _statistic;
        GameObject _nameText;
        GameObject _healthText;
        Transform _cameraTransform;

        private void Start()
        {
            _statistic = GetComponentInParent<EnemyStateController>().EnemyStatistic;
            
            _nameText = transform.Find("Name").gameObject;
            _healthText = transform.Find("Health").gameObject;
            
            _nameText.GetComponent<TextMeshPro>().text = $"{_statistic.raceName}";
            
            _cameraTransform = UnityEngine.Camera.main!.transform;
        }

        private void FixedUpdate()
        {
            if (_statistic.health > 0)
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
            _healthText.GetComponent<TextMeshPro>().text = $"XP: {_statistic.health}";
        }
        private void NoDisplay()
        {
            _nameText.SetActive(false);
            _healthText.SetActive(false);
        }
    }
}
