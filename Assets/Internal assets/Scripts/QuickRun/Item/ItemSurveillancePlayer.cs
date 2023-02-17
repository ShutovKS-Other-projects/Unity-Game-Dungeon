using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Item
{
    public class ItemSurveillancePlayer : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.forward = UnityEngine.Camera.main.transform.forward;
        }
    }
}
