using UnityEngine;
using UnityEngine.Pool;

public class GoldSpawner : MonoBehaviour
{
    public static GoldSpawner Instance;
    
    private ObjectPool<Gold> _goldPool;

    [SerializeField] private Gold goldPrefab;

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
        
        _goldPool = new ObjectPool<Gold>(CreatePooledObject, OnTakeFromPool, 
            OnReturnToPool, OnDestroyObject, false, 10, 60);
    }

    private void ReturnObjectToPool(Gold instance)
    {
        _goldPool.Release(instance);
    }

    private Gold CreatePooledObject()
    {
        Gold instance = Instantiate(goldPrefab, Vector3.zero, Quaternion.identity);
        instance.Disable += ReturnObjectToPool;
        instance.gameObject.SetActive(false);

        return instance;
    }

    private void OnTakeFromPool(Gold instance)
    {
        instance.gameObject.SetActive(true);
        instance.transform.SetParent(transform, true);
    }

    private void OnReturnToPool(Gold instance)
    {
        instance.gameObject.SetActive(false);
    }

    private void OnDestroyObject(Gold instance)
    {
        Destroy(instance.gameObject);
    }

    public void SpawnGold(Vector2 position, Quaternion quaternion)
    {
        Gold instance = _goldPool.Get();
        
        instance.transform.position = position;
        instance.transform.rotation = quaternion;
    }
}
