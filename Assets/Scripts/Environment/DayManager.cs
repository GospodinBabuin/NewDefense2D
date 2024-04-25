using UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    public enum DayState { Day, Evening, Night }
    public DayState dayState { get; private set; } = DayState.Day;

    public delegate void DayStateHandler(DayState dayState, int currentDay);
    public event DayStateHandler OnDayStateChangedEvent;

    [SerializeField] private int _currentDay = 1;
    [SerializeField] private float _currentTime;
    private bool _globalLightIntensityOnPosition = true;

    [SerializeField] private Light2D globalLight;

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
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        GameUI.Instance.Notifications.ShowNewDayNotification(_currentDay);
    }

    private void FixedUpdate()
    {
        ChangeDayTime();
    }

    private void ChangeDayTime()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= eveningTime && dayState == DayState.Day)
        {
            _globalLightIntensityOnPosition = false;
            if (!_globalLightIntensityOnPosition)
            {
                globalLight.intensity -= Time.deltaTime / globalLightIntensityChangeSpeed;

                if (globalLight.intensity <= eveningTimeGlobalLightIntensity)
                {
                    globalLight.intensity = eveningTimeGlobalLightIntensity;
                    _globalLightIntensityOnPosition = true;
                    dayState = DayState.Evening;

                    OnDayStateChangedEvent?.Invoke(dayState, _currentDay);
                    return;
                }
            }
        }

        if (_currentTime >= nightTime && dayState == DayState.Evening)
        {
            _globalLightIntensityOnPosition = false;
            if (!_globalLightIntensityOnPosition)
            {
                globalLight.intensity -= Time.deltaTime / globalLightIntensityChangeSpeed;

                if (globalLight.intensity <= nightTimeGlobalLightIntensity)
                {
                    globalLight.intensity = nightTimeGlobalLightIntensity;
                    _globalLightIntensityOnPosition = true;
                    dayState = DayState.Night;

                    OnDayStateChangedEvent?.Invoke(dayState, _currentDay);
                    return;
                }
            }
        }

        if (_currentTime >= newDayTime && dayState == DayState.Night || (dayState == DayState.Night && WaveSpawner.Instance.IsMonstersDead()))
        {
            _globalLightIntensityOnPosition = false;
            if (!_globalLightIntensityOnPosition)
            {
                globalLight.intensity += Time.deltaTime / globalLightIntensityChangeSpeed;

                if (globalLight.intensity >= dayTimeGlobalLightIntensity)
                {
                    globalLight.intensity = dayTimeGlobalLightIntensity;
                    _globalLightIntensityOnPosition = true;
                    dayState = DayState.Day;
                    _currentTime = dayTime;
                    _currentDay++;

                    GameUI.Instance.Notifications.ShowNewDayNotification(_currentDay);

                    OnDayStateChangedEvent?.Invoke(dayState, _currentDay);
                    return;
                }
            }
        }
    }
}
