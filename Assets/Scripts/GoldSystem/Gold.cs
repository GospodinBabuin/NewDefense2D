using System.Linq;
using Network;
using Unity.Netcode;
using UnityEngine;

namespace GoldSystem
{
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Animator))]
    public class Gold : NetworkBehaviour
    {
        private Animator _animator;
        private int _animIDCollect;
        private int _animIDSpawn;
        
        private NetworkVariable<bool> _isCollected = new NetworkVariable<bool>(false);
    
        public delegate void OnDisableCallback(Gold instance);
        public OnDisableCallback Disable;
    
        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _animIDCollect = Animator.StringToHash("Collect");
            _animIDSpawn = Animator.StringToHash("Spawn");
        }

        private void CoinCollectedAnimEvent()
        {
            DespawnCoinServerRPC();
        }
    
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_isCollected.Value) return;
            if (!other.gameObject.TryGetComponent(out PlayerController player)) return;
                    
            _isCollected.Value = true;
            
            if (player.GetComponent<NetworkObject>().IsOwner)
            {
                GoldBank.Instance.AddGold(this, 1);
            }
            _animator.SetTrigger(_animIDCollect);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnCoinServerRPC()
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            GameObject prefab = NetworkObjectPool.Singleton.PooledPrefabsList.FirstOrDefault(o => o.Prefab.name == "Gold").Prefab;
            NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);
            if (networkObject.IsSpawned)
                networkObject.Despawn(false);
        }

        private void OnEnable()
        {
            _isCollected.Value = false;
            _animator.SetTrigger(_animIDSpawn);
        }
    }
}
