using UnityEngine;

public class ObjectsDatabase : MonoBehaviour
{
    public static ObjectsDatabase Instance;

    public BuildingDatabaseSO BuildingDatabase { get => buildingDatabase; private set => buildingDatabase = value; }
    public UnitDatabaseSO UnitDatabase { get => unitDatabase; private set => unitDatabase = value; }
    public PotionDatabaseSO PotionDatabase { get => potionDatabase; private set => potionDatabase = value; }

    [SerializeField] private BuildingDatabaseSO buildingDatabase;
    [SerializeField] private UnitDatabaseSO unitDatabase;
    [SerializeField] private PotionDatabaseSO potionDatabase;

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
}
