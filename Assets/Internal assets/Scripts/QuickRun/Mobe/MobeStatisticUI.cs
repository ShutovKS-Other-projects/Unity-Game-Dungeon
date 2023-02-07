using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobeStatisticUI : MonoBehaviour
{
    private MobeStatistic statistic;
    private GameObject nameText;
    private GameObject healthText;

    private void Start()
    {
        statistic = GetComponentInParent<MobeStatistic>();
        
        nameText = transform.Find("Name").gameObject;
        healthText = transform.Find("Health").gameObject;

        nameText.GetComponent<TextMesh>().text = $"{statistic.Name}";
    }

    private void FixedUpdate()
    {
        if (statistic.isDead)
            Destroy(gameObject);

        if (statistic.isPlayerDetected)
            Display();
        else
            NoDisplay();
    }

    private void Display()
    {
        nameText.SetActive(true);
        healthText.SetActive(true);
        transform.LookAt(Camera.main.transform.position);
        healthText.GetComponent<TextMesh>().text = $"XP: {statistic.Health}";
        if(statistic.Health != statistic.statisticObject.Health)
            healthText.GetComponent<TextMesh>().text = $"XP: {statistic.Health}/{statistic.statisticObject.Health}";
    }
    private void NoDisplay()
    {
        nameText.SetActive(false);
        healthText.SetActive(false);
    }
}
