using Buildings;
using Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using static AlliedSoldier;
using static Buildings.Building;
using static GameManager;

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
                goldCount = playerData.goldCount,
                speed = playerData.speed,
                speedAnimationMultiplier = playerData.speedAnimationMultiplier,
                damage = playerData.damage
            };
        }

        [ServerRpc(RequireOwnership = false)]
        public void SavePlayerServerRpc(ulong id, int maxHealth, int currentHealth, int gold, int damage, float speed, float speedAnimationMultiplier)
        {
            PlayerData data = GameData.playerData.FirstOrDefault(d => d.id == id);
            data.maxHealth = maxHealth;
            data.currentHealth = currentHealth;
            data.goldCount = gold;
            data.speed = speed;
            data.speedAnimationMultiplier = speedAnimationMultiplier;
            data.damage = damage;
            Debug.Log($"Players data saved, ID: {id}");
        }

        [ClientRpc]
        private void SaveGameProgressDataClientRpc()
        {
            GameManager.Instance.SaveData();
            GameManagerData gameManagerData = GameManager.Instance.GetGameProgressData();
            SaveGameProgressDataServerRpc(ConvertGameProgressData(gameManagerData));
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SaveGameProgressDataServerRpc(GameProgressDataStruct data)
        {
            GameManagerData gameManagerData = GameData.gameProgressData.FirstOrDefault(d => d.id == data.id);
            
            if (gameManagerData != null)
            {
                SetGameProgressDataValues();
            }
            else
            {
                gameManagerData = new GameManagerData();
                SetGameProgressDataValues();
                GameData.gameProgressData.Add(gameManagerData);
            }

            return;

            void SetGameProgressDataValues()
            {
                gameManagerData.id = data.id;
                gameManagerData.firstStart = data.firstStart;
                gameManagerData.healPotionLvl1HasBeenUsed = data.healPotionLvl1HasBeenUsed;
                gameManagerData.healPotionLvl2HasBeenUsed = data.healPotionLvl2HasBeenUsed;
                gameManagerData.increaseDamagePotionLvl1HasBeenUsed = data.increaseDamagePotionLvl1HasBeenUsed;
                gameManagerData.increaseDamagePotionLvl2HasBeenUsed = data.increaseDamagePotionLvl2HasBeenUsed;
                gameManagerData.increaseHealthPotionLvl1HasBeenUsed = data.increaseHealthPotionLvl1HasBeenUsed;
                gameManagerData.increaseHealthPotionLvl2HasBeenUsed = data.increaseHealthPotionLvl2HasBeenUsed;
                gameManagerData.increaseHealthPotionLvl3HasBeenUsed = data.increaseHealthPotionLvl3HasBeenUsed;
                gameManagerData.increaseSpeedPotionLvl1HasBeenUsed = data.increaseSpeedPotionLvl1HasBeenUsed;
                gameManagerData.increaseSpeedPotionLvl2HasBeenUsed = data.increaseSpeedPotionLvl2HasBeenUsed;
            }
        }
        
        private void LoadGameProgressData(List<GameManagerData> progressData)
        {
            foreach (GameManagerData data in progressData)
            {
                GameProgressDataStruct convertedGameProgressData = ConvertGameProgressData(data);
                LoadGameProgressDataClientRPC(convertedGameProgressData);
            }
        }

        [ClientRpc]
        private void LoadGameProgressDataClientRPC(GameProgressDataStruct progressData)
        {
            if (SteamClient.SteamId != progressData.id) return;
            
            GameManager.Instance.Bind(progressData);
        }
        
        private GameProgressDataStruct ConvertGameProgressData(GameManagerData gameManagerData)
        {
            return new GameProgressDataStruct
            {
                id = gameManagerData.id,
                firstStart = gameManagerData.firstStart,
                healPotionLvl1HasBeenUsed = gameManagerData.healPotionLvl1HasBeenUsed,
                healPotionLvl2HasBeenUsed = gameManagerData.healPotionLvl2HasBeenUsed,
                increaseHealthPotionLvl1HasBeenUsed = gameManagerData.increaseHealthPotionLvl1HasBeenUsed,
                increaseHealthPotionLvl2HasBeenUsed = gameManagerData.increaseHealthPotionLvl2HasBeenUsed,
                increaseHealthPotionLvl3HasBeenUsed = gameManagerData.increaseHealthPotionLvl3HasBeenUsed,
                increaseDamagePotionLvl1HasBeenUsed = gameManagerData.increaseDamagePotionLvl1HasBeenUsed,
                increaseDamagePotionLvl2HasBeenUsed = gameManagerData.increaseDamagePotionLvl2HasBeenUsed,
                increaseSpeedPotionLvl1HasBeenUsed = gameManagerData.increaseSpeedPotionLvl1HasBeenUsed,
                increaseSpeedPotionLvl2HasBeenUsed = gameManagerData.increaseSpeedPotionLvl2HasBeenUsed
            };
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

        private void LoadUnits(List<UnitData> unitDatas)
        {
            foreach (UnitData unitData in unitDatas)
            {
                UnitDataStruct convertedUnitData = ConvertUnitData(unitData);

                AlliedSoldier unit = EntitySpawner.SpawnUnitOnServer(unitData.id);
                unit.Bind(convertedUnitData);
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

        private UnitDataStruct ConvertUnitData(UnitData unitData)
        {
            return new UnitDataStruct
            {
                id = unitData.id,
                currentHealth = unitData.currentHealth,
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

        private void SaveUnitData()
        {
            GameData.unitData.Clear();
            foreach (AlliedSoldier unit in ObjectsInWorld.Instance.AlliedSoldiers)
            {
                unit.SaveData();
                UnitData unitData = unit.GetUnitData();
                GameData.unitData.Add(unitData);
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
            SaveUnitData();
            SaveGameProgressDataClientRpc();

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
        public void BindUnitData() => LoadUnits(GameData.unitData);
        public void BindGameProgressData() => LoadGameProgressData(GameData.gameProgressData);
    }

    public interface ISaveable { }

    public interface IBind<TData> where TData : ISaveable
    {
        void Bind(TData gameManagerData);
        void SaveData();
    }

    [Serializable]
    public class GameData
    {
        public List<PlayerData> playerData = new List<PlayerData>();
        public List<BuildingData> buildingData = new List<BuildingData>();
        public List<UnitData> unitData = new List<UnitData>();
        public List<GameManagerData> gameProgressData = new List<GameManagerData>();
    }
}