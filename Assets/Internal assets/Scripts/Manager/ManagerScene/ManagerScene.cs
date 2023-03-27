using UnityEngine;
using UnityEngine.Serialization;

namespace ManagerScene
{
    public class ManagerScene : MonoBehaviour
    {
        [FormerlySerializedAs("currentScene")] public SceneType currentSceneType;
    }
}