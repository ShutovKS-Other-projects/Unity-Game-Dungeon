using Scene;
using UI.Home_Scene;
using UnityEngine;
using XR;

namespace Interactive.Interactive
{
    public class InteractiveSkillsBook : MonoBehaviour, IInteractive
    {
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;

        public void OnInteract()
        {
            UIHomeSceneController.Instance.SwitchSkillsBookUI(true);
            SceneController.SwitchCursor(true);
        }
        
        public void OnInteractXR(SideType sideType)
        {
            UIHomeSceneController.Instance.SwitchSkillsBookUI(true);
            SceneController.SwitchCursor(true);
        }
    }
}