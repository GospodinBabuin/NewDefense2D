using Environment;
using Interfaces;
using UI;
using Unity.Netcode;
using UnityEngine;

namespace Buildings
{
    public class Barracks : Building, IInteractable, IUpgradeable
    {
        [SerializeField] private Transform unitSpawner;

        private Transform _interactingObjectTransform;
        private CircleCollider2D _circleCollider;
        [SerializeField] private LayerMask enemyLayerMask;

        private int _currentMaxAllyCountFromBuilding = 0;
        [SerializeField] private int startAllyCoutSize = 6;
        [SerializeField] private int magnificationSize = 2;

        struct Vector3Struct : INetworkSerializable
        {
            public Vector3 Position;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Position);
            }
        }

        protected override void Start()
        {
            base.Start();
            
            _circleCollider = GetComponentInChildren<CircleCollider2D>();
            AllyCountController.Instance.IncreaseMaxAllyCount(startAllyCoutSize);
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

            GameUI.Instance.OpenUnitMenu(BuildingLvl.Value, this);
        }

        public void SpawnUnit(int unitId, int unitCost)
        {
            if (CanSpawnUnit(unitCost) == false) return;

            GoldBank.Instance.SpendGold(this, unitCost);

            EntitySpawner.Instance.SpawnUnit(unitId, unitSpawner.position);
        }

        private bool CanSpawnUnit(int unitCost)
        {
            if (!GoldBank.Instance.IsEnoughGold(unitCost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return false;
            }

            if (!AllyCountController.Instance.CanRecruitAlly())
            {
                GameUI.Instance.Notifications.ShowImpossibleToRecruitMoreSoldiersNotification();
                return false;
            }

            return true;
        }
        
        [ContextMenu("UpgradeBarracks")]
        public override void UpgradeBuilding()
        {
            base.UpgradeBuilding();
            _currentMaxAllyCountFromBuilding += magnificationSize;
            AllyCountController.Instance.IncreaseMaxAllyCount(magnificationSize);
        }

        private void CheckDistance()
        {
            if (_interactingObjectTransform == null) return;

            float distanceOnXAxis = Mathf.Abs(transform.position.x - _interactingObjectTransform.position.x);

            if (distanceOnXAxis > _circleCollider.radius)
                CloseUnitMenu();
        }

        private void CheckEnemyPresence()
        {
            if (_interactingObjectTransform == null) return;

            Collider2D[] colliders2d = Physics2D.OverlapCircleAll(transform.position, _circleCollider.radius, enemyLayerMask);
            if (colliders2d.Length != 0)
            {
                CloseUnitMenu();
            }
        }

        private void CloseUnitMenu()
        {
            GameUI.Instance.CloseUnitMenu();
            _interactingObjectTransform = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            AllyCountController.Instance.ReduceMaxAllyCount(_currentMaxAllyCountFromBuilding);
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