using UnityEngine;

namespace Buildings
{
    public class BuildingGoldBringer : Building, IInteractable
    {
        [SerializeField] private int goldPerIteration = 1;
        [SerializeField] private float iterationTimerInSeconds;
        [SerializeField] private int maxGold;

        private Chest _chest;
    
        private int _collectedGold;
        private float _timerDelta;

        private void Awake()
        {
            _timerDelta = iterationTimerInSeconds;
            _chest = GetComponentInChildren<Chest>();
        }

        private void FixedUpdate()
        {
            FarmGold();
        }

        private void FarmGold()
        {
            if(!CanFarmGold()) return;
        
            if (_timerDelta > 0)
            {
                _timerDelta -= Time.deltaTime;
            }
            else
            {
                _collectedGold += goldPerIteration;
                _timerDelta = iterationTimerInSeconds;
            }
        }

        private bool CanFarmGold()
        {
            return (_collectedGold < maxGold);
        }

        public void Interact(GameObject interactingObject)
        {
            _chest.CollectGold(_collectedGold);
            _collectedGold = 0;
        }
    }
}
