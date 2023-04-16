using Scene;
using UI.Home_Scene;
using UnityEngine;
using Weapon;
using XR;

namespace Interactive.Interactive
{
    public class InteractiveSkillsBook : MonoBehaviour, IInteractive
    {
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;
        public WeaponTransformObject WeaponTransformObject { get; }

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