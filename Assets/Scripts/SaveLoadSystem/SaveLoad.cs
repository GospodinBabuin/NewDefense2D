using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveLoadSystem
{
    public class SaveLoad : MonoBehaviour
    {
        public static SaveLoad Instance { get; private set; }

        [SerializeField] public GameData gameData;
        private IDataService _dataService;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

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
                    data = new TData { Id = entity.Id };
                }

                entity.Bind(data);
            }
        }
        
        private void Bind<T, TData>(List<TData> datas) where T : NetworkBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }

                entity.Bind(data);
            }
        }

        public void NewGame()
        {
            gameData = new GameData
            {
                name = "New Game",
                currentLevelName = "Lobby"
            };
            
            SceneTransitionHandler.Instance.SwitchScene("Lobby");
        }

        public void SaveGame() => _dataService.Save(gameData);

        public void LoadGame(string gameName)
        {
            gameData = _dataService.Load(gameName);

            if (String.IsNullOrWhiteSpace(gameData.currentLevelName))
            {
                gameData.currentLevelName = "Lobby";
            }
            
            SceneTransitionHandler.Instance.SwitchScene(gameData.currentLevelName);
        }

        public void DeleteGame(string gameName) => _dataService.Delete(gameName);

        private void OnEnable() => NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;

        private void OnSceneLoaded(ulong clientid, string scenename, LoadSceneMode loadscenemode)
        {
            if (scenename is "MainMenu" or "Lobby") return;
            
            Bind<PlayerController, PlayerData>(gameData.playerData);
        }

        private void OnDisable() => NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
    }

    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }
        void Bind(TData data);
    }

    [Serializable]
    public class GameData
    {
        public string name;
        public string currentLevelName;
        public PlayerData playerData;
    }
}