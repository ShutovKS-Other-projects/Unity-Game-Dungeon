using UnityEngine;

public class UIGame : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject staminaText;
    [SerializeField] private GameObject killCountText;

    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void UpdateGameStatistics()
    {
        healthText.GetComponent<UnityEngine.UI.Text>().text = "Health: " + playerController.statistics.health;
        staminaText.GetComponent<UnityEngine.UI.Text>().text = "Stamina: " + playerController.statistics.stamina;
        killCountText.GetComponent<UnityEngine.UI.Text>().text = "Kill count: " + playerController.statistics.killCount;
    }

}
