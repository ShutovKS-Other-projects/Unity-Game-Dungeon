using UnityEngine;

public class UIGame : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerInteractionObject playerInteractionObject;
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject staminaText;
    [SerializeField] private GameObject collectCrystalText;
    [SerializeField] private GameObject interactionLabel;

    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerInteractionObject = GameObject.FindWithTag("Player").GetComponent<PlayerInteractionObject>();
    }

    public void UpdateGameStatistics()
    {
        healthText.GetComponent<UnityEngine.UI.Text>().text = "Health: " + playerController.statistic.Health;
        staminaText.GetComponent<UnityEngine.UI.Text>().text = "Stamina: " + playerController.statistic.Stamina;
        collectCrystalText.GetComponent<UnityEngine.UI.Text>().text = "Crystal count: " + playerController.statistic.CollectCrystal;
        interactionLabel.GetComponent<UnityEngine.UI.Text>().text = playerInteractionObject.interactionText;
    }
}
