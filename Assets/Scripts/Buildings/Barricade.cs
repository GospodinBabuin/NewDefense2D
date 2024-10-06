using Interfaces;

namespace Buildings
{
    public class Barricade : Building, IUpgradeable
    {
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
