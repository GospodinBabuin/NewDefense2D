namespace Interfaces
{
    public interface IUpgradeable
    {
        public void Upgrade();
        public bool CanUpgrade();
        public int GetUpgradeCost();
    }
}
