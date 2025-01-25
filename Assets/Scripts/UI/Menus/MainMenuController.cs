using SaveLoadSystem;
using Steamworks;
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
            SteamFriends.ClearRichPresence();
            SteamFriends.SetRichPresence( "steam_player_group", "In the main menu" );
            
            if (!SaveLoad.Instance.IsSaveFileExists())
            {
                continueGame.SetActive(false);
            }
        }
        
        public void OnContinueGameButtonClicked()
        {
            SaveLoad.Instance.LoadGame();
            
            SceneTransitionHandler.Instance.SwitchScene("Lobby");
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
