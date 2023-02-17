using UnityEngine;
using UnityEngine.SceneManagement;
namespace Internal_assets.Scripts.Menu
{
    public class UIManager : MonoBehaviour
    {
        public void StartStoryGame()
        {

        }
        public void StartGame()
        {
            SceneManager.LoadSceneAsync("QuickRun", LoadSceneMode.Single);
        }

        public void Settings()
        {
        
        }
    
        public void Exit()
        {
            Application.Quit();
        }
    }
}
