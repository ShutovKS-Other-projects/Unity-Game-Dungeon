using UnityEngine;
using UnityEngine.Serialization;

namespace Interactive
{
    [CreateAssetMenu(fileName = "new GrabTransformObject", menuName = "Data/Interactive/GrabTransformObject Data",
        order = 0)]
    public class GrabTransformObject : ScriptableObject
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
}