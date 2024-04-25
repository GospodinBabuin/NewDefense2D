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
        ObjectsInWorld.Instance.OnBuildingsListChangedEvent += SetBorders;
    }

    private void SetBorders(List<Building> buildings)
    {
        LeftBorder.position = defaultLeftBorder.position;
        RightBorder.position = defaultRightBorder.position;

        if (buildings.Count == 0) return;

        foreach (Building building in buildings)
        {
            if (building.transform.position.x < LeftBorder.transform.position.x)
            {
                LeftBorder.transform.position = building.transform.position + new Vector3(2, -3);
                Debug.Log($"New left border position: {LeftBorder.transform.position.x}");
                continue;
            }

            if (building.transform.position.x > RightBorder.transform.position.x)
            {
                RightBorder.transform.position = building.transform.position + new Vector3(-2, -3);
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
            soldier.SelectBorder(LeftBorder);
            _soldiersOnLeftBorder++;
        }
        else
        {
            soldier.SelectBorder(RightBorder);
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
}
