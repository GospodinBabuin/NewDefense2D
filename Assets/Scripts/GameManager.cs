using BuildingSystem;
using SaveLoadSystem;
using System;
using System.Collections;
using Steamworks;
using UI;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour, IBind<GameManager.GameProgressDataStruct>
{
    public static GameManager Instance;
    
    public delegate void DefeatHandler();
    public DefeatHandler OnDefeatEvent;
    
    public bool FirstStart { get; private set; } = true;
    public bool HealPotionLvl1HasBeenUsed { get; private set; } = false;
    public bool HealPotionLvl2HasBeenUsed { get; private set; } = false;
    public bool IncreaseDamagePotionLvl1HasBeenUsed { get; set; } = false;
    public bool IncreaseDamagePotionLvl2HasBeenUsed { get; set; } = false;
    public bool IncreaseHealthPotionLvl1HasBeenUsed { get; set; } = false;
    public bool IncreaseHealthPotionLvl2HasBeenUsed { get; set; } = false;
    public bool IncreaseHealthPotionLvl3HasBeenUsed { get; set; } = false;
    public bool IncreaseSpeedPotionLvl1HasBeenUsed { get; set; } = false;
    public bool IncreaseSpeedPotionLvl2HasBeenUsed { get; set; } = false;
    
    private GameManagerData _gameManagerData = new GameManagerData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Defeat()
    {
        OnDefeatEvent?.Invoke();
        
        StartCoroutine(CameraTransitionHandler.Instance.Transit(Camera.main, CameraTransitionHandler.Instance.BaseCamera, 0));
        
        if (!IsHost) return;
        
        SaveLoad.Instance.DeleteGame();
        SaveLoad.Instance.NewGame();
        
        StartCoroutine(WaitThenGoToLobby());
        
        IEnumerator WaitThenGoToLobby()
        {
            yield return new WaitForSeconds(5);
            
            SceneTransitionHandler.Instance.SwitchScene("Lobby");
        }
    }

    public struct GameProgressDataStruct : INetworkSerializable, ISaveable
    {
        public ulong id;
        public bool firstStart;
        public bool healPotionLvl1HasBeenUsed;
        public bool healPotionLvl2HasBeenUsed;
        public bool increaseDamagePotionLvl1HasBeenUsed;
        public bool increaseDamagePotionLvl2HasBeenUsed;
        public bool increaseHealthPotionLvl1HasBeenUsed;
        public bool increaseHealthPotionLvl2HasBeenUsed;
        public bool increaseHealthPotionLvl3HasBeenUsed;
        public bool increaseSpeedPotionLvl1HasBeenUsed;
        public bool increaseSpeedPotionLvl2HasBeenUsed;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref firstStart);
            serializer.SerializeValue(ref healPotionLvl1HasBeenUsed);
            serializer.SerializeValue(ref healPotionLvl2HasBeenUsed);
            serializer.SerializeValue(ref increaseDamagePotionLvl1HasBeenUsed);
            serializer.SerializeValue(ref increaseDamagePotionLvl2HasBeenUsed);
            serializer.SerializeValue(ref increaseHealthPotionLvl1HasBeenUsed);
            serializer.SerializeValue(ref increaseHealthPotionLvl2HasBeenUsed);
            serializer.SerializeValue(ref increaseHealthPotionLvl3HasBeenUsed);
            serializer.SerializeValue(ref increaseSpeedPotionLvl1HasBeenUsed);
            serializer.SerializeValue(ref increaseSpeedPotionLvl2HasBeenUsed);
        }
    }

    private void StartGameProgressManager()
    {
        if (FirstStart) FirstStartBehaviour();
    }

    private void FirstStartBehaviour()
    {
        if (IsHost)
            BuildingSpawner.SpawnBuilding(3, new Vector2(2, -3.6f));
        
        FirstStart = false;
    }

    public void Bind(GameProgressDataStruct gameManagerData)
    {
        FirstStart = gameManagerData.firstStart;
        HealPotionLvl1HasBeenUsed = gameManagerData.healPotionLvl1HasBeenUsed;
        HealPotionLvl2HasBeenUsed = gameManagerData.healPotionLvl2HasBeenUsed;
        IncreaseHealthPotionLvl1HasBeenUsed = gameManagerData.increaseHealthPotionLvl1HasBeenUsed;
        IncreaseHealthPotionLvl2HasBeenUsed = gameManagerData.increaseHealthPotionLvl2HasBeenUsed;
        IncreaseHealthPotionLvl3HasBeenUsed = gameManagerData.increaseHealthPotionLvl3HasBeenUsed;
        IncreaseDamagePotionLvl1HasBeenUsed = gameManagerData.increaseDamagePotionLvl1HasBeenUsed;
        IncreaseDamagePotionLvl2HasBeenUsed = gameManagerData.increaseDamagePotionLvl2HasBeenUsed;
        IncreaseSpeedPotionLvl1HasBeenUsed = gameManagerData.increaseSpeedPotionLvl1HasBeenUsed;
        IncreaseSpeedPotionLvl2HasBeenUsed = gameManagerData.increaseSpeedPotionLvl2HasBeenUsed;
        
        StartGameProgressManager();
    }

    public void SaveData()
    {
        _gameManagerData.id = SteamClient.SteamId;
        _gameManagerData.firstStart = FirstStart;
        _gameManagerData.healPotionLvl1HasBeenUsed = HealPotionLvl2HasBeenUsed;
        _gameManagerData.healPotionLvl2HasBeenUsed = HealPotionLvl1HasBeenUsed;
        _gameManagerData.increaseDamagePotionLvl1HasBeenUsed = IncreaseDamagePotionLvl1HasBeenUsed;
        _gameManagerData.increaseDamagePotionLvl2HasBeenUsed = IncreaseDamagePotionLvl2HasBeenUsed;
        _gameManagerData.increaseHealthPotionLvl1HasBeenUsed = IncreaseHealthPotionLvl1HasBeenUsed;
        _gameManagerData.increaseHealthPotionLvl2HasBeenUsed = IncreaseHealthPotionLvl2HasBeenUsed;
        _gameManagerData.increaseHealthPotionLvl3HasBeenUsed = IncreaseHealthPotionLvl3HasBeenUsed;
        _gameManagerData.increaseSpeedPotionLvl1HasBeenUsed = IncreaseSpeedPotionLvl1HasBeenUsed;
        _gameManagerData.increaseSpeedPotionLvl2HasBeenUsed = IncreaseSpeedPotionLvl2HasBeenUsed;
    }

    public GameManagerData GetGameProgressData() => _gameManagerData;
}

[Serializable]
public class GameManagerData : ISaveable
{
    public ulong id;
    public bool firstStart = true;
    public bool healPotionLvl1HasBeenUsed;
    public bool healPotionLvl2HasBeenUsed;
    public bool increaseDamagePotionLvl1HasBeenUsed;
    public bool increaseDamagePotionLvl2HasBeenUsed;
    public bool increaseHealthPotionLvl1HasBeenUsed;
    public bool increaseHealthPotionLvl2HasBeenUsed;
    public bool increaseHealthPotionLvl3HasBeenUsed;
    public bool increaseSpeedPotionLvl1HasBeenUsed;
    public bool increaseSpeedPotionLvl2HasBeenUsed;
}
