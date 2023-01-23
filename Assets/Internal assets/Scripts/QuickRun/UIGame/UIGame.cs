using UnityEngine;

public class UIGame : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerInteractionObject playerInteractionObject;
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject staminaText;
    [SerializeField] private GameObject killCountText;
    [SerializeField] private GameObject interactionLabel;

    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerInteractionObject = GameObject.FindWithTag("Player").GetComponent<PlayerInteractionObject>();
    }

    public void UpdateGameStatistics()
    {
        healthText.GetComponent<UnityEngine.UI.Text>().text = "Health: " + playerController.statistics.Health;
        staminaText.GetComponent<UnityEngine.UI.Text>().text = "Stamina: " + playerController.statistics.Stamina;
        killCountText.GetComponent<UnityEngine.UI.Text>().text = "Kill count: " + playerController.statistics.KillCount;
        interactionLabel.GetComponent<UnityEngine.UI.Text>().text = playerInteractionObject.interactionText;
    }

}
