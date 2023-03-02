using Input;
using UI.PlayerInfo;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        InputManager _inputManager;

        GameObject _uiPanelGame;
        GameObject _uiPanelMenu;
        GameObject _uiPanelPlayerInfo;

        UI.Game.Statistic _statisticScript;
        UI.Menu.Menu _menuScript;
        Characteristics _characteristicsScript;

        void Start()
        {
            _inputManager = InputManager.Instance;

            _uiPanelGame = transform.Find("UIPanelGame").gameObject;
            _uiPanelMenu = transform.Find("UIPanelMenu").gameObject;
            _uiPanelPlayerInfo = transform.Find("UIPanelPlayerInfo").gameObject;

            _statisticScript = _uiPanelGame.GetComponent<UI.Game.Statistic>();
            _menuScript = _uiPanelMenu.GetComponent<UI.Menu.Menu>();
            _characteristicsScript = _uiPanelPlayerInfo.GetComponent<Characteristics>();

            OnGame();
        }

        void Update()
        {
            if (_inputManager.GetAllMenuInput())
            {
                if (_uiPanelMenu.activeSelf)
                {
                    OnGame();
                }
                else
                {
                    OnMenu();
                }
            }

            if (_inputManager.GetAllPlayerInfoInput())
            {
                if (_uiPanelPlayerInfo.activeSelf)
                {
                    OnGame();
                }
                else
                {
                    OnPlayerInfo();
                }
            }
        }

        void OnGame()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _uiPanelGame.SetActive(true);
            _uiPanelMenu.SetActive(false);
            _uiPanelPlayerInfo.SetActive(false);
        }

        public void OnMenu()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _uiPanelGame.SetActive(false);
            _uiPanelMenu.SetActive(true);
            _uiPanelPlayerInfo.SetActive(false);
        }

        public void OnPlayerInfo()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _uiPanelGame.SetActive(false);
            _uiPanelMenu.SetActive(false);
            _uiPanelPlayerInfo.SetActive(true);
        }
    }
}
