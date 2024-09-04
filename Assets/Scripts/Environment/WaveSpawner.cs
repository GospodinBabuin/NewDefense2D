using System.Collections;
using Environment;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveSpawner : NetworkBehaviour
{
    public static WaveSpawner Instance { get; private set; }

    [System.Serializable]
    private class Wave
    {
        public string Name;
        public int Count = 3;
        public Transform[] Enemy;
    }

    [SerializeField] private Wave[] Waves;
    [SerializeField] private int waveType = 0;
    [SerializeField] private int[] daysWithNewWave;

    [SerializeField] private Transform[] spawnPoints;

    private bool _isMonstersAppeared = false;

    private void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DayManager.Instance.OnDayStateChangedEvent += CheckDay;

        CheckDayWithNewWave(DayManager.Instance.CurrentDay);
    }

    private void CheckDay(DayManager.DayState dayState, int day)
    {
        switch (dayState)
        {
            case DayManager.DayState.Day:
                if (_isMonstersAppeared)
                {
                    CheckDayWithNewWave(day);
                    _isMonstersAppeared = false;
                }
                break;

            case DayManager.DayState.Night:
                if (!_isMonstersAppeared)
                    StartCoroutine(SpawnWave(Waves[waveType], day));
                break;
        }
    }

    private void CheckDayWithNewWave(int day)
    {
        for (int i = 0; i < daysWithNewWave.Length; i++)
        {
            if (day >= daysWithNewWave[i])
            {
                waveType = i + 1;
            }
        }
    }

    private IEnumerator SpawnWave(Wave wave, int day)
    {
        Debug.Log("spawning wave: " + wave.Name + " day: " + day);

        _isMonstersAppeared = true;

        for (int i = 0; i < wave.Count + day; i++)
        {
            SpawnEnemy(wave.Enemy[Random.Range(0, wave.Enemy.Length)]);
            yield return new WaitForSeconds(1);
        }

        yield break;
    }

    private void SpawnEnemy(Transform enemy)
    {
        Debug.Log("spawning enemy: " + enemy.name);
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Transform newEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);
        newEnemy.GetComponent<NetworkObject>().Spawn(true);
    }

    public bool IsMonstersDead()
    {
        return (_isMonstersAppeared && ObjectsInWorld.Instance.Enemies.Count == 0);
    }
}
