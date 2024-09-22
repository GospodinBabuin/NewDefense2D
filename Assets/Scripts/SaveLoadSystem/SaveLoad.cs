using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using Environment;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveLoadSystem
{
    public class SaveLoad : NetworkBehaviour
    {
        public static SaveLoad Instance { get; private set; }

        [SerializeField] public GameData GameData;
        
        private IDataService _dataService;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                _dataService = new FileDataService(new JsonSerializer());
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Bind<T, TData>(TData data) where T : NetworkBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                if (data == null)
                {
                    data = new TData();
                }

                entity.Bind(data);
            }
        }
        
        /*private void Bind<T, TData>(List<TData> datas) where T : NetworkBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData();
                    datas.Add(data);
                }

                entity.Bind(data);
            }
        }*/
        
        private void LoadPlayers(List<PlayerData> playerDatas)
        {
            if (!IsHost) return;
            
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None); /// TODO: use ObjInWorld

            foreach (PlayerController player in players)
            {
                //PlayerData data = playerDatas.FirstOrDefault(d => d.id == player.Id);
                //PlayerData data = playerDatas.Find(d => d.id == player.Id);
                PlayerData data = null;
                Debug.Log(player.SteamId);
                foreach (PlayerData playerData in playerDatas)
                {
                    if (playerData.id == player.SteamId)
                    {
                        data = playerData;
                        continue;
                    }
                }
                if (data == null)
                {
                    data = new PlayerData{id = player.SteamId};
                    playerDatas.Add(data);
                }

                BindPlayerClientRPC(player.SteamId,ConvertPlayerData(data));
                //player.Bind(data);
            }
        }

        [ClientRpc]
        private void BindPlayerClientRPC(ulong playerId, PlayerController.PlayerDataStruct data)
        {
            //ObjectsInWorld.Instance.Players[playerId].Bind(data);
            
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None); /// TODO: use ObjInWorld
            foreach (PlayerController player in players)
            {
                if (player.SteamId == playerId)
                {
                    player.Bind(data);
                    return;
                }
            }
        }

        private PlayerController.PlayerDataStruct ConvertPlayerData(PlayerData playerData)
        {
            return new PlayerController.PlayerDataStruct
            {
                id = playerData.id,
                maxHealth = playerData.maxHealth,
                currentHealth = playerData.currentHealth,
                goldCount = playerData.goldCount
            };
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SavePlayerServerRpc(ulong id, int maxHealth, int currentHealth, int gold)
        {
            PlayerData data = GameData.playerData.FirstOrDefault(d => d.id == id);
            data.maxHealth = maxHealth;
            data.currentHealth = currentHealth;
            data.goldCount = gold;
            Debug.Log($"Players data saved, ID: {id}");
        }

        private void LoadBuildings(List<BuildingData> buildingDatas)
        {
            foreach (BuildingData buildingData in buildingDatas)
            {
                //BuildingSpawner.Instance.SpawnBuildingServerRPC(buildingData);
            }
        }

        public void CheckIfAllPlayersLoadedInGame()
        {
            foreach (KeyValuePair<ulong, GameObject> player in PlayerInfoHandler.Instance.PlayerInfos)
            {
                if (!player.Value.GetComponent<PlayerInfo>().isInGame)
                {
                    return;
                }
            }

            BindPlayersData();
        }

        [ContextMenu("NewGame")]
        public void NewGame() => GameData = new GameData();
        
        [ContextMenu("SaveGame")]
        public void SaveGame() => _dataService.Save(GameData);
        
        [ContextMenu("LoadGame")]
        public void LoadGame() => GameData = _dataService.Load();
        
        [ContextMenu("DeleteGame")]
        public void DeleteGame() => _dataService.Delete();

        public bool IsSaveFileExists() => _dataService.IsSaveFileExists();

        [ContextMenu("BindPlayers")]
        public void BindPlayersData() => LoadPlayers(GameData.playerData);
    }

    public interface ISaveable { }

    public interface IBind<TData> where TData : ISaveable
    { 
        void Bind(TData data);
        void SaveData();
    }

    [Serializable]
    public class GameData
    { 
        public List<PlayerData> playerData = new List<PlayerData>();
        public List<BuildingData> buildingData = new List<BuildingData>();
    }
}