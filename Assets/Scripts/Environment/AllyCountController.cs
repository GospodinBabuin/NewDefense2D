using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Environment
{
    public class AllyCountController : MonoBehaviour
    {
        public static AllyCountController Instance;

        private Text _unitsCount;
        private int _maxAllyCount;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            
            _unitsCount = GetComponentInChildren<Text>();
        }

        private void Start()
        {
            SetAllyCountTextValue(ObjectsInWorld.Instance.AlliedSoldiers.Count, _maxAllyCount);

            ObjectsInWorld.Instance.OnAlliedSoldiersListChangedEvent += AllyCountChange;
        }
        
        public void IncreaseMaxAllyCount(int increaseCount)
        {
            _maxAllyCount += increaseCount;
        
            SetAllyCountTextValue(ObjectsInWorld.Instance.AlliedSoldiers.Count, _maxAllyCount);
        }

        public void ReduceMaxAllyCount(int reduceCount)
        {
            _maxAllyCount -= reduceCount;
        
            SetAllyCountTextValue(ObjectsInWorld.Instance.AlliedSoldiers.Count, _maxAllyCount);
        }

        public bool CanRecruitAlly()
        {
            return (ObjectsInWorld.Instance.AlliedSoldiers.Count < _maxAllyCount);
        }
    
        private void AllyCountChange(List<AlliedSoldier> alliedSoldiers, AlliedSoldier soldier)
        {
            SetAllyCountTextValue(alliedSoldiers.Count, _maxAllyCount);
        }
    
        private void SetAllyCountTextValue(int currentAllyCount, int maxAllyCount)
        {
            _unitsCount.text = $"{currentAllyCount}/{maxAllyCount}";
        }
    }
}
