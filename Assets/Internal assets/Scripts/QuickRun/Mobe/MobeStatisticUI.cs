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
        transform.LookAt(Camera.main.transform.position);
        healthText.GetComponent<TextMesh>().text = $"XP: {statistic.Health.ToString()}";
        if (statistic.isDead)
        {
            Destroy(gameObject);
        }
    }
}
