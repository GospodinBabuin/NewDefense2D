using Buildings;
using Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static Buildings.Building;

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

        private void LoadPlayers(List<PlayerData> playerDatas)
        {
            if (!IsHost) return;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values)
            {
                PlayerController player = client.PlayerObject.GetComponent<PlayerController>();
                PlayerData data = null;
                foreach (PlayerData playerData in playerDatas)
                {
                    if (playerData.id == player.SteamId)
                    {
                        data = playerData;
                        break;
                    }
                }
                if (data == null)
                {
                    data = new PlayerData { id = player.SteamId };
                    playerDatas.Add(data);
                }

                BindPlayerClientRPC(client.ClientId, ConvertPlayerData(data));
            }
        }

        [ClientRpc]
        private void BindPlayerClientRPC(ulong clientId, PlayerController.PlayerDataStruct data)
        {
            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerController>().Bind(data);
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
                BuildingDataStruct convertedBuildingData = ConvertBuildingData(buildingData);
                Building building;

                if (buildingData.id == -1) continue;

                if (buildingData.id == 6)
                {
                    building = GameObject.FindGameObjectWithTag("Base").GetComponent<Building>();
                }
                else
                {
                    building = BuildingSystem.BuildingSpawner.SpawnBuilding(convertedBuildingData);
                }

                building.Bind(convertedBuildingData);
            }
        }

        private BuildingDataStruct ConvertBuildingData(BuildingData buildingData)
        {
            return new BuildingDataStruct
            {
                id = buildingData.id,
                level = buildingData.level,
                currentHealth = buildingData.currentHealth,
                position = buildingData.position
            };
        }

        private void SaveBuildingData()
        {
            GameData.buildingData.Clear();
            foreach (Building building in ObjectsInWorld.Instance.Buildings)
            {
                building.SaveData();
                BuildingData buildingData = building.GetBuildingData();
                GameData.buildingData.Add(buildingData);
            }
        }

        [ContextMenu("NewGame")]
        public void NewGame() => GameData = new GameData();

        [ContextMenu("SaveGame")]
        public void SaveGame()
        {
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (PlayerController player in players)
            {
                player.SaveData();
            }

            SaveBuildingData();

            _dataService.Save(GameData);
        }

        [ContextMenu("LoadGame")]
        public void LoadGame() => GameData = _dataService.Load();

        [ContextMenu("DeleteGame")]
        public void DeleteGame() => _dataService.Delete();

        public bool IsSaveFileExists() => _dataService.IsSaveFileExists();

        [ContextMenu("BindPlayers")]
        public void BindPlayersData() => LoadPlayers(GameData.playerData);
        public void BindBuildingsData() => LoadBuildings(GameData.buildingData);
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