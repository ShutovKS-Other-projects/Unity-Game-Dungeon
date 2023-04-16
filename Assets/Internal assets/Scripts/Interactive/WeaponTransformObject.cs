using UnityEngine;

namespace Weapon
{
    [CreateAssetMenu(fileName = "WeaponTransform", menuName = "Data/Weapon/WeaponTransform", order = 0)]
    public class WeaponTransformObject : ScriptableObject
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
}