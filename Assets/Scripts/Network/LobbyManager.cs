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
        notReadyButton.SetActive(false);
        lobbyMenuButtons.SetActive(false);
        IsHost = false;
        Connected = false;
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
        }
        else
        {
            _notifications.ShowLobbyWasNotCreatedNotification();
        }
        
        lobbyPanel.SetActive(true);
    }
    
    public void OnReadyOrNotButtonClicked(bool ready)
    {
        NetworkTransmission.Instance.ClientReadyStateServerRpc(ready, NetworkManager.Singleton.LocalClientId);
    }

    public void OnExitToMenuButtonClicked()
    {
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
