using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Buildings
{
    public class BuildingGoldBringer : Building, IInteractable, IUpgradeable
    {
        [SerializeField] private int goldPerIteration = 1;
        [SerializeField] private float iterationTimerInSeconds;
        [SerializeField] private int maxGold;

        private Chest _chest;
    
        private int _collectedGold;
        private float _timerDelta;

        protected override void Start()
        {
            base.Start();
            
            _timerDelta = iterationTimerInSeconds;
            _chest = GetComponentInChildren<Chest>();
        }

        private void FixedUpdate()
        {
            if (!IsHost) return;
            
            FarmGold();
        }

        public override void Bind(BuildingDataStruct gameManagerData)
        {
            base.Bind(gameManagerData);
            
            for (int i = 1; i < BuildingLvl.Value; i++)
            {
                goldPerIteration++;
                maxGold *= 2;
            }
        }

        private void FarmGold()
        {
            if(!CanFarmGold()) return;
        
            if (_timerDelta > 0)
            {
                _timerDelta -= Time.deltaTime;
            }
            else
            {
                _collectedGold += goldPerIteration;
                _timerDelta = iterationTimerInSeconds;
            }
        }

        private bool CanFarmGold()
        {
            return (_collectedGold < maxGold);
        }

        public void Interact(GameObject interactingObject)
        {
            InteractClientRPC();
        }

        [ClientRpc]
        private void InteractClientRPC()
        {
            _chest.CollectGold(_collectedGold);
            _collectedGold = 0;
        }
        
        [ContextMenu("UpgradeBuildingGoldBringer")]
        protected override void UpgradeBuilding()
        {
            base.UpgradeBuilding();
            
            goldPerIteration++;
            maxGold *= 2;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _chest?.DestroyChest();
        }

        public void Upgrade()
        {
            UpgradeBuilding();
        }
        
        public bool CanUpgrade()
        {
            return BuildingLvl.Value < 3;
        }
        
        public int GetUpgradeCost()
        {
            return GetUpgradeToNextLvlCost();
        }
    }
}
