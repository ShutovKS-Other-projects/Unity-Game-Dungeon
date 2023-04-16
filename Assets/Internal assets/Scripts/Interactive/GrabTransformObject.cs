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

        public GrabTransformObject()
        {
            position = new Vector3();
            rotation = new Quaternion();
            scale = new Vector3(1, 1, 1);
        }

        public GrabTransformObject(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}