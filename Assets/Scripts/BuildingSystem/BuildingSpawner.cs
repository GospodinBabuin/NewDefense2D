using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildingSpawner : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    
    private GameObject _buildingToSpawn;
    private bool _previewMode = false;
    private bool _needToInvoke = true;

    private InputReader _input;

    private void Start()
    {
        _input = GetComponentInParent<InputReader>();
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

        if (_input.ConfirmAction)
        {
            if(!CanPlaceBuilding(_buildingToSpawn))
                return;
            
            PlaceBuilding();
            StopPlacement();
        }
    }

    public void StartPlacement(GameObject buildingPrefab, bool needToInvoke)
    {
        if (_buildingToSpawn != null)
            Destroy(_buildingToSpawn);
        
        buildingPrefab.GetComponent<Building>().enabled = false;
        _buildingToSpawn = Instantiate(buildingPrefab, transform.position, quaternion.identity);
        DeactivateBuildingsCollider(_buildingToSpawn);
        _buildingToSpawn.AddComponent<SortingGroup>().sortingLayerName = "BuildingToSpawn";
        _needToInvoke = needToInvoke;
        _previewMode = true;
    }

    private void StopPlacement()
    {
        _needToInvoke = false;
        _previewMode = false;
        _buildingToSpawn = null;
    }

    private void PlaceBuilding()
    {
        _buildingToSpawn.GetComponent<Building>().enabled = true;
        ActivateBuildingsCollider(_buildingToSpawn);
        Destroy(_buildingToSpawn.GetComponent<SortingGroup>());
        ObjectsInWorld.Instance.AddBuildingToList(_buildingToSpawn.GetComponent<Building>(), _needToInvoke);
    }

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
