using System;
using Input;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        private InputManager _inputManager;

        private GameObject _uiPanelGame;
        private GameObject _uiPanelMenu;
        private GameObject _uiPanelPlayerInfo;

        private Game.Statistic _statisticScript;
        private Menu.Menu _menuScript;
        private PlayerInfo.Characteristics _characteristicsScript;

        private void Awake()
        {
            InputGame += OnGame;
            InputMenu += OnMenu;
            InputPlayerInfo += OnPlayerInfo;
        }

        private void Start()
        {
            _inputManager = InputManager.Instance;

            _uiPanelGame = transform.Find("UIPanelGame").gameObject;
            _uiPanelMenu = transform.Find("UIPanelMenu").gameObject;
            _uiPanelPlayerInfo = transform.Find("UIPanelPlayerInfo").gameObject;

            _statisticScript = _uiPanelGame.GetComponent<Game.Statistic>();
            _menuScript = _uiPanelMenu.GetComponent<Menu.Menu>();
            _characteristicsScript = _uiPanelPlayerInfo.GetComponent<PlayerInfo.Characteristics>();

            InputGame!();
        }

        private void Update()
        {
            // if (_inputManager.GetAllMenuInput())
            // {
            // if (_uiPanelMenu.activeSelf)
            // {
            // EnableGame!();
            // }
            // else
            // {
            // EnableMenu!();
            // }
            // }

            if (_inputManager.GetAllPlayerInfoInput())
            {
                if (_uiPanelPlayerInfo.activeSelf)
                {
                    InputGame!();
                }
                else
                {
                    InputPlayerInfo!();
                }
            }
        }

        public delegate void InputPanel();

        [CanBeNull] public event InputPanel InputGame;
        [CanBeNull] public event InputPanel InputMenu;
        [CanBeNull] public event InputPanel InputPlayerInfo;


        #region Metode

        private void OnGame()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _uiPanelGame.SetActive(true);
            _uiPanelMenu.SetActive(false);
            _uiPanelPlayerInfo.SetActive(false);
        }

        private void OnMenu()
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

        #endregion
    }
}