using UnityEngine;
using UnityEngine.SceneManagement;

namespace Old.Menu
{
    public class UIManager : MonoBehaviour
    {
        public void StartStoryGame()
        {

        }
        public void StartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene($"Game");
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
