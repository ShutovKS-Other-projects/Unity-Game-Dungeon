namespace Interactable.Interactable
{
    public class PortalHome : InteractableBase
    {
        public override void OnInteract()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}