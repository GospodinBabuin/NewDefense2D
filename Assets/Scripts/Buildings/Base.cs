using Interfaces;
using UnityEngine;

namespace Buildings
{
    public class Base : Building, IInteractable, IUpgradeable
    {
        public void Interact(GameObject interactingObject)
        {
            Health.TestDestroy();
        }

        public void Upgrade()
        {
            UpgradeBuildingServerRPC();
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
