using System;
using System.Linq;
using Buildings;
using Environment;
using GoldSystem;
using Interfaces;
using MenuSlots;
using SaveLoadSystem;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Entity, IBind<PlayerController.PlayerDataStruct>
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

    public ulong SteamId { get; set; }
    
    public struct PlayerDataStruct : INetworkSerializable, ISaveable
    {
        public ulong id;
        public int maxHealth;
        public int currentHealth;
        public int goldCount;
        public float speed;
        public float speedAnimationMultiplier;
        public int damage;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref maxHealth);
            serializer.SerializeValue(ref currentHealth);
            serializer.SerializeValue(ref goldCount);
            serializer.SerializeValue(ref speed);
            serializer.SerializeValue(ref speedAnimationMultiplier);
            serializer.SerializeValue(ref damage);
        }
    }
    
    public void Bind(PlayerDataStruct gameManagerData)
    {
        Health.SetMaxHealthServerRPC(gameManagerData.maxHealth);
        Health.SetCurrentHealthServerRPC(gameManagerData.currentHealth);
        Debug.Log(GoldBank.Instance);
        GoldBank.Instance.SetGoldCount(this, gameManagerData.goldCount);
        Locomotion.SetSpeedServerRPC(gameManagerData.speed);
        Locomotion.SetSpeedAnimationMultiplierServerRPC(gameManagerData.speedAnimationMultiplier);
        Combat.SetDamageServerRPC(gameManagerData.damage);
        
        Debug.Log($"SaveLoadSystem bind {this.gameObject}, ID: {this.SteamId}");
    }

    [ContextMenu("SavePlayerData")]
    public void SaveData()
    {
        SaveLoad.Instance.SavePlayerServerRpc(SteamId, Health.GetMaxHealth(), Health.GetCurrentHealth(), 
            GoldBank.Instance.Gold, Combat.Damage, Locomotion.Speed, Locomotion.SpeedAnimationMultiplier );
    }

    protected override void Awake()
    {
        base.Awake();
        
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
        ParalaxManager.Instance?.Initialize(GetComponentInChildren<Camera>());
        NetworkTransmission.Instance.PlayerLoadedInGameServerRPC(true, NetworkManager.Singleton.LocalClientId);
        if (GameManager.Instance != null)
            GameManager.Instance.OnDefeatEvent += () => { enabled = false; };
    }

    private void Update()
    {
        Attack();
        Interact();
        UpgradeObject();
        RepairObject();
        OpenOrCloseChat();
        TryToSendMessage();
    }

    private void FixedUpdate()
    {
        Locomotion.RotateAndMoveWithVelocity(_input.Move);
        
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

        Collider2D[] objectsToRepair;
        objectsToRepair = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        if (objectsToRepair == null) return;

        foreach (Collider2D ObjectToRepair in objectsToRepair)
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
        ObjectsInWorld.Instance?.RemovePlayerFromList(this, SteamId);
        if (GameManager.Instance != null)
            GameManager.Instance.OnDefeatEvent -= () => { enabled = false; };
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    public void Die()
    {
        if (NetworkManager.Singleton != null)
        {
            DieClientRPC(NetworkManager.Singleton.LocalClientId);

            if (DeadPlayersHandler.Instance.AreThereAnyDeadPlayers())
            {
                GameManager.Instance.Defeat();
            }
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
            Health.enabled = false;
            Combat.enabled = false;
            Locomotion.enabled = false;
        
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            
            GameManager.Instance.Defeat();
        }
    }

    [ClientRpc(RequireOwnership = false)]
    public void ReviveClientRPC(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;
        
        GetComponent<Collider2D>().enabled = true;
        Health.enabled = true;
        Combat.enabled = true;
        Locomotion.enabled = true;
        
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    [ClientRpc(RequireOwnership = false)]
    private void DieClientRPC(ulong clientId)
    {
        GetComponent<Collider2D>().enabled = false;
        Health.enabled = false;
        Combat.enabled = false;
        Locomotion.enabled = false;
        
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        
        DeadPlayersHandler.Instance.PlayerDead(clientId);
    }
    
    private void LoadPotionsEffects()
    {
        PotionDatabaseSO potionDatabase = Resources.Load<PotionDatabaseSO>("Databases/PotionDatabase");
        if (GameManager.Instance.IncreaseHealthPotionLvl1HasBeenUsed)
        {
            PotionScriptableObject potion = potionDatabase.potionSO.FirstOrDefault(potion => potion.Type == PotionScriptableObject.PotionType.IncreaseHealthLvl1);
            Health.IncreaseMaxHealthServerRPC(potion.IncreaseHealthCount);
            Debug.Log($"{potion} has been used");
        }
        if (GameManager.Instance.IncreaseHealthPotionLvl2HasBeenUsed)
        {
            PotionScriptableObject potion = potionDatabase.potionSO.FirstOrDefault(potion => potion.Type == PotionScriptableObject.PotionType.IncreaseHealthLvl2);
            Health.IncreaseMaxHealthServerRPC(potion.IncreaseHealthCount);
            Debug.Log($"{potion} has been used");
        }
        if (GameManager.Instance.IncreaseHealthPotionLvl3HasBeenUsed)
        {
            PotionScriptableObject potion = potionDatabase.potionSO.FirstOrDefault(potion => potion.Type == PotionScriptableObject.PotionType.IncreaseHealthLvl3);
            Health.IncreaseMaxHealthServerRPC(potion.IncreaseHealthCount);
            Debug.Log($"{potion} has been used");
        }
        if (GameManager.Instance.IncreaseDamagePotionLvl1HasBeenUsed)
        {
            PotionScriptableObject potion = potionDatabase.potionSO.FirstOrDefault(potion => potion.Type == PotionScriptableObject.PotionType.IncreaseDamageLvl1);
            Combat.IncreaseDamageServerRPC(potion.IncreaseDamageCount);
            Debug.Log($"{potion} has been used");
        }
        if (GameManager.Instance.IncreaseDamagePotionLvl2HasBeenUsed)
        {
            PotionScriptableObject potion = potionDatabase.potionSO.FirstOrDefault(potion => potion.Type == PotionScriptableObject.PotionType.IncreaseDamageLvl2);
            Combat.IncreaseDamageServerRPC(potion.IncreaseDamageCount);
            Debug.Log($"{potion} has been used");
        }

        if (GameManager.Instance.IncreaseSpeedPotionLvl1HasBeenUsed)
        {
            PotionScriptableObject potion = potionDatabase.potionSO.FirstOrDefault(potion => potion.Type == PotionScriptableObject.PotionType.IncreaseSpeedLvl1);
            Locomotion.IncreaseSpeedServerRPC(potion.IncreaseSpeedCount);
            Debug.Log($"{potion} has been used");
        }

        if (GameManager.Instance.IncreaseSpeedPotionLvl2HasBeenUsed)
        {
            PotionScriptableObject potion = potionDatabase.potionSO.FirstOrDefault(potion => potion.Type == PotionScriptableObject.PotionType.IncreaseSpeedLvl2);
            Locomotion.IncreaseSpeedServerRPC(potion.IncreaseSpeedCount);
            Debug.Log($"{potion} has been used");
        }
    }
}

[Serializable]
public class PlayerData : ISaveable
{
    public ulong id;
    public int maxHealth = 10;
    public int currentHealth = 10;
    public int goldCount = 200;
    public float speed = 180f;
    public float speedAnimationMultiplier = 1f;
    public int damage = 1;
}