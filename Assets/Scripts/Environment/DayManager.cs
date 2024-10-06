using SaveLoadSystem;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayManager : NetworkBehaviour
{
    public static DayManager Instance { get; private set; }

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
    }

    private void Start()
    {
        GameUI.Instance.Notifications.ShowNewDayNotification(_currentDay);
    }

    private void FixedUpdate()
    {
        if (IsHost) 
            ChangeDayTime();
    }

    [ClientRpc]
    public void ChangeGlobalLightIntensityClientRPC(float targetIntencity)
    {
        globalLight.intensity = targetIntencity;
    }

    private void ChangeGlobalLightIntensity(float targetIntencity, DayState tragetDayState)
    {
        _globalLightIntensityOnPosition = false;
        if (!_globalLightIntensityOnPosition)
        {
            float intensity;
            if (tragetDayState == DayState.Day)
            {
                intensity = globalLight.intensity + Time.deltaTime / globalLightIntensityChangeSpeed;
                ChangeGlobalLightIntensityClientRPC(intensity);
                if (globalLight.intensity >= targetIntencity)
                    ChangeDayStateClientRPC(targetIntencity, tragetDayState);
            }
            else
            {
                intensity = globalLight.intensity - Time.deltaTime / globalLightIntensityChangeSpeed;
                ChangeGlobalLightIntensityClientRPC(intensity);
                if (globalLight.intensity <= targetIntencity)
                    ChangeDayStateClientRPC(targetIntencity, tragetDayState);
            }
        }
    }

    [ClientRpc]
    private void ChangeDayStateClientRPC(float targetIntencity, DayState tragetDayState)
    {
        globalLight.intensity = targetIntencity;
        _globalLightIntensityOnPosition = true;
        dayState = tragetDayState;

        if (tragetDayState == DayState.Day)
        {
            _currentTime = dayTime;
            _currentDay++;
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
}
