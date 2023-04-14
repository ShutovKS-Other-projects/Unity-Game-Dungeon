using UnityEngine;

namespace Interactive.Interactive
{
    public class InteractiveWeapon : MonoBehaviour, IInteractive
    {
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;

        public void OnGrab()
        {
            GrabsController.GrabRight(transform);
        }
    }
}