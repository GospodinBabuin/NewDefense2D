using BuildingSystem;
using SaveLoadSystem;
using System;
using Unity.Netcode;
using UnityEngine;

public class GameProgress : NetworkBehaviour, IBind<GameProgress.GameProgressDataStruct>
{
    public static GameProgress Instance;

    [SerializeField] private bool firstStart = true;

    private GameProgressData _gameProgressData = new GameProgressData();

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

    public struct GameProgressDataStruct : INetworkSerializable, ISaveable
    {
        public bool firstStart;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref firstStart);
        }
    }

    private void StartGameProgressManager()
    {
        if (!IsHost) return;

        if (firstStart) FirstStart();
    }

    private void FirstStart()
    {
        BuildingSpawner.SpawnBuilding(3, new Vector2(2, -3.6f));
        firstStart = false;
    }

    public void Bind(GameProgressDataStruct gameProgressData)
    {
        firstStart = gameProgressData.firstStart;

        StartGameProgressManager();
    }

    public void SaveData()
    {
        _gameProgressData.firstStart = firstStart;
    }

    public GameProgressData GetGameProgressData() => _gameProgressData;
}

[Serializable]
public class GameProgressData : ISaveable
{
    public bool firstStart = true;
}
