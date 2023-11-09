using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveSpawner : MonoBehaviour
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
    [SerializeField] int waveType = 0;
    [SerializeField] private int[] daysWithNewWave;

    [SerializeField] private Transform[] spawnPoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        DayManager.Instance.OnDayStateChangedEvent += CheckDay;
    }

    private void CheckDay(DayManager.DayState dayState, int day)
    {
        switch (dayState)
        {
            case DayManager.DayState.DAY:
                StartCoroutine(SpawnWave(Waves[waveType], day));
                break;

            case DayManager.DayState.NIGHT:
                CheckDayWithNewWave(day);
                break;
        }
    }

    private void CheckDayWithNewWave(int day)
    {
        for (int i = 0; i < daysWithNewWave.Length; i++)
        {
            if (day == daysWithNewWave[i])
            {
                waveType = i;
            }
        }
    }

    private IEnumerator SpawnWave(Wave wave, int day)
    {
        Debug.Log("spawning wave: " + wave.Name + " day: " + day);

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
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }
}
