using Buildings;
using Interfaces;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Entity
{
    [SerializeField] private float interactionRadius = 1.3f;
    
    [SerializeField] private GameObject interactionNotice;
    [SerializeField] private GameObject upgradeNotice;
    [SerializeField] private Text upgradeCost;
    [SerializeField] private GameObject repairNotice;
    [SerializeField] private Text repairCost;
    [SerializeField] private GameObject notices;
    
    private InputReader _input;

    private bool _canInteract;
    private bool _canUpgrade;
    private bool _canRepair;
    
    protected override void Start()
    {
        base.Start();

        ObjectsInWorld.Instance.AddPlayerToList(this);

        _input = GetComponent<InputReader>();
    }

    private void Update()
    {
        Rotate();
        Move();
        Attack();
        Interact();
        UpgradeObject();
        RepairObject();
    }

    private void FixedUpdate()
    {
        CheckInteractionObjects();
        CheckUpgradeableObjects();
        CheckBuildingsToRepair();
    }
    
    private void Move()
    {
        if (_input.Move == 0f)
        {
            Locomotion.SetMoveAnimation(false);
            return;
        }

        transform.position += new Vector3(_input.Move, 0, 0) * (Locomotion.Speed * Time.deltaTime);

        Locomotion.SetMoveAnimation(true);
    }

    private void Rotate()
    {
        if (_input.Move == 0) return;
        
        transform.rotation = _input.Move < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        notices.transform.rotation = Quaternion.identity;
    }

    private void Attack()
    {
        if (!_input.Attack) return;
        
        Combat.Attack();
    }

    private void CheckInteractionObjects()
    {
        Collider2D[] interactionObjects;
        interactionObjects = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        if (interactionObjects == null) return;
        
        foreach (Collider2D interactionObject in interactionObjects)
        {
            if (interactionObject.GetComponent<IInteractable>() != null )
            {
                interactionNotice.SetActive(true);
                _canInteract = true;
                return;
            }
        }
        
        interactionNotice.SetActive(false);
        _canInteract = false;
    }

    private void CheckUpgradeableObjects()
    {
        Collider2D[] upgradeableObjects;
        upgradeableObjects = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        if (upgradeableObjects == null) return;
        
        foreach (Collider2D upgradeableObject in upgradeableObjects)
        {
            if (!upgradeableObject.TryGetComponent<IUpgradeable>(out IUpgradeable upgradeable)) continue;
            
            if (!upgradeable.CanUpgrade()) continue;
            
            upgradeNotice.SetActive(true);
            upgradeCost.text = upgradeable.GetUpgradeCost().ToString();
            _canUpgrade = true;
            return;
        }
        
        upgradeNotice.SetActive(false);
        _canUpgrade = false;
    }
    
    private void CheckBuildingsToRepair()
    {
        Collider2D[] ObjectsToRepair;
        ObjectsToRepair = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        if (ObjectsToRepair == null) return;
        
        foreach (Collider2D objectToRepair in ObjectsToRepair)
        {
            if (!objectToRepair.TryGetComponent<Building>(out Building building)) continue;
            
            if (building.Health.IsMaxHealth()) continue;
            
            repairNotice.SetActive(true);
            repairCost.text = building.CostToRepairBuilding().ToString();
            _canRepair = true;
            return;
        }
        
        repairNotice.SetActive(false);
        _canRepair = false;
    }

    private void Interact()
    {
        if (!_input.Interact || !_canInteract) return;
        
        if (GameUI.Instance.IsMenuOpen())
            return;

        Collider2D[] interactionObjects;
        interactionObjects = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        if (interactionObjects == null) return;
        
        foreach (Collider2D interactionObject in interactionObjects)
        {
            if (!interactionObject.TryGetComponent(out IInteractable interactable)) continue;
            interactable.Interact(this.gameObject);
            return;
        }
    }

    private void UpgradeObject()
    {
        if (!_input.UpgradeObject || !_canUpgrade) return;
        
        if (GameUI.Instance.IsMenuOpen())
            return;

        Collider2D[] upgradeableObjects;
        upgradeableObjects = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        if (upgradeableObjects == null) return;
        
        foreach (Collider2D upgradeableObject in upgradeableObjects)
        {
            if (!upgradeableObject.TryGetComponent(out IUpgradeable upgradeable)) continue;
            upgradeable.Upgrade();
            return;
        }
    }
    
    private void RepairObject()
    {
        if (!_input.RepairObject || !_canRepair) return;
        
        if (GameUI.Instance.IsMenuOpen())
            return;

        Collider2D[] ObjectsToRepair;
        ObjectsToRepair = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        if (ObjectsToRepair == null) return;
        
        foreach (Collider2D ObjectToRepair in ObjectsToRepair)
        {
            if (!ObjectToRepair.TryGetComponent(out Building building)) continue;
            building.RepairBuilding();
            return;
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        ObjectsInWorld.Instance.RemovePlayerFromList(this);
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
