using Unity.Netcode;
using UnityEngine;

public class EntitySpawner : NetworkBehaviour
{
    public static EntitySpawner Instance;

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

    public static AlliedSoldier SpawnUnitOnServer(int unitId)
    {
        GameObject unit = Instantiate(ObjectsDatabase.Instance.UnitDatabase.unitSO[unitId].Prefab, new Vector2(0, -2.5f), Quaternion.identity);
        unit.GetComponent<AlliedSoldier>().SetUnitsId(unitId);
        unit.GetComponent<NetworkObject>().Spawn(true);

        return unit.GetComponent<AlliedSoldier>();
    }

    public void SpawnUnit(int unitId, Vector2 position)
    {
        SpawnUnitServerRPC(unitId, position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnUnitServerRPC(int unitId, Vector2 position)
    {
        GameObject unit = Instantiate(ObjectsDatabase.Instance.UnitDatabase.unitSO[unitId].Prefab, position, Quaternion.identity);
        unit.GetComponent<AlliedSoldier>().SetUnitsId(unitId);
        unit.GetComponent<NetworkObject>().Spawn(true);
    }
}
