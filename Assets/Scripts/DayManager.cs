using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    public enum DayState { DAY, EVENING, NIGHT }
    public DayState dayState { get; private set; } = DayState.DAY;

    public delegate void DayStateHandler(DayState dayState, int currentDay);
    public event DayStateHandler OnDayStateChangedEvent;

    [SerializeField] private int _currentDay = 1;
    [SerializeField] private float _currentTime;
    private bool _globalLightIntensityOnPosition = true;

    [SerializeField] private Light2D GlobalLight;

    [SerializeField] private float GlobalLightIntensityChangeSpeed = 15f;

    [SerializeField] private float dayTimeGlobalLightIntensity = 1f;
    [SerializeField] private float eveningTimeGlobalLightIntensity = 0.65f;
    [SerializeField] private float nightTimeGlobalLightIntensity = 0.05f;

    [SerializeField] private int dayTime = 0;

    [SerializeField] private int eveningTime = 90;
    [SerializeField] private int nightTime = 180;
    [SerializeField] private int newDayTime = 360;

    public int CurrentDay { get { return _currentDay; } }

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

    private void FixedUpdate()
    {
        ChangeDayTime();
    }

    private void ChangeDayTime()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= eveningTime && dayState == DayState.DAY)
        {
            _globalLightIntensityOnPosition = false;
            if (!_globalLightIntensityOnPosition)
            {
                GlobalLight.intensity -= Time.deltaTime / GlobalLightIntensityChangeSpeed;

                if (GlobalLight.intensity <= eveningTimeGlobalLightIntensity)
                {
                    GlobalLight.intensity = eveningTimeGlobalLightIntensity;
                    _globalLightIntensityOnPosition = true;
                    dayState = DayState.EVENING;

                    OnDayStateChangedEvent?.Invoke(dayState, _currentDay);
                    return;
                }
            }
        }

        if (_currentTime >= nightTime && dayState == DayState.EVENING)
        {
            _globalLightIntensityOnPosition = false;
            if (!_globalLightIntensityOnPosition)
            {
                GlobalLight.intensity -= Time.deltaTime / GlobalLightIntensityChangeSpeed;

                if (GlobalLight.intensity <= nightTimeGlobalLightIntensity)
                {
                    GlobalLight.intensity = nightTimeGlobalLightIntensity;
                    _globalLightIntensityOnPosition = true;
                    dayState = DayState.NIGHT;

                    OnDayStateChangedEvent?.Invoke(dayState, _currentDay);
                    return;
                }
            }
        }

        if (_currentTime >= newDayTime && dayState == DayState.NIGHT || (dayState == DayState.NIGHT && WaveSpawner.Instance.IsMonstersDead()))
        {
            _globalLightIntensityOnPosition = false;
            if (!_globalLightIntensityOnPosition)
            {
                GlobalLight.intensity += Time.deltaTime / GlobalLightIntensityChangeSpeed;

                if (GlobalLight.intensity >= dayTimeGlobalLightIntensity)
                {
                    GlobalLight.intensity = dayTimeGlobalLightIntensity;
                    _globalLightIntensityOnPosition = true;
                    dayState = DayState.DAY;
                    _currentTime = dayTime;
                    _currentDay++;

                    OnDayStateChangedEvent?.Invoke(dayState, _currentDay);
                    return;
                }
            }
        }
    }
}
