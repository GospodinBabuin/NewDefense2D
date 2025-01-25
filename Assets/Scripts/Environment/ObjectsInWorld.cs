using Buildings;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Environment
{
    public class ObjectsInWorld : NetworkBehaviour
    {
        public static ObjectsInWorld Instance { get; private set; }

        [SerializeField] private List<Building> _buildings = new List<Building>();
        [SerializeField] private List<AlliedSoldier> _alliedSoldiers = new List<AlliedSoldier>();
        [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
        [SerializeField] private List<MonoBehaviour> _alliedObjects = new List<MonoBehaviour>();
        [SerializeField] private List<DeadBody> _deadBodies = new List<DeadBody>();
        [SerializeField] private Dictionary<ulong, PlayerController> _players = new Dictionary<ulong, PlayerController>();

        public Dictionary<ulong, PlayerController> Players => _players;
        public List<Building> Buildings => _buildings;
        public List<AlliedSoldier> AlliedSoldiers => _alliedSoldiers;
        public List<Enemy> Enemies => _enemies;
        public List<MonoBehaviour> AlliedObjects => _alliedObjects;
        public List<DeadBody> DeadBodies => _deadBodies;


        public delegate void BuildingsHandler(List<Building> buildings);
        public event BuildingsHandler OnBuildingsListChangedEvent;

        public delegate void AlliedSoldiersHandler(List<AlliedSoldier> alliedSoldiers, AlliedSoldier soldier);
        public event AlliedSoldiersHandler OnAlliedSoldiersListChangedEvent;

        public delegate void EnemiesHandler(List<Enemy> enemies);
        public event EnemiesHandler OnEnemiesListChangedEvent;

        public delegate void PlayersHandler(Dictionary<ulong, PlayerController> players);
        public event PlayersHandler OnPlayersDictionaryChangedEvent;

        public delegate void DeadBodiesHandler(List<DeadBody> deadBodies);
        public event DeadBodiesHandler OnDeadBodiesListChangedEvent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            Destroy(gameObject);
        }

        private void Start()
        {
            if (!IsServer)
                enabled = false;
            
            GameManager.Instance.OnDefeatEvent += ClearAll;
        }

        private void ClearAll()
        {
            _buildings.Clear();
            _alliedSoldiers.Clear();
            _enemies.Clear();
            _alliedObjects.Clear();
            _deadBodies.Clear();
            _players.Clear();
        }

        public void AddBuildingToList(Building newBuilding, bool needToInvoke)
        {
            _buildings.Add(newBuilding);
            _alliedObjects.Add(newBuilding.GetComponent<MonoBehaviour>());

            if (needToInvoke)
                OnBuildingsListChangedEvent?.Invoke(_buildings);
        }

        public void RemoveBuildingFromList(Building newBuilding, bool needToInvoke)
        {
            _buildings.Remove(newBuilding);
            _alliedObjects.Remove(newBuilding.GetComponent<MonoBehaviour>());

            if (needToInvoke)
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

        public void RemovePlayerFromList(PlayerController player, ulong id)
        {
            _players.Remove(id);
            _alliedObjects.Remove(player.GetComponent<MonoBehaviour>());

            OnPlayersDictionaryChangedEvent?.Invoke(_players);
        }

        public void AddAllPlayersInGameToDictionary()
        {
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (PlayerController player in players)
            {
                _players.Add(player.SteamId, player);
                _alliedObjects.Add(player.GetComponent<MonoBehaviour>());

                OnPlayersDictionaryChangedEvent?.Invoke(_players);
            }
        }

        [ClientRpc(RequireOwnership = false)]
        public void RemovePlayerFromListClientRpc(ulong id)
        {
            _alliedObjects.Remove(_players[id]);
            _players.Remove(id);

            OnPlayersDictionaryChangedEvent?.Invoke(_players);
        }

        public void AddDeadBodiesToList(DeadBody deadBodies)
        {
            _deadBodies.Add(deadBodies);

            OnDeadBodiesListChangedEvent?.Invoke(_deadBodies);
        }

        public void RemoveDeadBodiesFromList(DeadBody deadBodies)
        {
            _deadBodies.Remove(deadBodies);

            OnDeadBodiesListChangedEvent?.Invoke(_deadBodies);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.Instance.OnDefeatEvent -= ClearAll;
        }
    }
}
