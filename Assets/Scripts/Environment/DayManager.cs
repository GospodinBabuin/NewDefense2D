using System;
using SaveLoadSystem;
using Steamworks;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayManager : NetworkBehaviour
{
    public static DayManager Instance { get; private set; }
    
    public static event Action OnDayManagerCreated;

    public enum DayState { Day, Evening, Night }
    public DayState dayState { get; private set; } = DayState.Day;

    public delegate void DayStateHandler(DayState dayState, int currentDay);
    public event DayStateHandler OnDayStateChangedEvent;

    [SerializeField] private int _currentDay = 1;
    private float _currentTime;
    private bool _globalLightIntensityOnPosition = true;

    [SerializeField] private Light2D globalLight;
    [SerializeField] private float globalLightIntensity;

    [SerializeField] private float globalLightIntensityChangeSpeed = 15f;

    [SerializeField] private float dayTimeGlobalLightIntensity = 1f;
    [SerializeField] private float eveningTimeGlobalLightIntensity = 0.65f;
    [SerializeField] private float nightTimeGlobalLightIntensity = 0.05f;

    [SerializeField] private int dayTime = 0;

    [SerializeField] private int eveningTime = 60;
    [SerializeField] private int nightTime = 120;
    [SerializeField] private int newDayTime = 300;

    public int CurrentDay => _currentDay;

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
        
        OnDayManagerCreated?.Invoke();
    }

    private void Start()
    {
        SetRichPresence();
        
        GameUI.Instance.Notifications.ShowNewDayNotification(_currentDay);
        GameManager.Instance.OnDefeatEvent += () => { enabled = false; };
    }

    private void FixedUpdate()
    {
        if (IsHost) 
            ChangeDayTime();
    }

    private void SetRichPresence()
    {
        string richPresenceMassage = $"Trying to get through the day {CurrentDay}";
        SteamFriends.SetRichPresence( "steam_player_group", richPresenceMassage );
        
        Debug.Log(richPresenceMassage);
    }

    [ClientRpc]
    public void ChangeGlobalLightIntensityClientRPC(float targetIntensity)
    {
        globalLight.intensity = targetIntensity;
    }

    private void ChangeGlobalLightIntensity(float targetIntensity, DayState targetDayState)
    {
        _globalLightIntensityOnPosition = false;
        if (!_globalLightIntensityOnPosition)
        {
            float intensity;
            if (targetDayState == DayState.Day)
            {
                intensity = globalLight.intensity + Time.deltaTime / globalLightIntensityChangeSpeed;
                ChangeGlobalLightIntensityClientRPC(intensity);
                if (globalLight.intensity >= targetIntensity)
                    ChangeDayStateClientRPC(targetIntensity, targetDayState);
            }
            else
            {
                intensity = globalLight.intensity - Time.deltaTime / globalLightIntensityChangeSpeed;
                ChangeGlobalLightIntensityClientRPC(intensity);
                if (globalLight.intensity <= targetIntensity)
                    ChangeDayStateClientRPC(targetIntensity, targetDayState);
            }
        }
    }

    [ClientRpc]
    private void ChangeDayStateClientRPC(float targetIntensity, DayState targetDayState)
    {
        globalLight.intensity = targetIntensity;
        _globalLightIntensityOnPosition = true;
        dayState = targetDayState;

        if (targetDayState == DayState.Day)
        {
            _currentTime = dayTime;
            _currentDay++;
            SetRichPresence();
            GameUI.Instance.Notifications.ShowNewDayNotification(_currentDay);
        }
        
        OnDayStateChangedEvent?.Invoke(dayState, _currentDay);

        if (IsHost && dayState == DayState.Day)
        {
            SaveLoad.Instance.SaveGame();
        }
    }

    private void ChangeDayTime()
    {
        _currentTime += Time.fixedDeltaTime;

        if (_currentTime >= eveningTime && dayState == DayState.Day)
        {
            ChangeGlobalLightIntensity(eveningTimeGlobalLightIntensity, DayState.Evening);
        }

        if (_currentTime >= nightTime && dayState == DayState.Evening)
        {
            ChangeGlobalLightIntensity(nightTimeGlobalLightIntensity, DayState.Night);
        }

        if (_currentTime >= newDayTime && dayState == DayState.Night || (dayState == DayState.Night && WaveSpawner.Instance.IsMonstersDead()))
        {
            ChangeGlobalLightIntensity(dayTimeGlobalLightIntensity, DayState.Day);
        }
    }

    [ContextMenu("Add30SecToCurrentTime")]
    public void Add30SecToCurrentTime()
    {
        _currentTime += 30f;
    }
    
    [ContextMenu("Add120SecToCurrentTime")]
    public void Add120SecToCurrentTime()
    {
        _currentTime += 120f;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.Instance.OnDefeatEvent -= () => { enabled = false; };
    }
}
