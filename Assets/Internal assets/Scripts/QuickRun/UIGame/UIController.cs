using UnityEngine;

public class UIController : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] private GameObject uiPanelGame;
    [SerializeField] private GameObject uiMenu;
    private UIGame uiGame;
    private UIPlayerInfo uiPlayerInfo;
    private bool isGameOpen = true;
    private bool isPlayerInfo = false;
    
    void Start()
    {
        uiGame = GetComponent<UIGame>();
        uiPlayerInfo = GetComponent<UIPlayerInfo>();
        inputManager = InputManager.Instance;
    }

    void Update()
    {
        if (inputManager.GetPlayerMenuInput())
        {
            isGameOpen = !isGameOpen;
            isPlayerInfo = !isPlayerInfo;
        }

        if (isGameOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;

            uiPanelGame.SetActive(true);
            uiMenu.SetActive(false);
            uiGame.UpdateGameStatistics();
        }
        if (isPlayerInfo)
        {
            Cursor.lockState = CursorLockMode.None;
            uiPanelGame.SetActive(false);
            uiMenu.SetActive(true);
        }
    }
}
