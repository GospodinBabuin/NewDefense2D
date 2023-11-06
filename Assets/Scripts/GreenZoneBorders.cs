using System.Collections.Generic;
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
        ObjectsInWorld.Instance.OnAlliedSoldersListChangedEvent += SelectBorder;
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

    private void SelectBorder(List<AlliedSoldier> soldiers, AlliedSoldier solder)
    {
        soldiers[soldiers.IndexOf(solder)]
            .SelectBorder(soldiers.IndexOf(solder) % 2 == 0 ? LeftBorder : RightBorder);
    }
}
