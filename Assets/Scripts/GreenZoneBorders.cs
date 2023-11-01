using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GreenZoneBorders : MonoBehaviour
{
    public static GreenZoneBorders Instance { get; private set; }
    
    public Transform LeftBorder
    {
        get => leftBorder;
        private set => leftBorder = value;
    }
    public Transform RightBorder
    {
        get => rightBorder;
        private set => rightBorder = value;
    }
    
    [SerializeField] private Transform leftBorder;
    [SerializeField] private Transform rightBorder;

    [SerializeField] private Transform defaultLeftBorder;
    [SerializeField] private Transform defaultRightBorder;

    private readonly List<Building> _buildings = new List<Building>();
    private readonly List<AlliedSoldier> _alliedSoldiersOnBorders = new List<AlliedSoldier>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            return;
        }
        
        Destroy(gameObject);
    }

    private void Start()
    {
        SetBorders();
    }

    private void SetBorders()
    {
        LeftBorder.position = defaultLeftBorder.position;
        RightBorder.position = defaultRightBorder.position;

        if (_buildings.Count == 0) return;
        
        foreach (Building building in _buildings)
        {
            if (building.transform.position.x < LeftBorder.transform.position.x)
            {
                LeftBorder.transform.position = building.transform.position + new Vector3(2, -3);
                Debug.Log($"New left border position: {LeftBorder.transform.position.x}");
            }

            if (building.transform.position.x > RightBorder.transform.position.x)
            {
                RightBorder.transform.position = building.transform.position + new Vector3(-2, -3);
                Debug.Log($"New Right border position: {RightBorder.transform.position.x}");
            }
        }
    }

    public void AddBuildingToList(Building newBuilding)
    {
        _buildings.Add(newBuilding);
        
        SetBorders();
    }
    
    public void RemoveBuildingFromList(Building newBuilding)
    {
        _buildings.Remove(newBuilding);
        
        SetBorders();
    }

    public void AddSolderToList(AlliedSoldier soldier)
    {
        _alliedSoldiersOnBorders.Add(soldier);
        SelectBorder();
    }
    
    public void RemoveSolderFromList(AlliedSoldier soldier)
    {
        _alliedSoldiersOnBorders.Remove(soldier);
    }

    private void SelectBorder()
    {
        for (int i = 0; i < _alliedSoldiersOnBorders.Count; i++)
        {
            _alliedSoldiersOnBorders[i].SelectBorder(i % 2 == 0 ? LeftBorder : RightBorder);
        }
    }
}
