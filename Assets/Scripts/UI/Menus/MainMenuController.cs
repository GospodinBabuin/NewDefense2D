using SaveLoadSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Menus
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject continueGame;
        [SerializeField] private GameObject startNewGameConfirmation;

        private void Start()
        {
            if (!SaveLoad.Instance.IsSaveFileExists())
            {
                continueGame.SetActive(false);
            }
        }
        
        public void OnContinueGameButtonClicked()
        {
            SaveLoad.Instance.LoadGame();
            
            SceneManager.LoadSceneAsync("Lobby");
        }
        
        public void OnStartNewGameButtonClicked()
        {
            if (!continueGame.activeInHierarchy)
            {
                OnStartNewGameConfirmationButtonClicked();
            }
            else
            {
                startNewGameConfirmation.SetActive(true);
            }
        }
        
        public void OnStartNewGameConfirmationButtonClicked()
        {
            SaveLoad.Instance.DeleteGame();
            SaveLoad.Instance.NewGame();

            SceneTransitionHandler.Instance.SwitchScene("Lobby");
        }

        public void OnExitButtonClicked()
        {
            Application.Quit();
        }
    }
}
