using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactive
{
    [CreateAssetMenu(fileName = "new InteractiveObject", menuName = "Data/Interactive/Interactive Data", order = 0)]
    public class InteractiveObject : ScriptableObject
    {
        public bool isInteract; 
        public bool isGrab;

        [TextArea(5, 5)] [CanBeNull] public string tooltipTextInteract;
        [TextArea(5, 5)] [CanBeNull] public string tooltipTextTake;
        
        [TextArea(5, 5)] [CanBeNull] public string tooltipTextInteractXR;
        [TextArea(5, 5)] [CanBeNull] public string tooltipTextTakeXR;
    }
}