using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
    private PlayerController controller;
    [SerializeField] private GameObject _classText;

    [SerializeField] private GameObject _healthText;
    [SerializeField] private GameObject _staminaText;
    [SerializeField] private GameObject _сollectCrystal;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdatePlayerInfo();
    }

    public void UpdatePlayerInfo()
    {
        _classText.GetComponent<Text>().text = $"{controller.statistic.Class}";
        _healthText.GetComponent<Text>().text = $"Здоровья: {controller.statistic.Health}";
        _staminaText.GetComponent<Text>().text = $"Выносливости: {controller.statistic.Stamina}";
        _сollectCrystal.GetComponent<Text>().text = $"Кристаллов: {controller.statistic.CollectCrystal}";
    }
}