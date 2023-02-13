using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobeStatisticUI : MonoBehaviour
{
    private EnemyData statistic;
    private GameObject nameText;
    private GameObject healthText;

    private void Start()
    {
        statistic = GetComponentInParent<EnemyStateController>().enemyData;
        
        nameText = transform.Find("Name").gameObject;
        healthText = transform.Find("Health").gameObject;

        nameText.GetComponent<TextMesh>().text = $"{statistic.raceName}";
    }

    private void FixedUpdate()
    {
        if (statistic.health > 0)
            Display();
        else
            NoDisplay();
    }

    private void Display()
    {
        nameText.SetActive(true);
        healthText.SetActive(true);
        transform.LookAt(Camera.main.transform.position);
        healthText.GetComponent<TextMesh>().text = $"XP: {statistic.health}";
    }
    private void NoDisplay()
    {
        nameText.SetActive(false);
        healthText.SetActive(false);
    }
}
