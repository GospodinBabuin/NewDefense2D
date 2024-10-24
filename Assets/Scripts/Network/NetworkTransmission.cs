using Environment;
using SaveLoadSystem;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkTransmission : NetworkBehaviour
{
    public static NetworkTransmission Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void IWishToSendAChatServerRpc(string message, ulong fromWho)
    {
        ChatFromServerClientRpc(message, fromWho);
    }

    [ClientRpc]
    private void ChatFromServerClientRpc(string message, ulong fromWho)
    {
        Chat.Instance.SendMessageToChat(message, fromWho, false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddMeToDictionaryServerRpc(ulong steamId, string steamName, ulong clientId)
    {
        Chat.Instance.SendMessageToChat($"{steamName} has joined", clientId, true);
        PlayerInfoHandler.Instance.AddPlayerToDictionary(clientId, steamName, steamId);
        PlayerInfoHandler.Instance.UpdateClients();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveMeFromDictionaryServerRpc(ulong steamId)
    {
        RemovePlayerFromDictionaryClientRpc(steamId);
    }

    [ClientRpc]
    private void RemovePlayerFromDictionaryClientRpc(ulong steamId)
    {
        Debug.Log("Removing client");
        PlayerInfoHandler.Instance.RemovePlayerFromDictionary(steamId);
    }

    [ClientRpc]
    public void UpdateClientsPlayerInfoClientRpc(ulong steamId, string steamName, ulong clientId)
    {
        PlayerInfoHandler.Instance.AddPlayerToDictionary(clientId, steamName, steamId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerLoadedInGameServerRPC(bool inGame, ulong clientId)
    {
        if (SceneManager.GetActiveScene().name == "Lobby") return;

        UpdatePlayerInfoInGameStateClientRpc(inGame, clientId);
    }

    [ClientRpc]
    private void UpdatePlayerInfoInGameStateClientRpc(bool inGame, ulong clientId)
    {
        foreach (KeyValuePair<ulong, GameObject> player in PlayerInfoHandler.Instance.PlayerInfos)
        {
            if (player.Key == clientId)
            {
                player.Value.GetComponent<PlayerInfo>().isInGame = inGame;
            }
        }

        if (!NetworkManager.Singleton.IsHost) return;

        if (IsAllPlayersLoadedInGame())
        {
            SaveLoad.Instance.BindPlayersData();
            ObjectsInWorld.Instance.AddAllPlayersInGameToDictionary();
            SaveLoad.Instance.BindBuildingsData();
            SaveLoad.Instance.BindUnitData();
            SaveLoad.Instance.BindGameProgressData();
        }
    }

    public bool IsAllPlayersLoadedInGame()
    {
        foreach (KeyValuePair<ulong, GameObject> player in PlayerInfoHandler.Instance.PlayerInfos)
        {
            if (!player.Value.GetComponent<PlayerInfo>().isInGame)
            {
                return false;
            }
        }
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayersScreenFadedServerRPC(bool faded, ulong clientId)
    {
        UpdatePlayerInfoFadedScreenStateClientRpc(faded, clientId);
    }

    [ClientRpc]
    private void UpdatePlayerInfoFadedScreenStateClientRpc(bool faded, ulong clientId)
    {
        foreach (KeyValuePair<ulong, GameObject> player in PlayerInfoHandler.Instance.PlayerInfos)
        {
            if (player.Key == clientId)
            {
                player.Value.GetComponent<PlayerInfo>().isPlayerScreenFaded = faded;

                if (!NetworkManager.Singleton.IsHost) continue;

                SceneTransitionHandler.Instance.CheckIfPlayersScreensAreFaded();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientReadyStateServerRpc(bool ready, ulong clientId)
    {
        UpdatePlayerInfoReadyStateClientRpc(ready, clientId);
    }

    [ClientRpc]
    private void UpdatePlayerInfoReadyStateClientRpc(bool ready, ulong clientId)
    {
        foreach (KeyValuePair<ulong, GameObject> player in PlayerInfoHandler.Instance.PlayerInfos)
        {
            if (player.Key == clientId)
            {
                player.Value.GetComponent<PlayerInfo>().isReady = ready;
                player.Value.GetComponent<PlayerInfo>().readyIndicator.SetActive(ready);

                if (NetworkManager.Singleton.IsHost)
                {
                    Debug.Log(LobbyManager.Instance.CheckIfPlayersAreReady());
                }
            }
        }
    }
}
