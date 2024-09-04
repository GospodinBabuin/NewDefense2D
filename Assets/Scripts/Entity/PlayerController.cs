using System;
using Buildings;
using Cinemachine;
using Environment;
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

    private InputReader _input;

    private bool _canInteract;
    private bool _canUpgrade;
    private bool _canRepair;

    protected override void Awake()
    {
        base.Awake();
        
        ObjectsInWorld.Instance?.AddPlayerToList(this);

        _input = GetComponent<InputReader>();
    }

    private void Start()
    {
        if (!IsOwner)
        {
            _input.enabled = false;
            enabled = false;
            return;
        }
        _input.Enable();
        ParalaxManager.Instance.Initialize(GetComponentInChildren<Camera>());
    }

    private void Update()
    {
        Locomotion.Rotate(_input.Move);
        Locomotion.Move(_input.Move);
        Attack();
        Interact();
        UpgradeObject();
        RepairObject();
        OpenOrCloseChat();
        TryToSendMessage();
    }

    private void FixedUpdate()
    {
        CheckInteractionObjects();
        CheckUpgradeableObjects();
        CheckBuildingsToRepair();
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
            if (interactionObject.GetComponent<IInteractable>() != null)
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

    private void OpenOrCloseChat()
    {
        if (_input.Chat)
        {
            Chat.Instance.OpenOrCloseChat(_input.InputActions);
        }
    }

    private void TryToSendMessage()
    {
        if (_input.Confirm)
        {
            Chat.Instance.TryToSendMessage();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ObjectsInWorld.Instance?.RemovePlayerFromList(this);
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
