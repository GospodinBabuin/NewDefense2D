using System;
using Buildings;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    public class DailyChestManager : NetworkBehaviour
    {
        [SerializeField] private Transform leftSpawnBorderEnd;
        [SerializeField] private Transform rightSpawnBorderEnd;
        
        [SerializeField] private DailyChest dailyChest;
        
        private void Start()
        {
            SubscribeToDayManager();
        }
        
        private void EnableDailyChest(DayManager.DayState dayState, int currentDay)
        {
            if (!IsHost) return;
            if (dayState != DayManager.DayState.Day) return;
            
            int randomNumber = Random.Range(0, 2);
            float spawnBorderStart = 0;
            float randomSpawnPoint = 0;
            
            switch (randomNumber)
            {
                case 0:
                    spawnBorderStart = GreenZoneBorders.Instance.LeftBorder.position.x;
                    randomSpawnPoint = Random.Range(spawnBorderStart - 1, leftSpawnBorderEnd.position.x + 1);
                    break;
                
                case 1:
                    spawnBorderStart = GreenZoneBorders.Instance.RightBorder.position.x;
                    randomSpawnPoint = Random.Range(spawnBorderStart + 1, rightSpawnBorderEnd.position.x - 1);
                    break;
            }
            
            EnableDailyChestClientRpc(randomSpawnPoint, currentDay);
        }

        [ClientRpc]
        private void EnableDailyChestClientRpc(float spawnPoint, int currentDay)
        {
            dailyChest.transform.position = new Vector2(spawnPoint, dailyChest.transform.position.y);
            dailyChest.gameObject.SetActive(true);
            dailyChest.GoldInChest = currentDay * 10;
        }
        
        private void SubscribeToDayManager()
        {
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnDayStateChangedEvent += EnableDailyChest;
                DayManager.OnDayManagerCreated -= SubscribeToDayManager;
            }
            else
            {
                DayManager.OnDayManagerCreated += SubscribeToDayManager;
            }
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            
            DayManager.Instance.OnDayStateChangedEvent -= EnableDailyChest;
        }
    }
}
