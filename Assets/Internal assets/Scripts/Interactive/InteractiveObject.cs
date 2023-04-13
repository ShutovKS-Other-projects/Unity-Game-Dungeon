using JetBrains.Annotations;
using UnityEngine;

namespace Interactive
{
    [CreateAssetMenu(fileName = "new InteractiveObject", menuName = "Data/Interactive/Interactive Data", order = 0)]
    public class InteractiveObject : ScriptableObject
    {
        public bool isInteract;
        public bool isTake;

        [TextArea(5, 5)] [CanBeNull] public string tooltipTextInteract;
        [TextArea(5, 5)] [CanBeNull] public string tooltipTextTake;
    }
}