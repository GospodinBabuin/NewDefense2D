using System.Threading.Tasks;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class GameNetworkManager : MonoBehaviour
    {
        public static GameNetworkManager Instance { get; private set; } = null;
        public Lobby? CurrentLobby { get; private set; } = null;

        private FacepunchTransport _transport = null;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            _transport = GetComponent<FacepunchTransport>();
            SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
            SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
        }

        public async Task<bool> StartHost(bool createLobby)
        {
            NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
            NetworkManager.Singleton.StartHost();

            if (createLobby)
            {
                PlayerSpawnManager.Instance.SpawnPlayerServerRPC(NetworkManager.Singleton.LocalClientId, true);
                CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(2);
                Debug.Log(CurrentLobby);
            }
        
            return CurrentLobby != null;
        }

        private void StartClient(SteamId steamId)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
            //NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
            NetworkManager.Singleton.Shutdown();
            _transport.targetSteamId = steamId;
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client has started");
            }
        }

        public void Disconnected()
        {
            NetworkTransmission.Instance.ClientDisconnectedClientRPC(NetworkManager.Singleton.LocalClientId);
        
            CurrentLobby?.Leave();
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
                }
                else
                {
                    NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
                }

                NetworkManager.Singleton.Shutdown();
            }

            Chat.Instance?.ClearChat();
            PlayerInfoHandler.Instance?.ClearPlayerInfos();
        
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.Disconnected();
            }
        
            CurrentLobby = null;
        
            Debug.Log("Disconnected");
        }

        private void Singleton_OnClientConnectedCallback(ulong clientId)
        {
            NetworkTransmission.Instance.AddMeToDictionaryServerRpc(SteamClient.SteamId, SteamClient.Name, clientId);
            NetworkTransmission.Instance.ClientReadyStateServerRpc(false, clientId);
            PlayerSpawnManager.Instance.SpawnPlayerServerRPC(NetworkManager.Singleton.LocalClientId, true);

            Debug.Log($"Client has connected: {clientId}");
        }

        private void Singleton_OnServerStarted()
        {
            LobbyManager.Instance.HostCreated();
            Debug.Log("Host started");
        }

        private void OnDestroy()
        {
            SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
            SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

            if (NetworkManager.Singleton == null)
            {
                return;
            }
            NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        }

        private void OnApplicationQuit()
        {
            Disconnected();
        }
    
        // when you accept the invite or join on a friend 
        private async void SteamFriends_OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
        {
            RoomEnter joinedLobby = await lobby.Join();
            if (joinedLobby != RoomEnter.Success)
            {
                Debug.Log("Failed to join a lobby");
            }
            else
            {
                CurrentLobby = lobby;
                LobbyManager.Instance?.ConnectedAsClient();
                Debug.Log("Joined lobby");
            }
        }

        // lobby created
        private void SteamMatchmaking_OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id)
        {
            Debug.Log("Lobby was created");
            Chat.Instance.SendMessageToChat($"Lobby was created", NetworkManager.Singleton.LocalClientId, true);
        }

        // friend send you steam invite
        private void SteamMatchmaking_OnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log($"Invite from {friend.Name}");
        }

        private void SteamMatchmaking_OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            Debug.Log($"Member {friend.Name} leave");
            Chat.Instance.SendMessageToChat($"{friend.Name} has left", friend.Id, true);
            NetworkTransmission.Instance.RemoveMeFromDictionaryServerRpc(friend.Id);
            LobbyManager.Instance?.CheckMembersCount();
        }

        private void SteamMatchmaking_OnLobbyMemberJoined(Lobby lobby, Friend friend)
        {
            Debug.Log($"Member {friend.Name} join");
            LobbyManager.Instance?.CheckMembersCount();
        }

        private void SteamMatchmaking_OnLobbyEntered(Lobby lobby)
        {
            LobbyManager.Instance?.CheckMembersCount();

            if (NetworkManager.Singleton.IsHost)
            {
                return;
            }

            if (CurrentLobby != null) 
                StartClient(CurrentLobby.Value.Owner.Id);
        }

        private void SteamMatchmaking_OnLobbyCreated(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                NetworkManager.Singleton.Shutdown();
                Debug.Log("Lobby was not created");
            }
            else
            {
                lobby.SetPublic();
                lobby.SetJoinable(true);
                lobby.SetGameServer(lobby.Owner.Id);
                Debug.Log($"Lobby created {lobby.Owner.Name}");
            }
        
            NetworkTransmission.Instance.AddMeToDictionaryServerRpc(SteamClient.SteamId, SteamClient.Name, NetworkManager.Singleton.LocalClientId);
        }
    }
}
