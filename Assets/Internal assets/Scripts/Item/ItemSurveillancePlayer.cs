using UnityEngine;
namespace Item
{
    public class ItemSurveillancePlayer : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.forward = UnityEngine.Camera.main.transform.forward;
        }
    }
}
