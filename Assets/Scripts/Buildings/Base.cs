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
            throw new System.NotImplementedException();
        }

        public bool CanUpgrade()
        {
            throw new System.NotImplementedException();
        }

        public int GetUpgradeCost()
        {
            throw new System.NotImplementedException();
        }
    }
}
