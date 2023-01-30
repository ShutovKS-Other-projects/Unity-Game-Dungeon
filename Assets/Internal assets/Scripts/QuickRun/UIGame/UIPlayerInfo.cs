using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
    private PlayerStatistic _statistic;
    [SerializeField] private GameObject _classText;

    [SerializeField] private GameObject _healthText;
    [SerializeField] private GameObject _staminaText;
    [SerializeField] private GameObject _сollectCrystal;

    private void Awake()
    {
        _statistic = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatistic>();
    }

    private void Start()
    {
        _classText.GetComponent<Text>().text = $"{_statistic.Class.ToString()}";
        _healthText.GetComponent<Text>().text = $"Здоровья: {_statistic.Health.ToString()}";
        _staminaText.GetComponent<Text>().text = $"Выносливости: {_statistic.Stamina.ToString()}";
        _сollectCrystal.GetComponent<Text>().text = $"Кристаллов: {_statistic.CollectCrystal.ToString()}";
    }
}