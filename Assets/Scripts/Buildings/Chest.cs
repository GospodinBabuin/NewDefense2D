using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Buildings
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private Transform goldSpawner;
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private float goldSpawnDelay = 0.05f;
    
        private Animator _animator;

        private int _animIDOpen;
        private int _animIDClose;
        private int _animIDDestroy;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            SetAnimIDs();
        }

        private void SetAnimIDs()
        {
            _animIDOpen = Animator.StringToHash("Open");
            _animIDClose = Animator.StringToHash("Close");
            _animIDDestroy = Animator.StringToHash("Destroy");
        }
    
        public void CollectGold(int goldCount)
        {
            _animator.SetTrigger(_animIDOpen);

            StartCoroutine(SpawnGold(goldCount));
        }

        public void DestroyChest()
        {
            _animator.SetTrigger(_animIDDestroy);
            
            Destroy(gameObject, _animator.GetCurrentAnimatorStateInfo(0).length);
        }

        private IEnumerator SpawnGold(int goldCount)
        {
            for (int i = 0; i < goldCount; i++)
            {
                GoldSpawner.Instance.SpawnGold(GetRandomPoint(goldSpawner.position), quaternion.identity);
                yield return new WaitForSeconds(goldSpawnDelay);
            }
        
            _animator.SetTrigger(_animIDClose);
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
