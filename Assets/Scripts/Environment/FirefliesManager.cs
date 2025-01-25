using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    public class FirefliesManager : MonoBehaviour
    {
        public static FirefliesManager Instance;
        
        [SerializeField] private GameObject fireflyHomePrefab;
        [SerializeField] private int firefliesAmount = 100;
        
        [SerializeField] private float minY = -5f;
        [SerializeField] private float maxY = 0f;
        [SerializeField] private float minX = -100f;
        [SerializeField] private float maxX = 100f;
        
        [SerializeField] private List<Firefly> fireflies = new List<Firefly>();

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

        private void Start()
        {
            for (int i = 0; i < firefliesAmount; i++)
            {
                SpawnFirefly();
            }
            DayManager.Instance.OnDayStateChangedEvent += FirefliesStateChange;
        }

        private void SpawnFirefly()
        {
            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
            
            GameObject fireflyHome = Instantiate(fireflyHomePrefab, spawnPosition, Quaternion.identity, transform);
            fireflies.Add(fireflyHome.GetComponentInChildren<Firefly>());
            fireflyHome.SetActive(false);
        }

        [ContextMenu("Enable Fireflies")]
        private void EnableFireflies()
        {
            foreach (Firefly firefly in fireflies)
            { 
                firefly.transform.parent.gameObject.SetActive(true);
            }
        }
        
        [ContextMenu("Disable Fireflies")]
        private void DisableFireflies()
        {
            foreach (Firefly firefly in fireflies)
            { 
                firefly.TurnInactive();
            }
        }
        
        private void FirefliesStateChange(DayManager.DayState dayState, int currentDay)
        {
            switch (dayState)
            {
                case DayManager.DayState.Day:
                    DisableFireflies();
                    return;
                case DayManager.DayState.Night:
                    EnableFireflies();
                    return;
            }
        }
    }
}
