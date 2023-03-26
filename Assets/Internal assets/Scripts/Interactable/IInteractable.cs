namespace Interactable
{
    public interface IInteractable
    {
        public float HoldDuration { get; }

        public bool HoldInteract { get; }
        public bool MultipleUse { get; }
        public bool IsInteractable { get; }

        public string TooltipText { get; }

        void OnInteract();
    }
}