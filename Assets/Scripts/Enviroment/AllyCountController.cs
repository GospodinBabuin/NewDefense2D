using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyCountController : MonoBehaviour
{
    public static AllyCountController Instance;

    private Text _unitsCount;


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
    }

    private void Start()
    {
        _unitsCount = GetComponentInChildren<Text>();
        
        SetAllyCountTextValue(ObjectsInWorld.Instance.AlliedSoldiers.Count, MaxAllyCount);

        ObjectsInWorld.Instance.OnAlliedSoldiersListChangedEvent += AllyCountChange;
    }

    public int MaxAllyCount { get; private set; }

    public void IncreaseMaxAllyCount(int increaseCount)
    {
        MaxAllyCount += increaseCount;
        
        SetAllyCountTextValue(ObjectsInWorld.Instance.AlliedSoldiers.Count, MaxAllyCount);
    }

    public void ReduceMaxAllyCount(int reduceCount)
    {
        MaxAllyCount -= reduceCount;
        
        SetAllyCountTextValue(ObjectsInWorld.Instance.AlliedSoldiers.Count, MaxAllyCount);
    }

    public bool CanRecruitAlly()
    {
        return (ObjectsInWorld.Instance.AlliedSoldiers.Count < MaxAllyCount);
    }
    
    private void AllyCountChange(List<AlliedSoldier> alliedSoldiers, AlliedSoldier soldier)
    {
        SetAllyCountTextValue(alliedSoldiers.Count, MaxAllyCount);
    }
    
    private void SetAllyCountTextValue(int currentAllyCount, int maxAllyCount)
    {
        _unitsCount.text = $"{currentAllyCount}/{maxAllyCount}";
    }
}
