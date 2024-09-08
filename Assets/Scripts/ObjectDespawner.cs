using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ObjectDespawner : MonoBehaviour
{
    public static ObjectDespawner Instance { get; private set; }
    
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

    [ServerRpc(RequireOwnership = false)]
    public void DespawnObjectServerRPC(GameObject objectToDespawn, float timeBeforeDespawn)
    {
        StartCoroutine(DespawnObjectCoroutine(objectToDespawn, timeBeforeDespawn));
    }

    private IEnumerator DespawnObjectCoroutine(GameObject objectToDespawn, float timeBeforeDespawn)
    {
        yield return new WaitForSeconds(timeBeforeDespawn);
        
        objectToDespawn.GetComponent<NetworkObject>().Despawn(true);
    }
}
