using Interfaces;
using UI;
using UnityEngine;

namespace Buildings
{
    public class Altar : Building, IInteractable, IUpgradeable
    {
        private Transform _interactingObjectTransform;
        private CircleCollider2D _circleCollider;
        [SerializeField] private LayerMask enemyLayerMask;

        protected override void Start()
        {
            base.Start();
            
            _circleCollider = GetComponentInChildren<CircleCollider2D>();
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
            
            GameUI.Instance.OpenAltarMenu(BuildingLvl.Value, this);
        }
        
        private void CheckDistance()
        {
            if (_interactingObjectTransform == null) return;

            float distanceOnXAxis = Mathf.Abs(transform.position.x - _interactingObjectTransform.position.x);

            if (distanceOnXAxis > _circleCollider.radius)
                CloseAltarMenu();
        }

        private void CheckEnemyPresence()
        {
            if (_interactingObjectTransform == null) return;

            Collider2D[] colliders2d = Physics2D.OverlapCircleAll(transform.position, _circleCollider.radius, enemyLayerMask);
            if (colliders2d.Length != 0)
            {
                CloseAltarMenu();
            }
        }
        
        private void CloseAltarMenu()
        {
            GameUI.Instance.CloseAltarMenu();
            _interactingObjectTransform = null;
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
