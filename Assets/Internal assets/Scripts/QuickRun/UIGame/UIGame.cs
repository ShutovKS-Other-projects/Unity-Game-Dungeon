using UnityEngine;

public class UIGame : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject staminaText;
    [SerializeField] private GameObject collectCrystalText;
    [SerializeField] private GameObject dialogText;

    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        UpdateGameStatistics();
    }

    public void UpdateGameStatistics()
    {
        healthText.GetComponent<UnityEngine.UI.Text>().text = "Health: " + playerController.statistic.Health;
        staminaText.GetComponent<UnityEngine.UI.Text>().text = "Stamina: " + playerController.statistic.Stamina;
        collectCrystalText.GetComponent<UnityEngine.UI.Text>().text = "Crystal count: " + playerController.statistic.CollectCrystal;
        //dialogText.GetComponent<UnityEngine.UI.Text>().text = ;
    }
}
