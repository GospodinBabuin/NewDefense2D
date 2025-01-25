using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : NetworkBehaviour
{
    public static PlayerSpawnManager Instance;
    [SerializeField] private GameObject playerPrefab;

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
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsHost)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
        }
    }

    private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName is "Game")
        {
            foreach (ulong id in clientsCompleted)
            {
                SpawnPlayerServerRPC(id, true);
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRPC(ulong clientId, bool destroyWithScene)
    {
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<PlayerController>().SteamId = PlayerInfoHandler.Instance.ReturnSteamIdByLocalId(clientId);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, destroyWithScene);
        Debug.Log($"Player {clientId} spawned");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        if (IsHost)
        {
            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneLoaded;
        }
    }
}
