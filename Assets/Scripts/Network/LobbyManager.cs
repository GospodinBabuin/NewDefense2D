using System.Collections.Generic;
using Steamworks;
using UI;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }
    public bool Connected;
    public bool InGame;
    public bool IsHost;
    
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject notReadyButton;
    [SerializeField] private GameObject inviteButton;
    [SerializeField] private GameObject multiMenuButtons;
    [SerializeField] private GameObject lobbyMenuButtons;

    private Notifications _notifications;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        _notifications = GetComponentInChildren<Notifications>();
    }

    private void Start()
    {
        SteamFriends.ClearRichPresence();
        SteamFriends.SetRichPresence( "steam_player_group", "Resting at the camp" );

        if (GameNetworkManager.Instance.CurrentLobby != null && GameNetworkManager.Instance.CurrentLobby.Value.Id.IsValid)
        {
            PlayerSpawnManager.Instance.SpawnPlayerServerRPC(NetworkManager.Singleton.LocalClientId, true);
            
            multiMenuButtons.SetActive(false);
            lobbyMenuButtons.SetActive(true);
            readyButton.SetActive(true);
            CheckMembersCount();
        }
        else
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
            {
                GameNetworkManager.Instance.Disconnected();
            }
        }
    }

    public void HostCreated()
    {
        IsHost = true;
        Connected = true;
    }

    public void ConnectedAsClient()
    {
        multiMenuButtons.SetActive(false);
        lobbyMenuButtons.SetActive(true);
        readyButton.SetActive(true);
        IsHost = false;
        Connected = true;
    }

    public void CheckMembersCount()
    {
        Debug.Log("Checking members count");

        if (GameNetworkManager.Instance.CurrentLobby == null)
        {
            Debug.Log("lobby not found");
            return;
        }
        
        if (GameNetworkManager.Instance.CurrentLobby.Value.MaxMembers ==
            GameNetworkManager.Instance.CurrentLobby.Value.MemberCount)
        {
            inviteButton.SetActive(false);
        }

        Debug.Log($"{GameNetworkManager.Instance.CurrentLobby.Value.MemberCount} members");
        
        if (GameNetworkManager.Instance.CurrentLobby.Value.MemberCount > 1)
        {
            SteamFriends.SetRichPresence("steam_player_group_size", GameNetworkManager.Instance.CurrentLobby.Value.MemberCount.ToString());
        }
        else
        {
            SteamFriends.SetRichPresence("steam_player_group_size", null);
        }
    }

    public void Disconnected()
    {
        GameObject[] playersCards = GameObject.FindGameObjectsWithTag("PlayerCard");
        foreach (GameObject card in playersCards)
        {
            Destroy(card);
        }
        
        multiMenuButtons.SetActive(true);
        startButton.SetActive(false);
        readyButton.SetActive(false);
        inviteButton.SetActive(false);
        notReadyButton.SetActive(false);
        lobbyMenuButtons.SetActive(false);
        IsHost = false;
        Connected = false;
        
        Debug.Log("Disconnected from lobby");
    }

    public bool CheckIfPlayersAreReady()
    {
        bool ready = false;
        
        foreach (KeyValuePair<ulong,GameObject> player in PlayerInfoHandler.Instance.PlayerInfos)
        {
            if (!player.Value.GetComponent<PlayerInfo>().isReady)
            {
                startButton.SetActive(false);
                return false;
            }
            else
            {
                startButton.SetActive(true);
                ready = true;
            }
        }

        return ready;
    }

    public async void OnStartSingleplayer()
    {
        lobbyPanel.SetActive(false);

        await GameNetworkManager.Instance.StartHost(false);
        NetworkTransmission.Instance.AddMeToDictionaryServerRpc(SteamClient.SteamId, SteamClient.Name, NetworkManager.Singleton.LocalClientId);

        SceneTransitionHandler.Instance.SwitchScene("Game");
    }
    
    public async void OnHostButtonClicked()
    {
        lobbyPanel.SetActive(false);
        
        _notifications.ShowCreatingLobbyNotification();

        if (await GameNetworkManager.Instance.StartHost(true))
        {
            _notifications.ShowLobbyCreatedNotification();
            
            multiMenuButtons.SetActive(false);
            lobbyMenuButtons.SetActive(true);
            readyButton.SetActive(true);
            inviteButton.SetActive(true);
        }
        else
        {
            _notifications.ShowLobbyWasNotCreatedNotification();
        }
        
        lobbyPanel.SetActive(true);
    }

    public void OnJoinButtonClicked()
    {
        SteamFriends.OpenOverlay("friends");
    }

    public void OnInviteButtonClicked()
    {
        SteamFriends.OpenGameInviteOverlay(SteamClient.SteamId);
    }
    
    public void OnReadyOrNotButtonClicked(bool ready)
    {
        NetworkTransmission.Instance.ClientReadyStateServerRpc(ready, NetworkManager.Singleton.LocalClientId);
    }

    public void OnExitToMenuButtonClicked()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            GameNetworkManager.Instance.Disconnected();
        }
        
        SceneTransitionHandler.Instance.SwitchScene("MainMenu");
    }

    public void OnDisconnectButtonClicked()
    {
        GameNetworkManager.Instance.Disconnected();
    }

    public void OnStartButtonClicked()
    {
        lobbyPanel.SetActive(false);

        NetworkTransmission.Instance.SetLobbyJoinable(false);
        SceneTransitionHandler.Instance.SwitchScene("Game");
    }
}
