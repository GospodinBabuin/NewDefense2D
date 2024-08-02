using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Text lobbyCodeText;

        public async void OnHostButtonClicked()
        {
            bool succeeded = await GameLobbyManager.Instance.CreateLobby();

            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }
        
        public async void OnSubmitCodeButtonClicked()
        {
            string code = lobbyCodeText.text;
            //code = code.Substring(0, code.Length - 1);
            Debug.Log(code);
            
            bool succeeded = await GameLobbyManager.Instance.JoinLobby(code);
            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }
    }
}
