using UnityEngine;

public class Barracks : Building, IInteractable
{
    [SerializeField] private Transform unitSpawner;

    private Transform _interactingObjectTransform;
    private CircleCollider2D _circleCollider;

    private void Start()
    {
        _circleCollider = GetComponentInChildren<CircleCollider2D>();
    }
    private void FixedUpdate()
    {
        CheckDistance();
    }

    public void Interact(GameObject interactingObject)
    {
        _interactingObjectTransform = interactingObject.transform;

        GameUI.Instance.UnitMenu.SetActive(true);
        GameUI.Instance.UnitMenu.GetComponent<UnitMenu>().ShowMenu(BuildingLvl, this);
    }

    public void SpawnUnit(GameObject unitPrefab)
    {
        Instantiate(unitPrefab, unitSpawner.position, Quaternion.identity);
    }

    private void CheckDistance()
    {
        if (_interactingObjectTransform == null) return;

        //if (Vector2.Distance(gameObject.transform.position, _interactingObjectTransform.position) > _circleCollider.radius)

        float distanceOnXAxis = Mathf.Abs(transform.position.x - _interactingObjectTransform.position.x);

        if (distanceOnXAxis > _circleCollider.radius)
            CloseUnitMenu();
    }

    private void CloseUnitMenu()
    {
        GameUI.Instance.UnitMenu.SetActive(false);
        _interactingObjectTransform = null;
    }
}
