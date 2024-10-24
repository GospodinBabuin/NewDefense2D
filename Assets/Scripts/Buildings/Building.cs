using System;
using Environment;
using HealthSystem;
using SaveLoadSystem;
using UI;
using Unity.Netcode;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(BuildingHealth))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Animator))]
    public class Building : NetworkBehaviour, IBind<Building.BuildingDataStruct>
    {
        public BuildingHealth Health { get; private set; }
        protected Animator Animator { get; private set; }
        public NetworkVariable<byte> BuildingLvl = new NetworkVariable<byte>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        [SerializeField] private int id = -1;

        [SerializeField] private bool needToCheckBorders;

        [SerializeField] private byte upgradeToLvl2Cost;
        [SerializeField] private byte upgradeToLvl3Cost;

        [SerializeField] private byte repairCostPerDamage = 5;
        
        private int _animIDUpgradeToLvl2;
        private int _animIDUpgradeToLvl3;

        private BuildingData _buildingData = new BuildingData();

        public struct BuildingDataStruct : INetworkSerializable, ISaveable
        {
            public int id;
            public byte level;
            public int currentHealth;
            public Vector2 position;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref id);
                serializer.SerializeValue(ref level);
                serializer.SerializeValue(ref currentHealth);
                serializer.SerializeValue(ref position);
            }
        }

        public void Bind(BuildingDataStruct buildingData)
        {
            BuildingLvl.Value = buildingData.level;
            switch (BuildingLvl.Value)
            {
                case 2:
                    Animator.SetTrigger(_animIDUpgradeToLvl2);
                    break;
                case 3:
                    Animator.SetTrigger(_animIDUpgradeToLvl3);
                    break;
            }

            for (int i = 1; i < BuildingLvl.Value; i++)
            {
                Health.IncreaseMaxHealth(Health.GetMaxHealth() / 2);
            }

            Health.SetCurrentHealth(buildingData.currentHealth);
        }

        public void SaveData()
        {
            _buildingData.id = id;
            _buildingData.position = transform.position;
            _buildingData.currentHealth = Health.GetCurrentHealth();
            _buildingData.level = BuildingLvl.Value;
        }

        public BuildingData GetBuildingData() => _buildingData;

        private void Awake()
        {
            Health = GetComponent<BuildingHealth>();
            Animator = GetComponent<Animator>();
            
            SetAnimIDs();
        }

        protected virtual void Start()
        {
            ObjectsInWorld.Instance.AddBuildingToList(this, needToCheckBorders);
        }

        [ContextMenu("Upgrade")]
        public virtual void UpgradeBuilding()
        {
            switch (BuildingLvl.Value)
            {
                case 1:
                    if (!GoldBank.Instance.IsEnoughGold(upgradeToLvl2Cost))
                    {
                        GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                        return;
                    }
                    Animator.SetTrigger(_animIDUpgradeToLvl2);
                    GoldBank.Instance.SpendGold(this, upgradeToLvl2Cost);
                    break;
                case 2:
                    if (!GoldBank.Instance.IsEnoughGold(upgradeToLvl3Cost))
                    {
                        GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                        return;
                    }
                    Animator.SetTrigger(_animIDUpgradeToLvl3); 
                    GoldBank.Instance.SpendGold(this, upgradeToLvl3Cost);
                    break;
            }
            
            BuildingLvl.Value++;
            Health.IncreaseMaxHealth(Health.GetMaxHealth() /2);
            Health.Heal(Health.GetMaxHealth() /2);
        }

        protected int GetUpgradeToNextLvlCost()
        {
            switch (BuildingLvl.Value)
            {
                case 1:
                    return upgradeToLvl2Cost;
                case 2:
                    return upgradeToLvl3Cost;
            }

            throw new InvalidOperationException();
        }

        public void RepairBuilding()
        {
            if (!GoldBank.Instance.IsEnoughGold(CostToRepairBuilding()))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            GoldBank.Instance.SpendGold(this, CostToRepairBuilding());
            Health.HealToMaxHealth();
        }

        public int CostToRepairBuilding()
        { 
            return Health.HealthToMax() * repairCostPerDamage * BuildingLvl.Value;
        }

        public void SetBuildingsId(int id) => this.id = id;
        public int GetBuildingsId() => id;

        protected virtual void OnDestroy()
        {
            if (ObjectsInWorld.Instance.Buildings.Contains(this))
                ObjectsInWorld.Instance.RemoveBuildingFromList(this, true);
        }
        
        private void SetAnimIDs()
        {
            _animIDUpgradeToLvl2 = Animator.StringToHash("UpToLvl2");
            _animIDUpgradeToLvl3 = Animator.StringToHash("UpToLvl3");
        }
    }

    [Serializable]
    public class BuildingData : ISaveable
    {
        public int id;
        public byte level;
        public int currentHealth;
        public Vector2 position;
    }
}
