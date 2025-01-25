using System.Collections;
using GoldSystem;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Buildings
{
    public class Chest : NetworkBehaviour
    {
        [SerializeField] private Transform goldSpawner;
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private float goldSpawnDelay = 0.05f;
    
        protected Animator animator;

        protected int animIDOpen;
        protected int animIDClose;
        protected int animIDDestroy;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            SetAnimIDs();
        }

        private void SetAnimIDs()
        {
            animIDOpen = Animator.StringToHash("Open");
            animIDClose = Animator.StringToHash("Close");
            animIDDestroy = Animator.StringToHash("Destroy");
        }
    
        public void CollectGold(int goldCount)
        {
            animator.SetTrigger(animIDOpen);
            
            if (!IsHost) return;
            
            StartCoroutine(SpawnGold(goldCount));
        }

        public void DestroyChest()
        {
            animator.SetTrigger(animIDDestroy);
            
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }

        private IEnumerator SpawnGold(int goldCount)
        {
            for (int i = 0; i < goldCount; i++)
            {
                NetworkGoldSpawner.Instance.SpawnGold(GetRandomPoint(goldSpawner.position), quaternion.identity);
                yield return new WaitForSeconds(goldSpawnDelay);
            }
        
            animator.SetTrigger(animIDClose);
        }

        private Vector2 GetRandomPoint(Vector2 oldPos)
        {
            Vector2 spawnPoint = oldPos;

            spawnPoint.x += Random.Range(-radius, radius);
            spawnPoint.y += Random.Range(-radius, radius);
        
            return spawnPoint;
        }
    }
}
