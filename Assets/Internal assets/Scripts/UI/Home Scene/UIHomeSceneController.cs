using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Home_Scene
{
    public class UIHomeSceneController : MonoBehaviour
    {
        public static UIHomeSceneController Instance { get; private set; }

        private GameObject _interactUI;
        private GameObject _skillsBookUI;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        
        private void Start()
        {
            _interactUI = GameObject.Find("UIInteractionBare").gameObject;
            _skillsBookUI = GameObject.Find("UISkillsBook").gameObject;
            
            SwitchInteractUI(true);
            SwitchSkillsBookUI(false);
        }
        
        public void SwitchInteractUI(bool state)
        {
            _interactUI.SetActive(state);
        }
        
        public void SwitchSkillsBookUI(bool state)
        {
            _skillsBookUI.SetActive(state);
        }
    }
}
