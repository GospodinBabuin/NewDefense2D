using System;
using Buildings;
using Environment;
using UI;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace BuildingSystem
{
    public class BuildingSpawner : NetworkBehaviour
    {
        public static BuildingSpawner Instance;
        
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private BuildingDatabaseSO database;
    
        private GameObject _buildingToSpawn;
        private int _selectedObjectIndex = -1;
        
        private int _buildingCost;
        private bool _previewMode = false;

        private InputReader _input;

        private void Awake()
        {
            _input = GetComponentInParent<InputReader>();
        }

        private void Start()
        {
            if (IsOwner)
            {
                if (Instance == null)
                {
                    Instance = this;
                    StopPlacement();
                }
            }
            else
            {
                enabled = false;
            }
        }

        private void Update()
        {
            if (!_previewMode || _buildingToSpawn == null) return;

            SetBuildingPosition();

            if (_input.CancelAction)
            {
                Destroy(_buildingToSpawn);
                StopPlacement();
                return;
            }

            if (!_input.ConfirmAction) return;
            
            if (!CanPlaceBuilding(_buildingToSpawn))
            {
                GameUI.Instance.Notifications.ShowImpossibleToPlaceBuildingNotification();
                return;
            }
            
            if (!GoldBank.Instance.IsEnoughGold(_buildingCost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }
            
            GoldBank.Instance.SpendGold(this, _buildingCost);
            PlaceBuilding();
            StopPlacement();
        }

        public void StartPlacement(int buildingId, int buildingCost)
        {
            if (_buildingToSpawn != null)
                Destroy(_buildingToSpawn);

            _selectedObjectIndex = database.buildingsData.FindIndex(data => data.ID == buildingId);
            if (_selectedObjectIndex < 0)
                Debug.Log($"No id found {buildingId}");
            
            GameObject buildingPrefab = database.buildingsData[_selectedObjectIndex].Prefab;
            buildingPrefab.GetComponent<Building>().enabled = false;
           
            _buildingToSpawn = Instantiate(buildingPrefab, transform.position, quaternion.identity);
            DeactivateBuildingsCollider(_buildingToSpawn);
            _buildingToSpawn.AddComponent<SortingGroup>().sortingLayerName = "BuildingToSpawn";
            _buildingCost = buildingCost;
            _previewMode = true;
        }

        private void StopPlacement()
        {
            _selectedObjectIndex = -1;
            _previewMode = false;
            _buildingToSpawn = null;
            _buildingCost = int.MaxValue;
        }
        
        private void PlaceBuilding()
        {
            /*_buildingToSpawn.GetComponent<Building>().enabled = true;
            ActivateBuildingsCollider(_buildingToSpawn);
            Destroy(_buildingToSpawn.GetComponent<SortingGroup>());
            _buildingToSpawn.GetComponent<NetworkObject>().Spawn(true);*/

            Vector2 buildingPosition = _buildingToSpawn.transform.position;
            Destroy(_buildingToSpawn);
            SpawnBuildingServerRPC(_selectedObjectIndex, buildingPosition);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnBuildingServerRPC(int buildingId, Vector2 position)
        {
            GameObject buildingPrefab = Instantiate(database.buildingsData[buildingId].Prefab, position, quaternion.identity);
            buildingPrefab.GetComponent<Building>().enabled = true;
            buildingPrefab.GetComponent<Building>().SetBuildingsId(buildingId);
            buildingPrefab.GetComponent<NetworkObject>().Spawn(true);
        }
        
        /*[ServerRpc(RequireOwnership = false)]
        public void SpawnBuildingServerRPC(BuildingData data)
        {
            GameObject buildingPrefab = Instantiate(database.buildingsData[data.id].Prefab, data.position, quaternion.identity);
            buildingPrefab.GetComponent<Building>().enabled = true;
            buildingPrefab.GetComponent<Building>().SetBuildingsId(data.id);
            buildingPrefab.GetComponent<Building>().Bind(data);
            buildingPrefab.GetComponent<NetworkObject>().Spawn(true);
        } */

        private void SetBuildingPosition()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, float.MaxValue, groundLayerMask);
            if (hit) _buildingToSpawn.transform.position = hit.point;
        }

        private static bool CanPlaceBuilding(GameObject building)
        {
            return building.GetComponentInChildren<BuildingBoundaries>().IsBoundariesClear();
        }

        private static void DeactivateBuildingsCollider(GameObject building)
        {
            building.GetComponent<Collider2D>().enabled = false;
        }
    
        private static void ActivateBuildingsCollider(GameObject building)
        {
            building.GetComponent<Collider2D>().enabled = true;
        }
    }
}
