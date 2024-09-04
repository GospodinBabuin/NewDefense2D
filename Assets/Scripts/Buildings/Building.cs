using System;
using Environment;
using HealthSystem;
using UI;
using Unity.Netcode;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(BuildingHealth))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Animator))]
    public class Building : MonoBehaviour
    {
        private BuildingHealth _health;
        public BuildingHealth Health { get => _health; private set => _health = value; }
        
        public byte BuildingLvl { get; private set; }

        [SerializeField] private byte upgradeToLvl2Cost;
        [SerializeField] private byte upgradeToLvl3Cost;

        [SerializeField] private byte repairCostPerDamage = 5;

        private Animator _animator;
        protected Animator Animator { get => _animator; private set => _animator = value; }
        
        private int _animIDUpgradeToLvl2;
        private int _animIDUpgradeToLvl3;

        
        private void Awake()
        {
            _health = GetComponent<BuildingHealth>();
            _animator = GetComponent<Animator>();
            BuildingLvl = 1;
            
            SetAnimIDs();
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
                    _animator.SetTrigger(_animIDUpgradeToLvl2);
                    GoldBank.Instance.SpendGold(this, upgradeToLvl2Cost);
                    break;
                case 2:
                    if (!GoldBank.Instance.IsEnoughGold(upgradeToLvl3Cost))
                    {
                        GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                        return;
                    }
                    _animator.SetTrigger(_animIDUpgradeToLvl3); 
                    GoldBank.Instance.SpendGold(this, upgradeToLvl3Cost);
                    break;
            }
            
            BuildingLvl++;
            _health.IncreaseMaxHealth(_health.MaxHealth/2);
            _health.Heal(_health.MaxHealth/2);
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
            _health.HealToMaxHealth();
        }

        public int CostToRepairBuilding()
        { 
            return _health.HealthToMax() * repairCostPerDamage * BuildingLvl;
        }

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
}
