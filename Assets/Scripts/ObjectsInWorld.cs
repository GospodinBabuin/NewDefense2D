using System.Collections.Generic;
using UnityEngine;

public class ObjectsInWorld : MonoBehaviour
{
    public static ObjectsInWorld Instance { get; private set; }

    [SerializeField] private List<Building> _buildings = new List<Building>();
    [SerializeField] private List<AlliedSoldier> _alliedSoldiers = new List<AlliedSoldier>();
    [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
    [SerializeField] private List<Entity> _players = new List<Entity>();
    [SerializeField] private List<MonoBehaviour> _alliedObjects  = new List<MonoBehaviour>();
    [SerializeField] private List<GameObject> _deadBodies = new List<GameObject>();

    public List<Building> Buildings { get { return _buildings; } }
    public List<AlliedSoldier> AlliedSoldiers { get { return _alliedSoldiers; } }
    public List<Enemy> Enemies { get { return _enemies; } }
    public List<Entity> Players { get { return _players; } }
    public List<MonoBehaviour> AlliedObjects { get { return _alliedObjects; } }
    public List<GameObject> DeadBodies { get { return _deadBodies; } }


    public delegate void BuildingsHandler(List<Building> buildings);
    public event BuildingsHandler OnBuildingsListChangedEvent;

    public delegate void AlliedSoldiersHandler(List<AlliedSoldier> alliedSoldiers, AlliedSoldier soldier);
    public event AlliedSoldiersHandler OnAlliedSoldiersListChangedEvent;

    public delegate void EnemiesHandler(List<Enemy> enemies);
    public event EnemiesHandler OnEnemiesListChangedEvent;

    public delegate void PlayersHandler(List<Entity> players);
    public event PlayersHandler OnPlayersListChangedEvent;
    
    public delegate void DeadBodiesHandler(List<GameObject> deadBodies);
    public event DeadBodiesHandler OnDeadBodiesListChangedEvent;
    
    

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
        _alliedObjects.Add(newBuilding.GetComponent<MonoBehaviour>());

        OnBuildingsListChangedEvent?.Invoke(_buildings);
    }

    public void RemoveBuildingFromList(Building newBuilding)
    {
        _buildings.Remove(newBuilding);
        _alliedObjects.Remove(newBuilding.GetComponent<MonoBehaviour>());

        OnBuildingsListChangedEvent?.Invoke(_buildings);
    }

    public void AddSoldierToList(AlliedSoldier soldier)
    {
        _alliedSoldiers.Add(soldier);
        _alliedObjects.Add(soldier.GetComponent<MonoBehaviour>());

        OnAlliedSoldiersListChangedEvent?.Invoke(_alliedSoldiers, soldier);
    }

    public void RemoveSoldierFromList(AlliedSoldier soldier)
    {
        _alliedSoldiers.Remove(soldier);
        _alliedObjects.Remove(soldier.GetComponent<MonoBehaviour>());

        OnAlliedSoldiersListChangedEvent?.Invoke(_alliedSoldiers, soldier);
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
    public void AddPlayerToList(Entity player)
    {
        _players.Add(player);
        _alliedObjects.Add(player.GetComponent<MonoBehaviour>());


        OnPlayersListChangedEvent?.Invoke(_players);
    }

    public void RemovePlayerFromList(Entity player)
    {
        _players.Remove(player);
        _alliedObjects.Remove(player.GetComponent<MonoBehaviour>());

        OnPlayersListChangedEvent?.Invoke(_players);
    }
    
    public void AddDeadBodiesToList(GameObject deadBodies)
    {
        _deadBodies.Add(deadBodies);

        OnDeadBodiesListChangedEvent?.Invoke(_deadBodies);
    }

    public void RemoveDeadBodiesFromList(GameObject deadBodies)
    {
        _deadBodies.Remove(deadBodies);

        OnDeadBodiesListChangedEvent?.Invoke(_deadBodies);
    }
}
