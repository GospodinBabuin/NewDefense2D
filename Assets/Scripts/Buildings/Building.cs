using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Building : MonoBehaviour
    {
        private Health _health;
        public byte BuildingLvl { get; private set; }

        private void Awake()
        {
            _health = GetComponent<Health>();
            BuildingLvl = 1;
        }

        public virtual void UpgradeBuilding()
        {
            BuildingLvl++;
        }

        protected virtual void OnDestroy()
        {
            if (ObjectsInWorld.Instance.Buildings.Contains(this))
                ObjectsInWorld.Instance.RemoveBuildingFromList(this, true);
        }
    }
}
