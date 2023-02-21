using Input;
using Unity.VisualScripting;
using UnityEngine;
namespace UIGame
{
    public class UIController : MonoBehaviour
    {
        private InputManager _inputManager;

        private GameObject _uiPanelGame;
        private GameObject _uiPanelMenu;
        private GameObject _uiPanelPlayerInfo;

        private UIGame _uiGameScript;
        private UIMenu _uiMenuScript;
        private UIPlayerInfo _uiPlayerInfoScript;

        void Start()
        {
            _inputManager = InputManager.Instance;

            _uiPanelGame = transform.Find("UIPanelGame").gameObject; 
            _uiPanelMenu = transform.Find("UIPanelMenu").gameObject;
            _uiPanelPlayerInfo = transform.Find("UIPanelPlayerInfo").gameObject;

            _uiGameScript = _uiPanelGame.GetComponent<UIGame>();
            _uiMenuScript = _uiPanelMenu.GetComponent<UIMenu>();
            _uiPlayerInfoScript = _uiPanelPlayerInfo.GetComponent<UIPlayerInfo>();

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
            
            _uiPlayerInfoScript.InventoryUpdate();
        }
    }
}
