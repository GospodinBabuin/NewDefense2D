using Interfaces;
using UnityEngine;

namespace Buildings
{
    public class Base : Building, IInteractable, IUpgradeable
    {
        public void Interact(GameObject interactingObject)
        {
            throw new System.NotImplementedException();
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
