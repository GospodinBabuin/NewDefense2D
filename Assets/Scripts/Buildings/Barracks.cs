using Interfaces;
using UI;
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

        private void Start()
        {
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

            GameUI.Instance.OpenUnitMenu(BuildingLvl, this);
        }

        public void SpawnUnit(GameObject unitPrefab, int unitCost)
        {
            if (!GoldBank.Instance.IsEnoughGold(unitCost))
            {
                GameUI.Instance.Notifications.ShowNotEnoughMoneyNotification();
                return;
            }

            if (!AllyCountController.Instance.CanRecruitAlly())
            {
                GameUI.Instance.Notifications.ShowImpossibleToRecruitMoreSoldiersNotification();
                return;
            }

            Instantiate(unitPrefab, unitSpawner.position, Quaternion.identity);
            GoldBank.Instance.SpendGold(this, unitCost);
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
            return BuildingLvl < 3;
        }
        
        public int GetUpgradeCost()
        {
            return GetUpgradeToNextLvlCost();
        }
    }
}
