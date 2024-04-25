using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button submitCodeButton;
        
        [SerializeField] private Text lobbyCodeText;
        private void OnEnable()
        {
            hostButton.onClick.AddListener(OnHostButtonClicked);
            submitCodeButton.onClick.AddListener(OnsubmitCodeButtonClicked);
        }

        private void OnDisable()
        {
            hostButton.onClick.RemoveListener(OnHostButtonClicked);
            submitCodeButton.onClick.RemoveListener(OnsubmitCodeButtonClicked);
        }

        private async void OnHostButtonClicked()
        {
            bool succeeded = await GameLobbyManager.Instance.CreateGameLobby();

            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }
        
        private void OnsubmitCodeButtonClicked()
        {
            string code = lobbyCodeText.text; 
            Debug.Log(code);
        }
    }
}
