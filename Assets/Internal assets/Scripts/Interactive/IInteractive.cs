using JetBrains.Annotations;

namespace Interactive
{
    public interface IInteractive
    {
        protected InteractiveObject InteractiveObject { get; }

        public bool IsInteract => InteractiveObject.isInteract;
        public bool IsTake => InteractiveObject.isTake;

        [CanBeNull] public string TooltipTextInteract => InteractiveObject.tooltipTextInteract;
        [CanBeNull] public string TooltipTextTake => InteractiveObject.tooltipTextTake;

        public void OnInteract();
        public void OnTake();
    }
}