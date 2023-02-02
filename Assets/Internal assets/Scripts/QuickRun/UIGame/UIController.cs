using UnityEngine;

public class UIController : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] private GameObject uiPanelGame;
    [SerializeField] private GameObject uiPanelMenu;
    private UIGame uiGame;
    private UIPlayerInfo uiMenu;
    private bool isGameOpen = true;

    void Start()
    {
        uiGame = GetComponent<UIGame>();
        uiMenu = GetComponent<UIPlayerInfo>();
        inputManager = InputManager.Instance;

        Cursor.lockState = CursorLockMode.Locked;
        uiPanelGame.SetActive(true);
        uiPanelMenu.SetActive(false);
    }

    void Update()
    {
        if (inputManager.GetPlayerMenuInput())
        {
            isGameOpen = !isGameOpen;

            if (isGameOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                uiPanelGame.SetActive(true);
                uiPanelMenu.SetActive(false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                uiPanelGame.SetActive(false);
                uiPanelMenu.SetActive(true);
            }

        }

        if (isGameOpen)
        {
            uiGame.UpdateGameStatistics();
        }
        else
        {
            uiMenu.UpdatePlayerInfo();
        }
    }
}
