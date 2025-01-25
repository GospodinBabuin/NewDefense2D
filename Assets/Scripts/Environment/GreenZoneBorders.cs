using System;
using System.Collections.Generic;
using Buildings;
using Environment;
using UnityEngine;

public class GreenZoneBorders : MonoBehaviour
{
    public static GreenZoneBorders Instance { get; private set; }

    public delegate void BordersHandler();
    public event BordersHandler OnBordersPositionChangedEvent;

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

    private int _soldiersOnLeftBorder = 0;
    private int _soldiersOnRightBorder = 0;
    
    private Vector2 leftBorderPositionOffset = new Vector2(1, -3);
    private Vector2 rightBorderPositionOffset = new Vector2(-1, -3);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ObjectsInWorld.Instance.OnBuildingsListChangedEvent += SetBorders;
    }

    private void SetBorders(List<Building> buildings)
    {
        LeftBorder.position = defaultLeftBorder.position;
        RightBorder.position = defaultRightBorder.position;

        if (buildings.Count == 0) return;

        foreach (Building building in buildings)
        {
            if (building.CompareTag("RoadLamp")) continue;
            
            if (building.transform.position.x < LeftBorder.transform.position.x)
            {
                LeftBorder.transform.position = building.transform.position;
                Debug.Log($"New left border position: {LeftBorder.transform.position.x}");
                continue;
            }

            if (building.transform.position.x > RightBorder.transform.position.x)
            {
                RightBorder.transform.position = building.transform.position;
                Debug.Log($"New Right border position: {RightBorder.transform.position.x}");
                continue;
            }
        }

        OnBordersPositionChangedEvent?.Invoke();
    }
    
    public void SelectBorder(AlliedSoldier soldier)
    {
        if (_soldiersOnLeftBorder <= _soldiersOnRightBorder)
        {
            soldier.SelectBorder(LeftBorder, leftBorderPositionOffset);
            _soldiersOnLeftBorder++;
        }
        else
        {
            soldier.SelectBorder(RightBorder, rightBorderPositionOffset);
            _soldiersOnRightBorder++;
        }
    }

    public void RemoveFromBorder(AlliedSoldier soldier)
    {
        if (soldier.SelectedBorder == LeftBorder)
        {
            _soldiersOnLeftBorder--;
            return;
        }

        if (soldier.SelectedBorder == RightBorder)
        {
            _soldiersOnRightBorder--;
            return;
        }
    }

    public bool IsBeyondGreenZoneBorders(Vector2 objectsPosition)
    {
        return (objectsPosition.x > LeftBorder.position.x &&
            objectsPosition.x < RightBorder.position.x);
    }

    public bool IsBeyondDefaultGreenZoneBorders(Vector2 objectsPosition)
    {
        return (objectsPosition.x > defaultLeftBorder.position.x &&
            objectsPosition.x < defaultRightBorder.position.x);
    }

    private void OnDestroy()
    {
        ObjectsInWorld.Instance.OnBuildingsListChangedEvent -= SetBorders;
    }
}
