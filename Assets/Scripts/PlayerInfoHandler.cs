using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static UnityEngine.Rendering.DebugUI;

public class PlayerInfoHandler : MonoBehaviour
{
    public static PlayerInfoHandler Instance { get; private set; }
    
    public readonly Dictionary<ulong, GameObject> PlayerInfos = new Dictionary<ulong, GameObject>();
    [SerializeField] private GameObject playerFieldBox, playerCardPrefab;

    [SerializeField] private Canvas playerFieldBoxCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneTransitionHandler.Instance.OnFadeOutStartedEvent += ShowOrHidePlayerFieldBoxHandler;
    }

    public void ClearPlayerInfos()
    {
        PlayerInfos.Clear();
    }
    
    public void AddPlayerToDictionary(ulong clientId, string steamName, ulong steamId)
    {
        if (!PlayerInfos.ContainsKey(clientId))
        {
            PlayerInfo playerInfo = Instantiate(playerCardPrefab, playerFieldBox.transform).GetComponent<PlayerInfo>();
            playerInfo.steamId = steamId;
            playerInfo.steamName = steamName;
            playerInfo.localId = clientId;
            PlayerInfos.Add(clientId, playerInfo.gameObject);
        }
    }
    
    public void UpdateClients()
    {
        foreach (KeyValuePair<ulong,GameObject> player in PlayerInfos)
        {
            ulong steamId = player.Value.GetComponent<PlayerInfo>().steamId;
            string steamName = player.Value.GetComponent<PlayerInfo>().steamName;
            ulong clientId = player.Key;
            
            NetworkTransmission.Instance.UpdateClientsPlayerInfoClientRpc(steamId, steamName, clientId);
        }
    }
    
    public void RemovePlayerFromDictionary(ulong steamId)
    {
        GameObject value = null;
        ulong key = 100;
        foreach (KeyValuePair<ulong,GameObject> player in PlayerInfos)
        {
            if (player.Value.GetComponent<PlayerInfo>().steamId == steamId)
            {
                value = player.Value;
                key = player.Key;
            }

            if (key != 100)
            {
                PlayerInfos.Remove(key);
            }

            if (value != null)
            {
                Destroy(value);
            }
        }
    }

    private void ShowOrHidePlayerFieldBoxHandler(string sceneName)
    {
        if (playerFieldBoxCanvas == null) return;
        
        if (sceneName == "Lobby")
        {
            playerFieldBoxCanvas.rootCanvas.enabled = true;
        }
        else
        {
            playerFieldBoxCanvas.rootCanvas.enabled = false;
        }
    }

    public ulong ReturnSteamIdByLocalId(ulong localId)
    {
        ulong tempSteamId = 0;
        foreach (KeyValuePair<ulong, GameObject> player in PlayerInfos)
        {
            if (player.Value.GetComponent<PlayerInfo>().localId == localId)
            {
                tempSteamId = player.Value.GetComponent<PlayerInfo>().steamId;
                break;
            }
        }
        
        return tempSteamId;
    }

    private void OnDestroy()
    {
        SceneTransitionHandler.Instance.OnFadeOutStartedEvent -= ShowOrHidePlayerFieldBoxHandler;
    }
}
