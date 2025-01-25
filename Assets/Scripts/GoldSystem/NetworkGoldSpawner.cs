using Network;
using Unity.Netcode;
using UnityEngine;

namespace GoldSystem
{
    public class NetworkGoldSpawner : NetworkBehaviour
    {
        public static NetworkGoldSpawner Instance;

        [SerializeField] private GameObject goldPrefab;

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
    
        public void SpawnGold(Vector2 position, Quaternion quaternion)
        {
            if (!IsHost) return;
        
            NetworkObject gold = NetworkObjectPool.Singleton.GetNetworkObject(goldPrefab, position, quaternion);
            if (!gold.IsSpawned) gold.Spawn(true);
        }
    }
}
