using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance {  get; private set; }

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

    private bool _isMonstersAppear = false;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CheckDayWithNewWave();
    }

    private void FixedUpdate()
    {
        if (DayManager.Instance.dayState == DayManager.DayState.NIGHT && !_isMonstersAppear)
        {
            StartCoroutine(SpawnWave(Waves[waveType]));
        }
        if (DayManager.Instance.dayState == DayManager.DayState.DAY && _isMonstersAppear)
        {
            CheckDayWithNewWave();
            _isMonstersAppear = false;
        }
    }

    private void CheckDayWithNewWave()
    {
        bool needToPlayMusic = false;

        for (int i = 0; i < daysWithNewWave.Length; i++)
        {
            if (DayManager.Instance.DayCount == daysWithNewWave[i])
            {
                waveType = i;
                needToPlayMusic = true;
            }
        }

        if (needToPlayMusic)
            _newWaveMusic.Play();
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log("spawning wave: " + wave.Name);
        _isMonstersAppear = true;

        for (int i = 0; i < wave.Count + DayManager.Instance.DayCount; i++)
        {
            SpawnEnemy(wave.Enemy[Random.Range(0, wave.Enemy.Length)]);
            yield return new WaitForSeconds(1);
        }

        DayManager.Instance.CanCheckIsMonstersDead = true;
        yield break;
    }

    private void SpawnEnemy(Transform enemy)
    {
        Debug.Log("spawning enemy: " + enemy.name);
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }


    public bool IsMonstersDead()
    {
        if (ObjectsInWorld.Instance.CanCheckIsMonstersDead)
        {
            if (ObjectsInWorld.Instance.EnemyList.Count == 0)
                return true;
            else
                return false;
        }

        return false;
    }
}
