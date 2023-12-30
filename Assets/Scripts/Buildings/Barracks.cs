using UnityEngine;

public class Barracks : Building, IInteractable
{
    [SerializeField] private Transform unitSpawner;

    private Transform _interactingObjectTransform;
    private CircleCollider2D _circleCollider;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private bool _enemyPresence = false;

    private void Start()
    {
        _circleCollider = GetComponentInChildren<CircleCollider2D>();
    }
    private void FixedUpdate()
    {
        CheckDistance();
        CheckEnemyPresence();
    }

    public void Interact(GameObject interactingObject)
    {
        Collider2D[] colliders2d = Physics2D.OverlapCircleAll(transform.position, _circleCollider.radius, enemyLayerMask);
        if (colliders2d.Length != 0)
            return;
            
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

    private void CheckEnemyPresence()
    {
        if (_interactingObjectTransform == null) return;

        Collider2D[] colliders2d = Physics2D.OverlapCircleAll(transform.position, _circleCollider.radius, enemyLayerMask);
        if (colliders2d.Length == 0)
        {
            _enemyPresence = false;
        }
        else
        {
            CloseUnitMenu();
            _enemyPresence = true;
        }
    }

    private void CloseUnitMenu()
    {
        GameUI.Instance.UnitMenu.SetActive(false);
        _interactingObjectTransform = null;
    }
}
