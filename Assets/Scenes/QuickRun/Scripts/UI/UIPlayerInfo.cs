using UnityEngine;

public class UIPlayerInfo : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerInventory playerInventory;

    [SerializeField] private GameObject[] inventorySlote;

    [SerializeField] private GameObject classText;

    [SerializeField] private GameObject helmet;
    [SerializeField] private GameObject scapular;
    [SerializeField] private GameObject chestplate;
    [SerializeField] private GameObject leggings;
    [SerializeField] private GameObject boots;
    [SerializeField] private GameObject ring;
    [SerializeField] private GameObject amulet;
    [SerializeField] private GameObject belt;
    [SerializeField] private GameObject bracers;
    [SerializeField] private GameObject gloves;

    [SerializeField] private GameObject arm;

    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject mannaText;
    [SerializeField] private GameObject staminaText;
    [SerializeField] private GameObject armorText;
    [SerializeField] private GameObject attackText;
    [SerializeField] private GameObject killCountText;

    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void UpdateEquipment()
    {
        
    }

        public void UpdatePlayerInfoStatistics()
    {
        classText.GetComponent<UnityEngine.UI.Text>().text = "Class: неопределён";
        healthText.GetComponent<UnityEngine.UI.Text>().text = "Health: " + playerController.statistics.health;
        mannaText.GetComponent<UnityEngine.UI.Text>().text = "Manna: " + playerController.statistics.manna;
        staminaText.GetComponent<UnityEngine.UI.Text>().text = "Stamina: " + playerController.statistics.stamina;
        armorText.GetComponent<UnityEngine.UI.Text>().text = "Armor: " + playerController.statistics.armor;
        attackText.GetComponent<UnityEngine.UI.Text>().text = "Attack: " + playerController.statistics.attack;
        killCountText.GetComponent<UnityEngine.UI.Text>().text = "Kill count: " + playerController.statistics.killCount;
    }

        public void UpdateInventoryAll()
    {
        for (int i = 0; i < inventorySlote.Length; i++)
        {
            if (i < playerInventory.inventory.Length)
            {
                inventorySlote[i].GetComponent<UnityEngine.UI.Image>().sprite = playerInventory.inventory[i].itemIcon;
            }
            else
            {
                inventorySlote[i].GetComponent<UnityEngine.UI.Image>().sprite = null;
            }
        }
    }
}
