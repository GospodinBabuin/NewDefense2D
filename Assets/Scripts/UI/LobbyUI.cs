using MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Text lobbyCodeText;

        private void Start()
        {
            lobbyCodeText.text = $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";
            Debug.Log(GameLobbyManager.Instance.GetLobbyCode());
        }
    
    }
}
