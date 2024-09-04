using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }
    public bool Connected;
    public bool InGame;
    public bool IsHost;
    

    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject multiMenuButtons;
    [SerializeField] private GameObject multiLobbyButtons;
    
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
    }

    public void HostCreated()
    {
        multiMenuButtons.SetActive(false);
        multiLobbyButtons.SetActive(true);
        IsHost = true;
        Connected = true;
    }

    public void ConnectedAsClient()
    {
        multiMenuButtons.SetActive(false);
        multiLobbyButtons.SetActive(true);
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
        multiLobbyButtons.SetActive(false);
        IsHost = false;
        Connected = false;
    }

    public void OnReadyOrNotButtonClicked(bool ready)
    {
        NetworkTransmission.Instance.ClientReadyStateServerRpc(ready, NetworkManager.Singleton.LocalClientId);
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

    public void OnHostButtonClicked()
    {
        GameNetworkManager.Instance.StartHost(2);
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
        SceneTransitionHandler.Instance.SwitchScene("Game");
    }
}
