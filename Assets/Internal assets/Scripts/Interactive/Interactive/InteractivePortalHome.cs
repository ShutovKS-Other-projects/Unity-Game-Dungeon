using Scene;
using UnityEngine;
using Weapon;
using XR;

namespace Interactive.Interactive
{
    public class InteractivePortalHome : MonoBehaviour, IInteractive
    {
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;
        public WeaponTransformObject WeaponTransformObject { get; }

        public void OnInteract()
        {
            SceneController.SwitchScene(SceneType.StartGame);
        }
        
        public void OnInteractXR(SideType sideType)
        {
            SceneController.SwitchScene(SceneType.StartGame);
        }
    }
}