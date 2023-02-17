using Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Enemy
{
    public class EnemyStatisticUI : MonoBehaviour
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

        // ReSharper disable Unity.PerformanceAnalysis
        private void Display()
        {
            nameText.SetActive(true);
            healthText.SetActive(true);
            transform.LookAt(UnityEngine.Camera.main.transform.position);
            healthText.GetComponent<TextMesh>().text = $"XP: {statistic.health}";
        }
        private void NoDisplay()
        {
            nameText.SetActive(false);
            healthText.SetActive(false);
        }
    }
}
