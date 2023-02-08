using UnityEngine;

public class UIController : MonoBehaviour
{
    private InputManager inputManager;

    [SerializeField] private GameObject uiPanelGame;
    [SerializeField] private GameObject uiPanelMenu;
    [SerializeField] private GameObject uiPanelPlayerInfo;

    private UIGame uiGameScript;
    private UIMenu uiMenuScript;
    private UIPlayerInfo uiPlayerInfoScript;

    void Start()
    {
        inputManager = InputManager.Instance;

        AddScript();
        OnGame();
    }

    void Update()
    {
        if (inputManager.GetAllMenuInput())
        {
            if (uiPanelMenu.activeSelf)
            {
                OnGame();
            }
            else
            {
                OnMenu();
            }
        }

        if (inputManager.GetAllPlayerInfoInput())
        {
            if (uiPanelPlayerInfo.activeSelf)
            {
                OnGame();
            }
            else
            {
                OnPlayerInfo();
            }
        }
    }

    public void OnGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        uiPanelGame.SetActive(true);
        uiPanelMenu.SetActive(false);
        uiPanelPlayerInfo.SetActive(false);
    }

    public void OnMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        uiPanelGame.SetActive(false);
        uiPanelMenu.SetActive(true);
        uiPanelPlayerInfo.SetActive(false);
    }

    public void OnPlayerInfo()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        uiPanelGame.SetActive(false);
        uiPanelMenu.SetActive(false);
        uiPanelPlayerInfo.SetActive(true);

        try
        {
            uiPlayerInfoScript.UpdateInventory();
        }
        catch (System.Exception)
        {
            Debug.Log("UpdateInventory error");
        }
    }

    private void AddScript()
    {
        try
        {
            uiGameScript = uiPanelGame.GetComponent<UIGame>();
        }
        catch (System.Exception)
        {
            Debug.Log("UIGame script not found");
        }

        try
        {
            uiMenuScript = uiPanelMenu.GetComponent<UIMenu>();
        }
        catch (System.Exception)
        {
            Debug.Log("UIMenu script not found");
        }

        try
        {
            uiPlayerInfoScript = uiPanelPlayerInfo.GetComponent<UIPlayerInfo>();
        }
        catch (System.Exception)
        {
            Debug.Log("UIPlayerInfo script not found");
        }
    }
}
