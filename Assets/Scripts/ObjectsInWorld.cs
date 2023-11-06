using System.Collections.Generic;
using UnityEngine;

public class ObjectsInWorld : MonoBehaviour
{
    public static ObjectsInWorld Instance { get; private set; }

    [SerializeField] private List<Building> _buildings = new List<Building>();
    [SerializeField] private List<AlliedSoldier> _alliedSoldiers = new List<AlliedSoldier>();
    [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
    [SerializeField] private List<PlayerController> _players = new List<PlayerController>();

    public delegate void BuildingsHandler(List<Building> buildings);
    public event BuildingsHandler OnBuildingsListChangedEvent;

    public delegate void AlliedSoldersHandler(List<AlliedSoldier> alliedSoldiers, AlliedSoldier soldier);
    public event AlliedSoldersHandler OnAlliedSoldersListChangedEvent;

    public delegate void EnemiesHandler(List<Enemy> enemies);
    public event EnemiesHandler OnEnemiesListChangedEvent;

    public delegate void PlayersHandler(List<PlayerController> players);
    public event PlayersHandler OnPlayersListChangedEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            return;
        }

        Destroy(gameObject);
    }

    public void AddBuildingToList(Building newBuilding)
    {
        _buildings.Add(newBuilding);

        OnBuildingsListChangedEvent?.Invoke(_buildings);
    }

    public void RemoveBuildingFromList(Building newBuilding)
    {
        _buildings.Remove(newBuilding);

        OnBuildingsListChangedEvent?.Invoke(_buildings);
    }

    public void AddSolderToList(AlliedSoldier soldier)
    {
        _alliedSoldiers.Add(soldier);

        OnAlliedSoldersListChangedEvent?.Invoke(_alliedSoldiers, soldier);
    }

    public void RemoveSolderFromList(AlliedSoldier soldier)
    {
        _alliedSoldiers.Remove(soldier);

        OnAlliedSoldersListChangedEvent?.Invoke(_alliedSoldiers, soldier);
    }

    public void AddEnemyToList(Enemy enemy)
    {
        _enemies.Add(enemy);

        OnEnemiesListChangedEvent?.Invoke(_enemies);
    }

    public void RemoveEnemyFromList(Enemy enemy)
    {
        _enemies.Remove(enemy);

        OnEnemiesListChangedEvent?.Invoke(_enemies);
    }
    public void AddPlayerToList(PlayerController player)
    {
        _players.Add(player);

        OnPlayersListChangedEvent?.Invoke(_players);
    }

    public void RemovePlayerFromList(PlayerController player)
    {
        _players.Remove(player);

        OnPlayersListChangedEvent?.Invoke(_players);
    }

}
