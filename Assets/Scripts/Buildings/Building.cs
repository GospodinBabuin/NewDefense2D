using System;
using Environment;
using HealthSystem;
using SaveLoadSystem;
using UI;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(BuildingHealth))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Animator))]
    public class Building : MonoBehaviour, IBind<BuildingData>
    {
        public BuildingHealth Health { get; private set; }
        protected Animator Animator { get; private set; }
        public byte BuildingLvl { get; private set; }
        public int Id { get; private set; }

        [SerializeField] private bool needToCheckBorders;

        [SerializeField] private byte upgradeToLvl2Cost;
        [SerializeField] private byte upgradeToLvl3Cost;

        [SerializeField] private byte repairCostPerDamage = 5;
        
        private int _animIDUpgradeToLvl2;
        private int _animIDUpgradeToLvl3;

        private BuildingData _buildingData;
        
        private void Awake()
        {
            Health = GetComponent<BuildingHealth>();
            Animator = GetComponent<Animator>();
            BuildingLvl = 1;
            
            SetAnimIDs();
        }

        protected virtual void Start()
        {
            ObjectsInWorld.Instance.AddBuildingToList(this, needToCheckBorders);
        }

        [ContextMenu("Upgrade")]
        public virtual void UpgradeBuilding()
        {
            switch (BuildingLvl)
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
            
            BuildingLvl++;
            Health.IncreaseMaxHealth(Health.MaxHealth/2);
            Health.Heal(Health.MaxHealth/2);
        }

        protected int GetUpgradeToNextLvlCost()
        {
            switch (BuildingLvl)
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
            return Health.HealthToMax() * repairCostPerDamage * BuildingLvl;
        }

        public void SetBuildingsId(int id) => Id = id;

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

        public void Bind(BuildingData data)
        {
            _buildingData = data;
            data.id = Id;
            BuildingLvl = data.level;
            switch (BuildingLvl)
            {
                case 2:
                    Animator.SetTrigger(_animIDUpgradeToLvl2);
                    break;
                case 3:
                    Animator.SetTrigger(_animIDUpgradeToLvl3); 
                    break;
            }

            for (int i = 1; i < BuildingLvl; i++)
            {
                Health.IncreaseMaxHealth(Health.MaxHealth/2);
            }

            Health.SetCurrentHealth(data.currentHealth);
        }

        public void SaveData()
        {
            _buildingData.position = transform.position;
            _buildingData.currentHealth = Health.CurrentHealth;
            _buildingData.level = BuildingLvl;
        }
    }

    [Serializable]
    public class BuildingData : ISaveable
    {
        public int id;
        public Vector2 position;
        public int currentHealth;
        public byte level;
    }
}
