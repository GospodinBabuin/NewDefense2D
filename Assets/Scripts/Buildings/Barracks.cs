using UI;
using UnityEngine;

namespace Buildings
{
    public class Barracks : Building, IInteractable
    {
        [SerializeField] private Transform unitSpawner;

        private Transform _interactingObjectTransform;
        private CircleCollider2D _circleCollider;
        [SerializeField] private LayerMask enemyLayerMask;

        private void Start()
        {
            _circleCollider = GetComponentInChildren<CircleCollider2D>();

            AllyCountController.Instance.IncreaseMaxAllyCount(6);
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

            GameUI.Instance.OpenUnitMenu();
            GameUI.Instance.UnitMenu.GetComponent<UnitMenu>().ShowMenu(BuildingLvl, this);
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

        public override void UpgradeBuilding()
        {
            base.UpgradeBuilding();
            
            
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
            
            AllyCountController.Instance.ReduceMaxAllyCount(6);
        }
    }
}
