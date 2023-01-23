using UnityEngine;

public class UIController : MonoBehaviour
{
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
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
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
