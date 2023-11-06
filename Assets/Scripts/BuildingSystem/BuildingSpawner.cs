using Unity.Mathematics;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    
    private GameObject _buildingToSpawn;
    private bool _previewMode = false;

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

    public void StartPlacement(GameObject buildingPrefab)
    {
        if (_buildingToSpawn != null)
            Destroy(_buildingToSpawn);
        
        buildingPrefab.GetComponent<Building>().enabled = false;
        _buildingToSpawn = Instantiate(buildingPrefab, transform.position, quaternion.identity);
        DeactivateBuildingsCollider(_buildingToSpawn);
        _previewMode = true;
    }

    private void StopPlacement()
    {
        _previewMode = false;
        _buildingToSpawn = null;
    }

    private void PlaceBuilding()
    {
        _buildingToSpawn.GetComponent<Building>().enabled = true;
        ActivateBuildingsCollider(_buildingToSpawn);
        ObjectsInWorld.Instance.AddBuildingToList(_buildingToSpawn.GetComponent<Building>());
    }

    private void SetBuildingPosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, float.MaxValue, groundLayerMask);
        if (hit) _buildingToSpawn.transform.position = hit.point;
    }

    private bool CanPlaceBuilding(GameObject building)
    {
        return building.GetComponentInChildren<BuildingBoundaries>().IsBoundariesClear();
    }

    private void DeactivateBuildingsCollider(GameObject building)
    {
        building.GetComponent<Collider2D>().enabled = false;
    }
    
    private void ActivateBuildingsCollider(GameObject building)
    {
        building.GetComponent<Collider2D>().enabled = true;
    }
}
