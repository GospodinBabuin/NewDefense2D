using UnityEngine;

public class Barracks : Building, IInteractable
{
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform unitSpawner;

    public void Interact(GameObject interactingObject)
    {
        GameUI.Instance.UnitMenu.SetActive(true);
        GameUI.Instance.UnitMenu.GetComponent<UnitMenu>().ShowMenu(BuildingLvl, this);
    }

    public void SpawnUnit(GameObject unitPrefab)
    {
        RaycastHit2D hit = Physics2D.Raycast(unitSpawner.position, -transform.up, float.MaxValue, groundLayerMask);
        if (hit) unitPrefab.transform.position = hit.point;
        Instantiate(unitPrefab);
    }
}
